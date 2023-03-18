using Dhrutara.WriteWise.Providers.UserServiceProvider;

namespace Dhrutara.WriteWise.Providers.Storage
{
    public interface IStorageProvider
    {
        Task<string?> AddContentAsync(Content content, CancellationToken cancellationToken);
        Task AddDeletedUserAsync(UserAccount userAccount, CancellationToken cancellationToken);
        Task RemoveDeletedUserAsync(UserAccount userAccount, CancellationToken cancellationToken);
    }
}