using API.Validators;
using Data.Models;

namespace StudentHub.Tests.Validators
{
    public class LoginRequestValidatorTests
    {
        private readonly LoginRequestValidator _validator = new();

        [Fact]
        public void ValidRequest_HasNoErrors()
        {
            Assert.True(_validator.Validate(new LoginRequest { Username = "ajay", Password = "secret" }).IsValid);
        }

        [Fact]
        public void EmptyUsernameAndPassword_ReturnsBothErrors()
        {
            var errors = _validator.Validate(new LoginRequest()).Errors.Select(e => e.ErrorMessage).ToList();

            Assert.Contains("Username is required.", errors);
            Assert.Contains("Password is required.", errors);
        }

        [Fact]
        public void UsernameOver100Characters_IsInvalid()
        {
            var result = _validator.Validate(new LoginRequest { Username = new string('u', 101), Password = "x" });

            Assert.Contains(result.Errors, e => e.ErrorMessage == "Username must not exceed 100 characters.");
        }
    }
}
