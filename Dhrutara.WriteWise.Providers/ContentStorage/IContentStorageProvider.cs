namespace Dhrutara.WriteWise.Providers.ContentStorage
{
    public interface IContentStorageProvider
    {
        Task<string?> AddContentAsync(Content content, CancellationToken cancellationToken);
    }
}