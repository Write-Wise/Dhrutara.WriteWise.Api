namespace Dhrutara.WriteWise.Providers.UserServiceProvider
{
    public interface IUserAccountService
    {
        UserAccount? GetUserAccount(string? authToken);
        Task DeleteUserAsync(UserAccount? userAccount, CancellationToken cancellationToken);
    }
}
