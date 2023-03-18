using Newtonsoft.Json;

namespace Dhrutara.WriteWise.Providers.UserServiceProvider
{
    public class UserAccountService : IUserAccountService
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly HttpClient _httpClient;


        public UserAccountService(IConfigurationProvider configurationProvider, HttpClient httpClient)
        {
            _configurationProvider = configurationProvider;
            _httpClient = httpClient;   
        }
        public async Task<Guid> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
        {

            string? accessToken = await GetAccessToken().ConfigureAwait(false);
            if(!string.IsNullOrWhiteSpace(accessToken))
            {
                _ = await DeleteUserAsync(accessToken, userId).ConfigureAwait(false);
            }

            return Guid.NewGuid();
        }

        private async Task<string?> GetAccessToken()
        {
            string tenantId = _configurationProvider.AuthTenantId;
            string clientId = _configurationProvider.AuthClientId;
            string clientSecret = _configurationProvider.AuthClientSecret;

            HttpContent reqBody = new StringContent($"client_id={clientId}&client_secret={clientSecret}&grant_type={Constants.Constants.GRANT_TYPE}&scope={Constants.Constants.SCOPE}", System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

            HttpClient httpClient = new()
            {
                BaseAddress = new Uri("https://login.microsoftonline.com/")
            };

            var response = await httpClient.PostAsync($"{tenantId}/oauth2/v2.0/token", reqBody);

            string resBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var dynaBody = JsonConvert.DeserializeObject<dynamic>(resBody);

            return dynaBody?.access_token;
        }

        private async Task<string> GetUserAsync(string accessToken, string userId)
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var response = await _httpClient.GetAsync($"users/{userId}").ConfigureAwait(false);

            string resBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return resBody;

        }

        private async Task<bool> DeleteUserAsync(string accessToken, string userId)
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var response = await _httpClient.DeleteAsync($"users/{userId}").ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }
    }
}
