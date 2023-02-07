
namespace Dhrutara.WriteWise.Providers.ContentProvider.OpenAI
{
    internal class OpenAIResponse
    {
        public string? Id { get; set; }
        public string? Object { get; set; }
        public long Created { get; set; }
        public string? Model { get; set; }
        public List<Choice> Choices { get; set; } = new List<Choice>();
    }

    class Choice
    {
        public string? Text { get; set; }
        public long Index { get; set; }
        public string? LogProbs { get; set; }
        public string? Finish_Reason { get; set; }
    }
}
