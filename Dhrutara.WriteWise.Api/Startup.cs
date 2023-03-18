using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Dhrutara.WriteWise.Api;
using Dhrutara.WriteWise.Providers;
using Dhrutara.WriteWise.Providers.ContentProvider;
using Dhrutara.WriteWise.Providers.ContentProvider.OpenAI;
using Dhrutara.WriteWise.Providers.ContentStorage;
using Dhrutara.WriteWise.Providers.ContentStorage.Cosmos;
using Dhrutara.WriteWise.Providers.UserServiceProvider;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Dhrutara.WriteWise.Api
{
    public class Startup : FunctionsStartup
    {
        //private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
        //    .SetBasePath(Environment.CurrentDirectory)
        //    .AddEnvironmentVariables()
        //    .Build();

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<Providers.IConfigurationProvider, ConfigurationProvider>();

            Providers.IConfigurationProvider configProvider = builder.Services.BuildServiceProvider().GetService<Providers.IConfigurationProvider>();

            CosmosClient cosmosClient = new CosmosClientBuilder(configProvider.CosmosEndpointUri.AbsoluteUri, configProvider.CosmosAuthKey)
                .Build();


            builder.Services
                .AddSingleton(s => cosmosClient)
                .AddSingleton(s => cosmosClient.GetDatabase(configProvider.CosmosContentDatabase))
                .AddSingleton<IHashProvider, HashProvider>()
                .AddSingleton<IContentStorageProvider, CosmosContentStorageProvider>();

            builder.Services
                .AddHttpClient<IContentProvider, OpenAIContentProvider>(client =>
                {
                    client.BaseAddress = configProvider.OpenAIUri;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configProvider.OpenAIAuthKey);
                })
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());

            builder.Services
                .AddHttpClient<IUserAccountService, UserAccountService>(client => {
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
