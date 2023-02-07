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
        public Relation Relation { get; set; } = Relation.None;
        public string Text { get; set; }
    }
}
