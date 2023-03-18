using Dhrutara.WriteWise.Api.Exceptions;
using Dhrutara.WriteWise.Providers;
using Microsoft.Graph.DeviceManagement.Reports.GetConfigurationPolicyNonComplianceReport;

namespace Dhrutara.WriteWise.Api
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        const string COSMOS_ENDPOINT_URI_CONFIG_KEY = "CosmosEndpointUri";
        const string COSMOS_AUTH_KEY_KEY = "CosmosAuthKey";
        const string COSMOS_CONTENT_DATABASE_CONFIG_KEY = "ContentDatabase";
        const string COSMOS_CONTENT_CONTAINER_CONFIG_KEY = "ContentContainer";
        const string COSMOS_DELETE_USER_CONTAINER_CONFIG_KEY = "DeletedUserContainer";
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
        private readonly string _cosmosDeletedUserContainer;
        private readonly Uri _openAIUri;
        private readonly string _openAIAuthKey;
        private readonly string _authTenantId;
        private readonly string _authClientId;
        private readonly string _authClientSecret;
        private readonly Uri _microsoftGraphBaseUri;


        public ConfigurationProvider()
        {
            _cosmosEndpointUri = GetConfiguredUri(COSMOS_ENDPOINT_URI_CONFIG_KEY);

            _cosmosAuthKey = GetConfiguredString(COSMOS_AUTH_KEY_KEY);

            _cosmosContentDatabase = GetConfiguredString(COSMOS_CONTENT_DATABASE_CONFIG_KEY);

            _cosmosContentContainer = GetConfiguredString(COSMOS_CONTENT_CONTAINER_CONFIG_KEY);

            _cosmosDeletedUserContainer = GetConfiguredString(COSMOS_DELETE_USER_CONTAINER_CONFIG_KEY);

            _openAIUri = GetConfiguredUri(OPENAI_ENDPOINT_URI_CONFIG_KEY);

            _openAIAuthKey = GetConfiguredString(OPENAI_AUTH_CONFIG_KEY);

            _authTenantId = GetConfiguredString(AUTH_TENANT_ID_CONFIG_KEY);

            _authClientId = GetConfiguredString(AUTH_CLIENT_ID_CONFIG_KEY);

            _authClientSecret = GetConfiguredString(AUTH_CLIENT_SECRET_CONFIG_KEY);

            _microsoftGraphBaseUri = GetConfiguredUri(MICROSOFT_GRAPH_BASE_URI_CONFIG_KEY);
        }

        private static string GetConfiguredString(string key)
        {
            string val = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process)?? throw new InValidConfigurationException(key);
            if (string.IsNullOrWhiteSpace(val)){
                throw new InValidConfigurationException(key);
            }
            return val;
        }

        private static Uri GetConfiguredUri(string key)
        {
            return Uri.TryCreate(Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process), UriKind.Absolute, out Uri? uri)
               ? uri
               : throw new InValidConfigurationException(key);

        }

        public Uri CosmosEndpointUri => _cosmosEndpointUri;

        public string CosmosAuthKey => _cosmosAuthKey;

        public string CosmosDatabaseContent => _cosmosContentDatabase;

        public string CosmosContainerContent => _cosmosContentContainer;

        public string CosmosContainerDeletedUser => _cosmosDeletedUserContainer;

        public Uri OpenAIUri => _openAIUri;

        public string OpenAIAuthKey => _openAIAuthKey;

        public string AuthTenantId => _authTenantId;

        public string AuthClientId => _authClientId;

        public string AuthClientSecret => _authClientSecret;

        public Uri MicrosoftGraphBaseUri => _microsoftGraphBaseUri;
    }
}
