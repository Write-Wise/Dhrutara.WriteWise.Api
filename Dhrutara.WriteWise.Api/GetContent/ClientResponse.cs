namespace Dhrutara.WriteWise.Api.GetContent
{
    internal class ClientResponse
    {
        public string[] Content { get; set; }
        public bool IsContentValid { get; set; } = true;
    }
}
