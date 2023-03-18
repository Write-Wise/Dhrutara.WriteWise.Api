namespace Dhrutara.WriteWise.Providers
{
    public interface IConfigurationProvider
    {
        Uri CosmosEndpointUri { get;}
        string CosmosAuthKey { get; }
        string CosmosDatabaseContent { get; }
        string CosmosContainerContent { get; }
        string CosmosContainerDeletedUser { get; }
        Uri OpenAIUri { get; }
        string OpenAIAuthKey { get; }

        string AuthTenantId { get; }
        string AuthClientId { get; }
        string AuthClientSecret { get; }

        Uri MicrosoftGraphBaseUri { get; }
    }
}
