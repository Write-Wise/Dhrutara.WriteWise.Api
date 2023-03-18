using System.Net.Http.Headers;
using Azure.Identity;
using Dhrutara.WriteWise.Api;
using Dhrutara.WriteWise.Providers;
using Dhrutara.WriteWise.Providers.ContentProvider;
using Dhrutara.WriteWise.Providers.ContentProvider.OpenAI;
using Dhrutara.WriteWise.Providers.Storage;
using Dhrutara.WriteWise.Providers.Storage.Cosmos;
using Dhrutara.WriteWise.Providers.UserServiceProvider;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Polly;
using Polly.Extensions.Http;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Dhrutara.WriteWise.Api
{
    public class Startup : FunctionsStartup
    {
       
        public override void Configure(IFunctionsHostBuilder builder)
        {
            _ = builder.Services.AddSingleton<Providers.IConfigurationProvider, ConfigurationProvider>();

            Providers.IConfigurationProvider configProvider = builder.Services.BuildServiceProvider().GetService<Providers.IConfigurationProvider>();

            CosmosClient cosmosClient = new CosmosClientBuilder(configProvider.CosmosEndpointUri.AbsoluteUri, configProvider.CosmosAuthKey)
                .Build();

            TokenCredentialOptions options = new()
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            ClientSecretCredential clientSecretCredential = new(configProvider.AuthTenantId, configProvider.AuthClientId, configProvider.AuthClientSecret, options);

            GraphServiceClient graphClient = new(clientSecretCredential, new[] { "https://graph.microsoft.com/.default" });

            _ = builder.Services
                .AddSingleton(s => graphClient);


            _ = builder.Services
                .AddSingleton(s => cosmosClient)
                .AddSingleton(s => cosmosClient.GetDatabase(configProvider.CosmosDatabaseContent))
                .AddSingleton<IHashProvider, HashProvider>()
                .AddSingleton<IStorageProvider, CosmosStorageProvider>()
                .AddSingleton<IUserAccountService, UserAccountService>();


            _ = builder.Services
                .AddHttpClient<IContentProvider, OpenAIContentProvider>(client =>
                {
                    client.BaseAddress = configProvider.OpenAIUri;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configProvider.OpenAIAuthKey);
                })
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());


            _ = builder.Services
                .AddHttpClient<IUserAccountService, UserAccountService>(client =>
                {
                    client.BaseAddress = configProvider.MicrosoftGraphBaseUri;
                })
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }
}
