using Data.Common;
using Data.Entities;
using Data.Models.Masters;
using Microsoft.Extensions.Logging.Abstractions;
using Services.Service.Masters;
using StudentHub.Tests.Fakes;

namespace StudentHub.Tests.Services.Masters
{
    public class CountryServiceTests
    {
        private readonly FakeGenericRepository<Country> _repository = new();
        private readonly CountryService _service;

        public CountryServiceTests()
        {
            _service = new CountryService(_repository, NullLogger<CountryService>.Instance);
        }

        private static CountryRequest Request(string name = "India", string code = "IN", bool isActive = true) =>
            new() { Name = name, Code = code, IsActive = isActive };

        [Fact]
        public async Task CreateAsync_TrimsNameAndUppercasesCode()
        {
            var result = await _service.CreateAsync(Request("  India  ", " in "));

            Assert.True(result.IsSuccess);
            Assert.Equal("India", result.Value!.Name);
            Assert.Equal("IN", result.Value.Code);
            Assert.Equal(1, result.Value.Id);
        }

        [Fact]
        public async Task CreateAsync_DuplicateName_ReturnsConflict()
        {
            await _service.CreateAsync(Request("India", "IN"));

            var result = await _service.CreateAsync(Request("India", "IND"));

            Assert.Equal(ServiceResultStatus.Conflict, result.Status);
            Assert.Equal("A country named 'India' already exists.", result.Error);
        }

        [Fact]
        public async Task CreateAsync_DuplicateCode_ReturnsConflict()
        {
            await _service.CreateAsync(Request("India", "IN"));

            var result = await _service.CreateAsync(Request("Indonesia", "in"));

            Assert.Equal(ServiceResultStatus.Conflict, result.Status);
            Assert.Equal("A country with code 'IN' already exists.", result.Error);
        }

        [Fact]
        public async Task CreateAsync_AfterDelete_AllowsSameNameAgain()
        {
            var first = await _service.CreateAsync(Request());
            await _service.DeleteAsync(first.Value!.Id);

            var again = await _service.CreateAsync(Request());

            Assert.True(again.IsSuccess);
        }

        [Fact]
        public async Task UpdateAsync_WhenNotFound_ReturnsNotFound()
        {
            var result = await _service.UpdateAsync(99, Request());

            Assert.Equal(ServiceResultStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task UpdateAsync_KeepingSameName_IsNotADuplicate()
        {
            var created = await _service.CreateAsync(Request("India", "IN"));

            var result = await _service.UpdateAsync(created.Value!.Id, Request("India", "IND", isActive: false));

            Assert.True(result.IsSuccess);
            Assert.Equal("IND", result.Value!.Code);
            Assert.False(result.Value.IsActive);
        }

        [Fact]
        public async Task UpdateAsync_ToAnotherCountrysName_ReturnsConflict()
        {
            await _service.CreateAsync(Request("India", "IN"));
            var nepal = await _service.CreateAsync(Request("Nepal", "NP"));

            var result = await _service.UpdateAsync(nepal.Value!.Id, Request("India", "NP"));

            Assert.Equal(ServiceResultStatus.Conflict, result.Status);
        }

        private async Task SeedAsync(int count)
        {
            for (var i = 1; i <= count; i++)
                await _service.CreateAsync(Request($"Country {(char)('A' + i - 1)}", $"C{(char)('A' + i - 1)}", isActive: i % 2 == 1));
        }

        [Fact]
        public async Task GetPagedAsync_DefaultPageSize_Returns5Records()
        {
            await SeedAsync(12);

            var page = await _service.GetPagedAsync(new CountryQuery());

            Assert.Equal(5, page.Items.Count);
            Assert.Equal(1, page.PageNumber);
            Assert.Equal(5, page.PageSize);
            Assert.Equal(12, page.TotalCount);
            Assert.Equal(3, page.TotalPages);
            Assert.False(page.HasPreviousPage);
            Assert.True(page.HasNextPage);
        }

        [Fact]
        public async Task GetPagedAsync_LastPage_ReturnsRemainingRecords()
        {
            await SeedAsync(12);

            var page = await _service.GetPagedAsync(new CountryQuery { PageNumber = 3 });

            Assert.Equal(2, page.Items.Count);
            Assert.Equal("Country K", page.Items[0].Name);
            Assert.True(page.HasPreviousPage);
            Assert.False(page.HasNextPage);
        }

        [Fact]
        public async Task GetPagedAsync_PageBeyondEnd_ReturnsEmptyItems()
        {
            await SeedAsync(3);

            var page = await _service.GetPagedAsync(new CountryQuery { PageNumber = 5 });

            Assert.Empty(page.Items);
            Assert.Equal(3, page.TotalCount);
        }

        [Fact]
        public async Task GetPagedAsync_ActiveOnly_CountsOnlyActive()
        {
            await SeedAsync(12); // odd numbers are active -> 6 active

            var page = await _service.GetPagedAsync(new CountryQuery { ActiveOnly = true, PageSize = 10 });

            Assert.Equal(6, page.TotalCount);
            Assert.All(page.Items, c => Assert.True(c.IsActive));
        }

        [Fact]
        public async Task GetPagedAsync_Search_MatchesNameOrCode()
        {
            await _service.CreateAsync(Request("India", "IN"));
            await _service.CreateAsync(Request("Indonesia", "ID"));
            await _service.CreateAsync(Request("Nepal", "NP"));

            Assert.Equal(2, (await _service.GetPagedAsync(new CountryQuery { Search = "Ind" })).TotalCount);
            Assert.Equal("Nepal", Assert.Single((await _service.GetPagedAsync(new CountryQuery { Search = " NP " })).Items).Name);
        }

        [Fact]
        public async Task GetPagedAsync_ExcludesDeleted()
        {
            var india = await _service.CreateAsync(Request("India", "IN"));
            await _service.CreateAsync(Request("Nepal", "NP"));
            await _service.DeleteAsync(india.Value!.Id);

            Assert.Equal(1, (await _service.GetPagedAsync(new CountryQuery())).TotalCount);
        }

        [Fact]
        public async Task DeleteAsync_HidesCountry_AndSecondDeleteReturnsFalse()
        {
            var created = await _service.CreateAsync(Request());

            Assert.True(await _service.DeleteAsync(created.Value!.Id));
            Assert.Null(await _service.GetByIdAsync(created.Value.Id));
            Assert.False(await _service.DeleteAsync(created.Value.Id));
        }
    }
}
