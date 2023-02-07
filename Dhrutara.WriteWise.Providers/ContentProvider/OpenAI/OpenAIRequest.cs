namespace Dhrutara.WriteWise.Providers.ContentProvider.OpenAI
{
    internal class OpenAIRequest
    {
        public string? Model { get; set; }
        public string? Prompt { get; set; }
        public float Temperature { get; set; }
        public int Max_Tokens { get; set; }
        public int Top_P { get; set; }
        public float Frequency_Penalty { get; set; }
        public float Presense_Penalty { get; set; }
        public List<string>? Stop { get; set; }
    }
}
