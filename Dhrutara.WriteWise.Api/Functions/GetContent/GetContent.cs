using System.Net;
using System.Security.Claims;
using Dhrutara.WriteWise.Providers.ContentProvider;
using Dhrutara.WriteWise.Providers.Storage;
using Dhrutara.WriteWise.Providers.ExtensionMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Dhrutara.WriteWise.Api.Functions.GetContent
{
    public class GetContent
    {
        private readonly IContentProvider _contentProvider;
        private readonly IStorageProvider _contentStorageProvider;
        private readonly ILogger<GetContent> _logger;

        public GetContent(IContentProvider contentProvider, IStorageProvider contentStorageProvider, ILogger<GetContent> logger)
        {
            _contentProvider = contentProvider;
            _contentStorageProvider = contentStorageProvider;
            _logger = logger;
        }

        [FunctionName("GetContent")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(GetContent)} triggered.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);

                try
                {
                    ClientRequest? request = JsonConvert.DeserializeObject<ClientRequest>(requestBody);
                    ClientResponse response = await GenerateResponseAsync(request, cancellationToken).ConfigureAwait(false);

                    StoreContentAsync(request, response, cancellationToken).SafeFireAndForget(false);

                    return new OkObjectResult(response);
                }
                catch (JsonSerializationException)
                {
                    return new BadRequestObjectResult("The request payload is not valid!");
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new ObjectResult("Unknow error please try again") { StatusCode = 500 };
            }
        }

        private async Task<ClientResponse> GenerateResponseAsync(ClientRequest? request, CancellationToken cancellationToken)
        {
            ClientResponse response = new();

            if (request != null)
            {
                ContentResponse msgResponse = await GetContentAsync(request, cancellationToken).ConfigureAwait(false);
                response.Content = msgResponse.Content;
                response.IsContentValid = msgResponse.IsContentValid;
            }
            else
            {
                response.Content = new string[] { "Pass the request payload in the HTTP request body." };
            }

            return response;
        }

        private async Task<ContentResponse> GetContentAsync(ClientRequest request, CancellationToken cancellationToken)
        {
            ContentRequest contentRequest = new(request.Category, request.Type, request.From, request.To);
            return await _contentProvider.GetContentAsync(contentRequest, cancellationToken).ConfigureAwait(false);
        }

        private async Task StoreContentAsync(ClientRequest? request, ClientResponse response, CancellationToken cancellationToken)
        {
            if (request != null && response.IsContentValid)
            {
                Content content = new(request.Category, request.Type, JsonConvert.SerializeObject(response.Content))
                {
                    Receiver = request.To ?? Relation.None,
                    Sender = request.From ?? Relation.None,
                };
                _ = await _contentStorageProvider.AddContentAsync(content, cancellationToken);
            }
        }

        private static async Task<string> GetTestContentAsync()
        {
            await Task.Yield();

            string content = "\n\nWith each passing day, I am more and more in awe of the strength of our love. In the moments when life is hard and I".Replace("\n", "").Replace("\r", "");
            return content;
        }


    }
}

