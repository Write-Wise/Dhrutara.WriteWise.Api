using Dhrutara.WriteWise.Api.Exceptions;
using Dhrutara.WriteWise.Providers;
using System;

namespace Dhrutara.WriteWise.Api
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        const string COSMOS_ENDPOINT_URI_CONFIG_KEY = "CosmosEndpointUri";
        const string COSMOS_AUTH_KEY_KEY = "CosmosAuthKey";
        const string COSMOS_CONTENT_DATABASE_CONFIG_KEY = "ContentDatabase";
        const string COSMOS_CONTENT_CONTAINER_CONFIG_KEY = "ContentContainer";
        const string OPENAI_ENDPOINT_URI_CONFIG_KEY = "OpenAIUri";
        const string OPENAI_AUTH_CONFIG_KEY = "OpenAIAPIKey";

        private readonly Uri _cosmosEndpointUri;
        private readonly string _cosmosAuthKey;
        private readonly string _cosmosContentDatabase;
        private readonly string _cosmosContentContainer;
        private readonly Uri _openAIUri;
        private readonly string _openAIAuthKey;

        public ConfigurationProvider()
        {
            _cosmosEndpointUri = Uri.TryCreate(Environment.GetEnvironmentVariable(COSMOS_ENDPOINT_URI_CONFIG_KEY, EnvironmentVariableTarget.Process), UriKind.Absolute, out Uri uriCosmos)
                ? uriCosmos
                : throw new InValidConfigurationException(COSMOS_ENDPOINT_URI_CONFIG_KEY);

            _cosmosAuthKey = Environment.GetEnvironmentVariable(COSMOS_AUTH_KEY_KEY, EnvironmentVariableTarget.Process);
            if (string.IsNullOrWhiteSpace(_cosmosAuthKey))
            {
                throw new InValidConfigurationException(COSMOS_AUTH_KEY_KEY);
            }

            _cosmosContentDatabase = Environment.GetEnvironmentVariable(COSMOS_CONTENT_DATABASE_CONFIG_KEY, EnvironmentVariableTarget.Process);
            if (string.IsNullOrWhiteSpace(_cosmosContentDatabase))
            {
                throw new InValidConfigurationException(COSMOS_CONTENT_DATABASE_CONFIG_KEY);
            }

            _cosmosContentContainer = Environment.GetEnvironmentVariable(COSMOS_CONTENT_CONTAINER_CONFIG_KEY, EnvironmentVariableTarget.Process);
            if (string.IsNullOrWhiteSpace(_cosmosContentContainer))
            {
                throw new InValidConfigurationException(COSMOS_CONTENT_CONTAINER_CONFIG_KEY);
            }

            _openAIUri = Uri.TryCreate(Environment.GetEnvironmentVariable(OPENAI_ENDPOINT_URI_CONFIG_KEY, EnvironmentVariableTarget.Process), UriKind.Absolute, out Uri uriOpenAI)
                ? uriOpenAI
                : throw new InValidConfigurationException(OPENAI_ENDPOINT_URI_CONFIG_KEY);

            _openAIAuthKey = Environment.GetEnvironmentVariable(OPENAI_AUTH_CONFIG_KEY, EnvironmentVariableTarget.Process);
            if (string.IsNullOrWhiteSpace(_openAIAuthKey))
            {
                throw new InValidConfigurationException(OPENAI_AUTH_CONFIG_KEY);
            }
        }

        public Uri CosmosEndpointUri => _cosmosEndpointUri;

        public string CosmosAuthKey => _cosmosAuthKey;

        public string CosmosContentDatabase => _cosmosContentDatabase;

        public string CosmosContentContainer => _cosmosContentContainer;

        public Uri OpenAIUri => _openAIUri;

        public string OpenAIAuthKey => _openAIAuthKey;
    }
}
