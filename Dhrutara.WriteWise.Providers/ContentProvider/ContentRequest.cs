
namespace Dhrutara.WriteWise.Providers.ContentProvider
{
    public  class ContentRequest
    {
        public ContentRequest(ContentCategory category, ContentType type, Relation? from = null, Relation? to = null)
        {
            Category = category;
            Type = type;
            From = from;
            To = to;
        }

        public ContentCategory Category { get; set; }
        public ContentType Type { get; set; }
        public Relation? From { get; set; }
        public Relation? To { get; set; }
        public int MaxContentLength { get; set; } = 200;
    }
}
