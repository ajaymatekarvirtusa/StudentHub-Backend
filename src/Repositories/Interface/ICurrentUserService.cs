namespace Repositories.Interface
{
    /// <summary>Who is making the request; used to fill CreatedBy / ModifiedBy.</summary>
    public interface ICurrentUserService
    {
        string UserName { get; }
    }
}
