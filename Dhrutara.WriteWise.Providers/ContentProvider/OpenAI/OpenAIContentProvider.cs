using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

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
            string prompt = GeneratePrompt(request);

            string[] content = await GetContentAsync(prompt, request.MaxContentLength, cancellationToken).ConfigureAwait(false);

            return content.Any(s => !string.IsNullOrWhiteSpace(s) == true)
                ? new ContentResponse(content) :
                new ContentResponse(false);
        }

        private async Task<string[]> GetContentAsync(string prompt, int maxContentLength, CancellationToken cancellationToken)
        {
            OpenAIRequest apiRequest = new()
            {
                Frequency_Penalty = 0,
                Max_Tokens = maxContentLength / 3,
                Model = "text-davinci-003",
                Presense_Penalty = 0.6f,
                Prompt = prompt,
                Stop = new List<string> { "Human:", "AI:" },
                Temperature = 0.9f,
                Top_P = 1
            };
            OpenAIResponse? response = await GetContentFromOpenAIAsync(apiRequest, cancellationToken).ConfigureAwait(false);
            return CleanUpContent(response?.Choices?.FirstOrDefault()?.Text);
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

        private static string[] CleanUpContent(string? content)
        {
            string? tempResult = content;
            if (!string.IsNullOrWhiteSpace(tempResult))
            {
                if (!tempResult.StartsWith("\n\n"))
                {
                    int index = tempResult.IndexOf("\n\n");
                    if (index > -1)
                    {
                        tempResult = tempResult[index..];
                    }
                    else
                    {
                        tempResult = string.Empty;
                    }
                }

                if (!tempResult.EndsWith('.') && !tempResult.EndsWith('!') && !tempResult.EndsWith('?'))
                {
                    int lastSentenceFinisherindex = tempResult.LastIndexOfAny(new[] { '.', '!', '?' });
                    if (lastSentenceFinisherindex > -1)
                    {
                        tempResult = tempResult[..(lastSentenceFinisherindex + 1)];
                    }
                    else
                    {
                        tempResult = string.Empty;
                    }
                }

                tempResult = Regex.Replace(tempResult, @"(\d+)\.\ ", string.Empty);
                tempResult = Regex.Replace(tempResult, @"(\d+)\.", string.Empty);

                tempResult = tempResult.Replace("\n\n","\n");

                return tempResult.Split("\n").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            }
            else
            {
                return Array.Empty<string>();
            }
        }

        private static string GeneratePrompt(ContentRequest request)
        {
            StringBuilder prompt = new();
            if(request.From.HasValue && request.From != Relation.None)
            {
                _ = prompt.Append($"Assume you are a {request.From}. ");
            }

            

            switch (request.Type)
            {
                case ContentType.Joke:
                    _ = prompt.Append($"Tell me a {request.Category} {request.Type}");
                    break;
                case ContentType.Poem:
                case ContentType.Message:
                default:
                    _ = prompt.Append($"Now write a 4 line {request.Category} {ContentType.Poem}");
                    break;
            }

            Writer writer = Constants.GetARandomWriter(request.Type);
            _ = prompt.Append($" in the style of {writer.FirstName} {writer.LastName}");

            if (request.To.HasValue && request.To != Relation.None)
            {
                _ = prompt.Append($" to your {request.To}");
            }

            _ = prompt.Append('.');
            return prompt.ToString();
        }

        private class LowerCaseNamingPolicy : JsonNamingPolicy
        {
            public override string ConvertName(string name) =>
                name.ToLower();
        }
    }
}
