using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Dhrutara.WriteWise.Providers.ContentProvider.OpenAI
{
    public class OpenAIContentProvider : IContentProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IConfigurationProvider _configurationProvider;


        public OpenAIContentProvider(HttpClient httpClient, IConfigurationProvider configurationProvider)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
        }

        public async Task<ContentResponse> GetContentAsync(ContentRequest request, CancellationToken cancellationToken)
        {
            string prompt = GeneratePrompt(request);

            string[] content = await GetContentAsync(prompt, cancellationToken).ConfigureAwait(false);

            return content.Any(s => !string.IsNullOrWhiteSpace(s) == true)
                ? new ContentResponse(content) :
                new ContentResponse(false);
        }

        private async Task<string[]> GetContentAsync(string prompt, CancellationToken cancellationToken)
        {
            OpenAIResponse? response = await GetContentFromOpenAIAsync(prompt, cancellationToken).ConfigureAwait(false);
            return CleanUpContent(response?.Choices?.FirstOrDefault()?.Text);
        }

        private async Task<OpenAIResponse?> GetContentFromOpenAIAsync(string prompt, CancellationToken cancellationToken)
        {
            string stringContent = @"{
  ""model"": """ + _configurationProvider.OpenAIModel + @""",
  ""prompt"": """ + prompt + @""",
  ""temperature"": 0.9,
  ""max_tokens"": " + _configurationProvider.OpenAIMaxTokens + @",
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

            HttpContent content = new StringContent(stringContent, Encoding.UTF8, "application/json");
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

            _ = request.Type switch
            {
                ContentType.Joke => prompt.Append($"Tell me a {request.Category} {request.Type}"),
                _ => prompt.Append($"Now write a small {request.Category} {ContentType.Poem}"),
            };
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
