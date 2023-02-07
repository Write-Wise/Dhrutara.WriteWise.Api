namespace Dhrutara.WriteWise.Providers.ContentStorage
{
    public class Content
    {
        public Content(ContentCategory category, ContentType type, string text)
        {
            Category = category;
            Type = type;
            Text = text;
        }

        public ContentCategory Category { get; set; }
        public ContentType Type { get; set; }
        public string Text { get; set; }
    }
}
