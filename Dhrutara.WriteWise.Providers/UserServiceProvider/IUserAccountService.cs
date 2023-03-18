namespace Dhrutara.WriteWise.Providers.UserServiceProvider
{
    public interface IUserAccountService
    {
        Task<Guid> DeleteUserAsync(string userId, CancellationToken cancellationToken);
    }
}
