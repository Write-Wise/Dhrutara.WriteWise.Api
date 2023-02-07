using System.Text;
using System.Text.Json;

namespace Dhrutara.WriteWise.Providers.ContentProvider.OpenAI
{
    public class OpenAIContentProvider : IContentProvider
    {
        private readonly HttpClient _httpClient;


        public OpenAIContentProvider(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<ContentResponse> GetContentAsync(ContentRequest request, CancellationToken cancellationToken)
        {
            string prompt = $"A {request.Category} {request.Type}"
                + (request.To.HasValue ? $" to a {request.To}" : string.Empty)
                + (request.From.HasValue ? $" from a {request.From}" : string.Empty);

            string? content = await GetContentAsync(prompt, request.MaxContentLength, cancellationToken).ConfigureAwait(false);

            return !string.IsNullOrWhiteSpace(content)
                ? new ContentResponse(content) :
                new ContentResponse(false);
        }

        private async Task<string> GetContentAsync(string prompt, int maxContentLength, CancellationToken cancellationToken)
        {
            OpenAIRequest apiRequest = new()
            {
                Frequency_Penalty = 0,
                Max_Tokens = maxContentLength / 4,
                Model = "text-davinci-003",
                Presense_Penalty = 0.6f,
                Prompt = prompt,
                Stop = new List<string> { "Human:", "AI:" },
                Temperature = 0.9f,
                Top_P = 1
            };
            OpenAIResponse? response = await GetContentFromOpenAIAsync(apiRequest, cancellationToken).ConfigureAwait(false);
            string? cleanContent = CleanUpContent(response?.Choices?.FirstOrDefault()?.Text);
            return cleanContent;
        }

        private async Task<OpenAIResponse?> GetContentFromOpenAIAsync(OpenAIRequest request, CancellationToken cancellationToken)
        {
            string stringContent = @"{
  ""model"": ""text-davinci-003"",
  ""prompt"": """ + request.Prompt + @""",
  ""temperature"": 0.9,
  ""max_tokens"": " + request.Max_Tokens + @",
  ""top_p"": 1,
  ""frequency_penalty"": 0.0,
  ""presence_penalty"": 0.6,
  ""stop"": ["" Human:"", "" AI:""]
}";

            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = new LowerCaseNamingPolicy(),
                WriteIndented = true,
            };

            //string jsonContent = JsonSerializer.Serialize<Request>(apiRequest, options);
            HttpContent content = new StringContent(stringContent, Encoding.UTF8, "application/json");
            //HttpResponseMessage apiResponse = await _httpClient.PostAsync("v1/completions", apiRequest, options: options, cancellationToken: cancellationToken).ConfigureAwait(false);
            HttpResponseMessage apiResponse = await _httpClient.PostAsync("v1/completions", content, cancellationToken).ConfigureAwait(false);

            if (apiResponse.IsSuccessStatusCode)
            {
                Stream resultContent = await apiResponse.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                return await JsonSerializer.DeserializeAsync<OpenAIResponse>(resultContent, options, cancellationToken).ConfigureAwait(false);
            }

            return null;
        }

        private static string CleanUpContent(string? content)
        {
            string? result = content;
            if (!string.IsNullOrWhiteSpace(result))
            {
                if (!result.StartsWith("\n\n"))
                {
                    int index = result.IndexOf("\n\n");
                    if (index > -1)
                    {
                        result = result[index..];
                    }
                    else
                    {
                        result = string.Empty;
                    }
                }

                if (!result.EndsWith('.') && !result.EndsWith('!') && !result.EndsWith('?'))
                {
                    int lastSentenceFinisherindex = result.LastIndexOfAny(new[] { '.', '!', '?' });
                    if (lastSentenceFinisherindex > -1)
                    {
                        result = result[..(lastSentenceFinisherindex + 1)];
                    }
                    else
                    {
                        result = string.Empty;
                    }
                }
            }
            else
            {
                result = string.Empty;
            }

            return result;
        }

        private class LowerCaseNamingPolicy : JsonNamingPolicy
        {
            public override string ConvertName(string name) =>
                name.ToLower();
        }
    }
}
