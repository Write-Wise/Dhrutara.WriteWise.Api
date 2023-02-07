using Microsoft.Azure.Cosmos;

namespace Dhrutara.WriteWise.Providers.ContentStorage.Cosmos
{
    public class CosmosContentStorageProvider: IContentStorageProvider
    {
        private readonly Container _container;

        private readonly Database _database;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IHashProvider _hashProvider;
        public CosmosContentStorageProvider(IConfigurationProvider configurationProvider, IHashProvider hashProvider, Database database)
        {
            _configurationProvider= configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));

            _container = _database.GetContainer(_configurationProvider.CosmosContentContainer) ?? throw new ApplicationException($"Couldn't find a content container.");
        }


        public async Task<string?> AddContentAsync(Content content, CancellationToken cancellationToken)
        {
            if (content != null && !string.IsNullOrWhiteSpace(content.Text))
            {
                string textHash = _hashProvider.ComputeSha256Hash(content.Text);
                StorageContent input = new(textHash, content.Category, content.Type, content.Text, "Sha256");
                await _container.Scripts
                    .ExecuteStoredProcedureAsync<StorageContent>("spCreateContent", new PartitionKey(input.categoryPlusType), new dynamic[] { input }, cancellationToken: cancellationToken).ConfigureAwait(false);

            }

            return null;
        }
    }
}