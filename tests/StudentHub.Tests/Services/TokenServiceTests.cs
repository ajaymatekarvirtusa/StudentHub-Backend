using Data.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Services.Service;
using System.IdentityModel.Tokens.Jwt;

namespace StudentHub.Tests.Services
{
    public class TokenServiceTests
    {
        private static readonly JwtSettings Settings = new()
        {
            SecretKey = "unit-test-secret-key-that-is-32-bytes+",
            Issuer = "https://test-issuer",
            Audience = "https://test-audience",
            AccessTokenExpiryMinutes = 5
        };

        private readonly TokenService _service = new(Options.Create(Settings), NullLogger<TokenService>.Instance);

        [Fact]
        public void GenerateToken_ReturnsBearerTokenForUser_WithAdminRole()
        {
            var response = _service.GenerateToken("ajay");

            Assert.False(string.IsNullOrWhiteSpace(response.AccessToken));
            Assert.Equal("Bearer", response.TokenType);
            Assert.Equal("ajay", response.Username);
            Assert.Equal("Admin", response.Role);
        }

        [Fact]
        public void GenerateToken_UsesConfiguredExpiry()
        {
            var before = DateTime.UtcNow;

            var response = _service.GenerateToken("ajay");

            Assert.Equal(5 * 60, response.ExpiresIn);
            Assert.InRange(response.ExpiresAtUtc, before.AddMinutes(5).AddSeconds(-5), DateTime.UtcNow.AddMinutes(5).AddSeconds(5));
        }

        [Fact]
        public void GenerateToken_JwtContainsIssuerAudienceAndClaims()
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(_service.GenerateToken("ajay").AccessToken);

            Assert.Equal(Settings.Issuer, jwt.Issuer);
            Assert.Contains(Settings.Audience, jwt.Audiences);
            Assert.Equal("ajay", jwt.Subject);
            Assert.Contains(jwt.Claims, c => c.Type == "role" && c.Value == "Admin");
            Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.UniqueName && c.Value == "ajay");
            Assert.Equal("HS256", jwt.Header.Alg);
        }

        [Fact]
        public void GenerateToken_EachTokenHasUniqueId()
        {
            var handler = new JwtSecurityTokenHandler();

            var first = handler.ReadJwtToken(_service.GenerateToken("ajay").AccessToken).Id;
            var second = handler.ReadJwtToken(_service.GenerateToken("ajay").AccessToken).Id;

            Assert.NotEqual(first, second);
        }
    }
}
