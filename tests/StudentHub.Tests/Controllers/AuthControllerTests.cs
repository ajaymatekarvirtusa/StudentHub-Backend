using API.Controllers;
using API.Models;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using StudentHub.Tests.Fakes;

namespace StudentHub.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly FakeTokenService _tokenService = new();
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _controller = new AuthController(_tokenService, NullLogger<AuthController>.Instance);
        }

        [Fact]
        public void Token_WhenBodyIsNull_ReturnsBadRequest_AndDoesNotIssueToken()
        {
            var result = _controller.Token(null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username and password are required.", Assert.IsType<ErrorResponse>(badRequest.Value).Error);
            Assert.Null(_tokenService.LastUsername);
        }

        [Fact]
        public void Token_WhenValid_ReturnsOkWithToken_ForThatUser()
        {
            var result = _controller.Token(new LoginRequest { Username = "ajay", Password = "secret" });

            var ok = Assert.IsType<OkObjectResult>(result);
            var token = Assert.IsType<TokenResponse>(ok.Value);
            Assert.Equal("ajay", token.Username);
            Assert.Equal("Admin", token.Role);
            Assert.Equal("ajay", _tokenService.LastUsername);
        }
    }
}
