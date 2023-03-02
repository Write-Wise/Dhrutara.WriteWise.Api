namespace Dhrutara.WriteWise.Providers.ContentProvider
{
    public class ContentResponse
    {
        public ContentResponse(string[] content, bool isContentValid = true)
        {
            Content = content;
            IsContentValid = isContentValid;
        }

        public ContentResponse(bool isContentValid):this(Array.Empty<string>(), isContentValid)
        {

        }

        public string[] Content { get; set; }
        public bool IsContentValid { get; set; }

    }
}
