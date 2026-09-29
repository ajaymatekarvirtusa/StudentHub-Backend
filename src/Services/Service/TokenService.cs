using Data.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.Service
{
    public class TokenService(IOptions<JwtSettings> _jwtOptions, ILogger<TokenService> _logger) : ITokenService
    {
        public const string AdminRole = "Admin";

        public TokenResponse GenerateToken(string username)
        {
            var settings = _jwtOptions.Value;
            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(settings.AccessTokenExpiryMinutes);

            // Every user gets the Admin role.
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, username),
                new(JwtRegisteredClaimNames.UniqueName, username),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new("role", AdminRole)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation("Access token issued. Username={Username} Role={Role} ExpiresAtUtc={ExpiresAtUtc}",
                username, AdminRole, expires);

            return new TokenResponse
            {
                AccessToken = accessToken,
                TokenType = "Bearer",
                ExpiresIn = (int)(expires - now).TotalSeconds,
                ExpiresAtUtc = expires,
                Username = username,
                Role = AdminRole
            };
        }
    }
}
