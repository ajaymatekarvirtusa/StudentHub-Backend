using API.Validators.Masters;
using Data.Models.Masters;

namespace StudentHub.Tests.Validators.Masters
{
    public class CountryQueryValidatorTests
    {
        private readonly CountryQueryValidator _validator = new();

        [Fact]
        public void Defaults_AreValid_AndPageSizeIs5()
        {
            var query = new CountryQuery();

            Assert.True(_validator.Validate(query).IsValid);
            Assert.Equal(5, query.PageSize);
            Assert.Equal(1, query.PageNumber);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void PageNumberBelow1_IsInvalid(int pageNumber)
        {
            var errors = _validator.Validate(new CountryQuery { PageNumber = pageNumber }).Errors.Select(e => e.ErrorMessage);

            Assert.Contains("Page number must be 1 or more.", errors);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        public void PageSizeOutOfRange_IsInvalid(int pageSize)
        {
            var errors = _validator.Validate(new CountryQuery { PageSize = pageSize }).Errors.Select(e => e.ErrorMessage);

            Assert.Contains("Page size must be between 1 and 100.", errors);
        }

        [Fact]
        public void SearchOver100Characters_IsInvalid()
        {
            Assert.False(_validator.Validate(new CountryQuery { Search = new string('a', 101) }).IsValid);
        }
    }
}
