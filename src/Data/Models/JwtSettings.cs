namespace Data.Models
{
    /// <summary>Bound from the "Jwt" section of appsettings.json.</summary>
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenExpiryMinutes { get; set; } = 15;
    }
}
