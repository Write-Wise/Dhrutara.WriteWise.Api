using Dhrutara.WriteWise.Providers;
using Newtonsoft.Json;

namespace Dhrutara.WriteWise.Api.GetContent
{
    internal class ClientRequest
    {
        [JsonProperty(Required = Required.Always)]
        public ContentCategory Category { get; set; }

        [JsonProperty(Required = Required.Always)]
        public ContentType Type { get; set; }

        public Relation? From { get; set; }
        public Relation? To { get; set; }
    }
}
