using Microsoft.Win32;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Transactions;

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
                Max_Tokens = maxContentLength / 3,
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

                result = result.Replace("\n",string.Empty);

                result = Regex.Replace(result, @"(\d+)\.\ ", string.Empty);
                result = Regex.Replace(result, @"(\d+)\.", string.Empty);
            }
            else
            {
                result = string.Empty;
            }

            return result;
        }

        private static string GeneratePrompt(ContentRequest request)
        {
            StringBuilder prompt = new();
            if(request.From.HasValue && request.From != Relation.None)
            {
                prompt.Append($"Assume you are a {request.From}. ");
            }

            if(request.Type == ContentType.Message || request.Type == ContentType.Poem)
            {
                prompt.Append($"Now write a 4 line {request.Category} {request.Type}");
            }
            else
            {
                prompt.Append($"Tell a {request.Category} {request.Type}");
            }
            

            if(request.To.HasValue && request.To != Relation.None)
            {
                prompt.Append($" to your {request.To}");
            }

            prompt.Append(".");
            return prompt.ToString();
        }

        private class LowerCaseNamingPolicy : JsonNamingPolicy
        {
            public override string ConvertName(string name) =>
                name.ToLower();
        }
    }
}
