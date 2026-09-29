using Repositories.Interface;

namespace API.Services
{
    /// <summary>Reads the user name from the JWT (the "unique_name" claim).</summary>
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public string UserName => httpContextAccessor.HttpContext?.User.Identity?.Name ?? "system";
    }
}
