using Dhrutara.WriteWise.Providers;

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
        const string OPENAI_MODEL_CONFIG_KEY = "OpenAIModel";
        const string OPENAI_MAX_TOKEN_CONFIG_KEY = "OpenAIMaxTokens";
        
        const string AUTH_TENANT_ID_CONFIG_KEY = "AuthTenantId";
        const string AUTH_CLIENT_ID_CONFIG_KEY = "AuthClientId";
        const string AUTH_CLIENT_SECRET_CONFIG_KEY = "AuthClientSecret";

        private readonly Uri _cosmosEndpointUri;
        private readonly string _cosmosAuthKey;
        private readonly string _cosmosContentDatabase;
        private readonly string _cosmosContentContainer;
        private readonly string _cosmosDeletedUserContainer;

        private readonly Uri _openAIUri;
        private readonly string _openAIAuthKey;
        private readonly string _openAIModel;
        private readonly int _openAIMaxTokens;
        
        private readonly string _authTenantId;
        private readonly string _authClientId;
        private readonly string _authClientSecret;


        public ConfigurationProvider()
        {
            _cosmosEndpointUri = GetConfiguredUri(COSMOS_ENDPOINT_URI_CONFIG_KEY);
            _cosmosAuthKey = GetConfiguredString(COSMOS_AUTH_KEY_KEY);
            _cosmosContentDatabase = GetConfiguredString(COSMOS_CONTENT_DATABASE_CONFIG_KEY);
            _cosmosContentContainer = GetConfiguredString(COSMOS_CONTENT_CONTAINER_CONFIG_KEY);
            _cosmosDeletedUserContainer = GetConfiguredString(COSMOS_DELETE_USER_CONTAINER_CONFIG_KEY);

            _openAIUri = GetConfiguredUri(OPENAI_ENDPOINT_URI_CONFIG_KEY);
            _openAIAuthKey = GetConfiguredString(OPENAI_AUTH_CONFIG_KEY);
            _openAIModel = GetConfiguredString(OPENAI_MODEL_CONFIG_KEY);
            _openAIMaxTokens = GetConfiguredInt(OPENAI_MAX_TOKEN_CONFIG_KEY, 256);
            _openAIModel = GetConfiguredString(OPENAI_MODEL_CONFIG_KEY);

            _authTenantId = GetConfiguredString(AUTH_TENANT_ID_CONFIG_KEY);
            _authClientId = GetConfiguredString(AUTH_CLIENT_ID_CONFIG_KEY);
            _authClientSecret = GetConfiguredString(AUTH_CLIENT_SECRET_CONFIG_KEY);
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

        private static int GetConfiguredInt(string key,int defaultValue)
        {
            return int.TryParse(Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process), out int maxTokens)
               ? maxTokens
               : defaultValue;

        }

        public Uri CosmosEndpointUri => _cosmosEndpointUri;
        public string CosmosAuthKey => _cosmosAuthKey;
        public string CosmosDatabaseContent => _cosmosContentDatabase;
        public string CosmosContainerContent => _cosmosContentContainer;
        public string CosmosContainerDeletedUser => _cosmosDeletedUserContainer;

        public Uri OpenAIUri => _openAIUri;
        public string OpenAIAuthKey => _openAIAuthKey;
        public int OpenAIMaxTokens => _openAIMaxTokens;
        public string OpenAIModel => _openAIModel;

        public string AuthTenantId => _authTenantId;
        public string AuthClientId => _authClientId;
        public string AuthClientSecret => _authClientSecret;
    }
}
