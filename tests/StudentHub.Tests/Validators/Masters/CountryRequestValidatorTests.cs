using API.Validators.Masters;
using Data.Models.Masters;

namespace StudentHub.Tests.Validators.Masters
{
    public class CountryRequestValidatorTests
    {
        private readonly CountryRequestValidator _validator = new();

        private List<string> Errors(string name, string code) =>
            _validator.Validate(new CountryRequest { Name = name, Code = code }).Errors.Select(e => e.ErrorMessage).ToList();

        [Theory]
        [InlineData("India", "IN")]
        [InlineData("United States", "USA")]
        [InlineData("Côte d'Ivoire", "ci")]
        [InlineData("Congo (Kinshasa)", "COD")]
        public void Valid_HasNoErrors(string name, string code)
        {
            Assert.Empty(Errors(name, code));
        }

        [Fact]
        public void EmptyName_ShowsOnlyRequiredMessage()
        {
            var errors = Errors("", "IN");

            Assert.Single(errors);
            Assert.Contains("Country name is required.", errors);
        }

        [Theory]
        [InlineData("India123")]
        [InlineData("India@")]
        public void NameWithDigitsOrSymbols_IsInvalid(string name)
        {
            Assert.Contains("Country name can contain only letters, spaces and . ' - ( )", Errors(name, "IN"));
        }

        [Fact]
        public void NameOver100Characters_IsInvalid()
        {
            Assert.Contains("Country name must not exceed 100 characters.", Errors(new string('a', 101), "IN"));
        }

        [Theory]
        [InlineData("I")]
        [InlineData("INDI")]
        [InlineData("I1")]
        public void CodeNot2Or3Letters_IsInvalid(string code)
        {
            Assert.Contains("Country code must be 2 or 3 letters (ISO 3166), e.g. IN or IND.", Errors("India", code));
        }

        [Fact]
        public void EmptyCode_ShowsOnlyRequiredMessage()
        {
            var errors = Errors("India", "");

            Assert.Single(errors);
            Assert.Contains("Country code is required.", errors);
        }
    }
}
