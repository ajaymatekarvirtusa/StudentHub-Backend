using Data.Common;
using Data.Entities;
using Data.Models.Masters;
using Microsoft.Extensions.Logging;
using Repositories.Interface;
using Services.Interface.Masters;

namespace Services.Service.Masters
{
    public class CountryService(IGenericRepository<Country> _repository, ILogger<CountryService> _logger) : ICountryService
    {
        public async Task<PagedResult<CountryResponse>> GetPagedAsync(CountryQuery query, CancellationToken cancellationToken = default)
        {
            var search = string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim();
            var activeOnly = query.ActiveOnly;

            var page = await _repository.GetPagedAsync(
                query.PageNumber,
                query.PageSize,
                c => (!activeOnly || c.IsActive) && (search == null || c.Name.Contains(search) || c.Code.Contains(search)),
                cancellationToken);

            return page.Map(CountryResponse.From);
        }

        public async Task<CountryResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var country = await _repository.GetByIdAsync(id, cancellationToken);
            return country is null ? null : CountryResponse.From(country);
        }

        public async Task<ServiceResult<CountryResponse>> CreateAsync(CountryRequest request, CancellationToken cancellationToken = default)
        {
            var name = request.Name.Trim();
            var code = request.Code.Trim().ToUpperInvariant();

            var duplicate = await FindDuplicateAsync(name, code, excludeId: null, cancellationToken);
            if (duplicate is not null)
                return ServiceResult<CountryResponse>.Conflict(duplicate);

            var country = await _repository.AddAsync(new Country { Name = name, Code = code, IsActive = request.IsActive }, cancellationToken);

            _logger.LogInformation("Country created. CountryId={CountryId} Code={Code}", country.Id, country.Code);
            return ServiceResult<CountryResponse>.Success(CountryResponse.From(country));
        }

        public async Task<ServiceResult<CountryResponse>> UpdateAsync(int id, CountryRequest request, CancellationToken cancellationToken = default)
        {
            var country = await _repository.GetByIdAsync(id, cancellationToken);
            if (country is null)
                return ServiceResult<CountryResponse>.NotFound("Country not found.");

            var name = request.Name.Trim();
            var code = request.Code.Trim().ToUpperInvariant();

            var duplicate = await FindDuplicateAsync(name, code, excludeId: id, cancellationToken);
            if (duplicate is not null)
                return ServiceResult<CountryResponse>.Conflict(duplicate);

            country.Name = name;
            country.Code = code;
            country.IsActive = request.IsActive;
            await _repository.UpdateAsync(country, cancellationToken);

            _logger.LogInformation("Country updated. CountryId={CountryId}", id);
            return ServiceResult<CountryResponse>.Success(CountryResponse.From(country));
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var deleted = await _repository.DeleteAsync(id, cancellationToken);

            if (deleted)
                _logger.LogInformation("Country deleted. CountryId={CountryId}", id);
            else
                _logger.LogWarning("Country delete failed - not found. CountryId={CountryId}", id);

            return deleted;
        }

        /// <summary>Returns an error message if another active row already uses this name or code.</summary>
        private async Task<string?> FindDuplicateAsync(string name, string code, int? excludeId, CancellationToken cancellationToken)
        {
            if (await _repository.ExistsAsync(c => c.Name == name && c.Id != excludeId, cancellationToken))
                return $"A country named '{name}' already exists.";

            if (await _repository.ExistsAsync(c => c.Code == code && c.Id != excludeId, cancellationToken))
                return $"A country with code '{code}' already exists.";

            return null;
        }
    }
}
