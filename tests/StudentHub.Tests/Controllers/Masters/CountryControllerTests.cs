using API.Controllers.Masters;
using API.Models;
using Data.Common;
using Data.Entities;
using Data.Models.Masters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Services.Service.Masters;
using StudentHub.Tests.Fakes;

namespace StudentHub.Tests.Controllers.Masters
{
    public class CountryControllerTests
    {
        private readonly CountryController _controller;

        public CountryControllerTests()
        {
            var service = new CountryService(new FakeGenericRepository<Country>(), NullLogger<CountryService>.Instance);
            _controller = new CountryController(service, NullLogger<CountryController>.Instance);
        }

        private static CountryRequest India => new() { Name = "India", Code = "IN" };

        [Fact]
        public async Task Create_NullBody_ReturnsBadRequest()
        {
            var result = await _controller.Create(null, default);

            Assert.Equal("Country details are required.", Assert.IsType<ErrorResponse>(Assert.IsType<BadRequestObjectResult>(result).Value).Error);
        }

        [Fact]
        public async Task Create_Valid_ReturnsCreatedAtGetById()
        {
            var result = await _controller.Create(India, default);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(CountryController.GetById), created.ActionName);
            Assert.Equal("India", Assert.IsType<CountryResponse>(created.Value).Name);
        }

        [Fact]
        public async Task Create_Duplicate_ReturnsConflict()
        {
            await _controller.Create(India, default);

            var result = await _controller.Create(India, default);

            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task GetById_Missing_ReturnsNotFound()
        {
            Assert.IsType<NotFoundObjectResult>(await _controller.GetById(5, default));
        }

        [Fact]
        public async Task Update_Missing_ReturnsNotFound()
        {
            Assert.IsType<NotFoundObjectResult>(await _controller.Update(5, India, default));
        }

        [Fact]
        public async Task Update_Existing_ReturnsOk()
        {
            await _controller.Create(India, default);

            var result = await _controller.Update(1, new CountryRequest { Name = "Bharat", Code = "IN" }, default);

            Assert.Equal("Bharat", Assert.IsType<CountryResponse>(Assert.IsType<OkObjectResult>(result).Value).Name);
        }

        [Fact]
        public async Task Delete_ExistingThenMissing_ReturnsOkThenNotFound()
        {
            await _controller.Create(India, default);

            Assert.IsType<OkObjectResult>(await _controller.Delete(1, default));
            Assert.IsType<NotFoundObjectResult>(await _controller.Delete(1, default));
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithFirstPageOf5()
        {
            for (var i = 0; i < 7; i++)
                await _controller.Create(new CountryRequest { Name = $"Country {(char)('A' + i)}", Code = $"C{(char)('A' + i)}" }, default);

            var ok = Assert.IsType<OkObjectResult>(await _controller.GetAll(new CountryQuery(), default));
            var page = Assert.IsType<PagedResult<CountryResponse>>(ok.Value);

            Assert.Equal(5, page.Items.Count);
            Assert.Equal(7, page.TotalCount);
            Assert.Equal(2, page.TotalPages);
        }
    }
}
