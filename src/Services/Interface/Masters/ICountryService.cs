using Data.Common;
using Data.Models.Masters;

namespace Services.Interface.Masters
{
    public interface ICountryService
    {
        Task<PagedResult<CountryResponse>> GetPagedAsync(CountryQuery query, CancellationToken cancellationToken = default);
        Task<CountryResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ServiceResult<CountryResponse>> CreateAsync(CountryRequest request, CancellationToken cancellationToken = default);
        Task<ServiceResult<CountryResponse>> UpdateAsync(int id, CountryRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
