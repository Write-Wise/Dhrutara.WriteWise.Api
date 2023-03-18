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
        const string AUTH_TENANT_ID_CONFIG_KEY = "AuthTenantId";
        const string AUTH_CLIENT_ID_CONFIG_KEY = "AuthClientId";
        const string AUTH_CLIENT_SECRET_CONFIG_KEY = "AuthClientSecret";
        const string MICROSOFT_GRAPH_BASE_URI_CONFIG_KEY = "MicrosoftGraphBaseUri";

        private readonly Uri _cosmosEndpointUri;
        private readonly string _cosmosAuthKey;
        private readonly string _cosmosContentDatabase;
        private readonly string _cosmosContentContainer;
        private readonly Uri _openAIUri;
        private readonly string _openAIAuthKey;
        private readonly string _authTenantId;
        private readonly string _authClientId;
        private readonly string _authClientSecret;
        private readonly Uri _microsoftGraphBaseUri;


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

            _authTenantId = Environment.GetEnvironmentVariable(AUTH_TENANT_ID_CONFIG_KEY, EnvironmentVariableTarget.Process);
            if (string.IsNullOrWhiteSpace(_authTenantId))
            {
                throw new InValidConfigurationException(AUTH_TENANT_ID_CONFIG_KEY);
            }

            _authClientId = Environment.GetEnvironmentVariable(AUTH_CLIENT_ID_CONFIG_KEY, EnvironmentVariableTarget.Process);
            if (string.IsNullOrWhiteSpace(_authClientId))
            {
                throw new InValidConfigurationException(AUTH_CLIENT_ID_CONFIG_KEY);
            }

            _authClientSecret = Environment.GetEnvironmentVariable(AUTH_CLIENT_SECRET_CONFIG_KEY, EnvironmentVariableTarget.Process);
            if (string.IsNullOrWhiteSpace(_authClientSecret))
            {
                throw new InValidConfigurationException(AUTH_CLIENT_SECRET_CONFIG_KEY);
            }

            _microsoftGraphBaseUri = Uri.TryCreate(Environment.GetEnvironmentVariable(MICROSOFT_GRAPH_BASE_URI_CONFIG_KEY, EnvironmentVariableTarget.Process), UriKind.Absolute, out Uri urimicrosoftGraph)
               ? urimicrosoftGraph
               : throw new InValidConfigurationException(MICROSOFT_GRAPH_BASE_URI_CONFIG_KEY);
        }

        public Uri CosmosEndpointUri => _cosmosEndpointUri;

        public string CosmosAuthKey => _cosmosAuthKey;

        public string CosmosContentDatabase => _cosmosContentDatabase;

        public string CosmosContentContainer => _cosmosContentContainer;

        public Uri OpenAIUri => _openAIUri;

        public string OpenAIAuthKey => _openAIAuthKey;

        public string AuthTenantId => _authTenantId;

        public string AuthClientId => _authClientId;

        public string AuthClientSecret => _authClientSecret;

        public Uri MicrosoftGraphBaseUri => _microsoftGraphBaseUri;
    }
}
