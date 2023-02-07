namespace Dhrutara.WriteWise.Providers.ContentProvider
{
    public interface IContentProvider
    {
        Task<ContentResponse> GetContentAsync(ContentRequest request, CancellationToken cancellationToken);
    }
}
