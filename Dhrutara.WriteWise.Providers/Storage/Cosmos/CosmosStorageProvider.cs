using Dhrutara.WriteWise.Providers.UserServiceProvider;
using Microsoft.Azure.Cosmos;
using Container = Microsoft.Azure.Cosmos.Container;


namespace Dhrutara.WriteWise.Providers.Storage.Cosmos
{
    public class CosmosStorageProvider: IStorageProvider
    {
        private readonly Lazy<Container> _containerContent;
        private readonly Lazy<Container> _containerDeletedUser;

        private readonly Database _database;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IHashProvider _hashProvider;
        public CosmosStorageProvider(IConfigurationProvider configurationProvider, IHashProvider hashProvider, Database database)
        {
            _configurationProvider= configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));

            _containerContent = new Lazy<Container>(() => { 
                return _database.GetContainer(_configurationProvider.CosmosContainerContent) ?? throw new ApplicationException($"Couldn't find a Content container.");
            });

            _containerDeletedUser = new Lazy<Container>(() => {
                return _database.GetContainer(_configurationProvider.CosmosContainerDeletedUser) ?? throw new ApplicationException($"Couldn't find a DeletedUser container.");
            });

        }


        public async Task<string?> AddContentAsync(Content content, CancellationToken cancellationToken)
        {
            if (content != null && !string.IsNullOrWhiteSpace(content.Text))
            {
                string textHash = _hashProvider.ComputeSha256Hash(content.Text);
                StorageContent input = new(textHash, content.Category, content.Type, content.Text, "Sha256", content.Relation.ToString());
                _ = await _containerContent.Value.Scripts
                    .ExecuteStoredProcedureAsync<StorageContent>("spCreateContent", new PartitionKey(input.categoryPlusType), new dynamic[] { input }, cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            return null;
        }

        public async Task AddDeletedUserAsync(UserAccount userAccount, CancellationToken cancellationToken)
        {
            if(userAccount == null) { throw new ArgumentNullException(nameof(userAccount)); }

            DeletedUser deletedUser = new(userAccount);
            try
            {
                _ = await _containerDeletedUser.Value.CreateItemAsync(deletedUser, partitionKey: new PartitionKey(deletedUser.identityProvider), cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new UserDeletionException(ex);
            }
        }

        public async Task RemoveDeletedUserAsync(UserAccount userAccount, CancellationToken cancellationToken)
        {
            if (userAccount != null)
            {
                try
                {
                    _ = await _containerDeletedUser.Value.DeleteItemAsync<DeletedUser>(userAccount.UserId, partitionKey: new PartitionKey(userAccount.IdentityProvider), cancellationToken: cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new UserDeletionException(ex);
                }
            }
        }
    }
}