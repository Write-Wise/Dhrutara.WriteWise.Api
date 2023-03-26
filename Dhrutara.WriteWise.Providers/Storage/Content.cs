namespace Dhrutara.WriteWise.Providers.Storage
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
        public Relation Receiver { get; set; } = Relation.None;
        public Relation Sender { get; set; } = Relation.None;
    }
}
