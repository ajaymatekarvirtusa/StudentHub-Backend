using Data.Models;
using Services.Interface;

namespace StudentHub.Tests.Fakes
{
    public class FakeTokenService : ITokenService
    {
        public string? LastUsername { get; private set; }

        public TokenResponse GenerateToken(string username)
        {
            LastUsername = username;
            return new TokenResponse
            {
                AccessToken = "fake-token",
                TokenType = "Bearer",
                ExpiresIn = 180,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(3),
                Username = username,
                Role = "Admin"
            };
        }
    }
}
