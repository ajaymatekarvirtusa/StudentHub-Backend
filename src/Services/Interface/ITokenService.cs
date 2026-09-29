using Data.Models;

namespace Services.Interface
{
    public interface ITokenService
    {
        TokenResponse GenerateToken(string username);
    }
}
