namespace Dhrutara.WriteWise.Providers
{
    public interface IConfigurationProvider
    {
        Uri CosmosEndpointUri { get;}
        string CosmosAuthKey { get; }
        string CosmosContentDatabase { get; }
        string CosmosContentContainer { get; }
        Uri OpenAIUri { get; }
        string OpenAIAuthKey { get; }
    }
}
