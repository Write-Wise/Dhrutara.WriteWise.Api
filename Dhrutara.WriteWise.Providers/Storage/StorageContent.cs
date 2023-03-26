namespace Dhrutara.WriteWise.Providers.Storage
{
    internal class StorageContent
    {
        public StorageContent(string id, ContentCategory category, ContentType type, string text, string hashFunction, Relation receiver = Relation.None, Relation sender = Relation.None)
        {
            this.id = id;
            this.category = category.ToString();
            this.type = type.ToString();
            this.text = text;
            this.hashFunction = hashFunction;
            this.receiver = receiver.ToString();
            this.sender = sender.ToString();
            this.categoryPlusType = $"{this.category}~{this.type}";
        }

        public string id { get; set; }
        public string categoryPlusType { get; set; }
        public string category {get;set; }
        public string type { get; set; }
        public string text { get; set; }
        public string hashFunction { get; set; }
        public string receiver { get; set; }
        public string sender { get; set; }
    }
}
