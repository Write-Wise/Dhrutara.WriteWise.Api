namespace Dhrutara.WriteWise.Providers.Storage
{
    internal class StorageContent
    {
        public StorageContent(string id, ContentCategory category, ContentType type, string text, string hashFunction, string relation)
        {
            this.id = id;
            this.category = category.ToString();
            this.type = type.ToString();
            this.text = text;
            this.hashFunction = hashFunction;
            this.relation = relation;
            this.categoryPlusType = $"{this.category}~{this.type}";

        }

        public string id { get; set; }
        public string categoryPlusType { get; set; }
        public string category {get;set; }
        public string type { get; set; }
        public string text { get; set; }
        public string hashFunction { get; set; }
        public string relation { get; set; }
    }
}
