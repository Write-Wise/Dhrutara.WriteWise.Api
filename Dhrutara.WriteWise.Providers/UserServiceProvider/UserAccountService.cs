using Microsoft.Graph;
using Microsoft.Graph.Users.Item;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Dhrutara.WriteWise.Providers.UserServiceProvider
{
    public class UserAccountService : IUserAccountService
    {
        private readonly GraphServiceClient _graphServiceClient;

        public UserAccountService(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
        }

        public async Task DeleteUserAsync(UserAccount? userAccount, CancellationToken cancellationToken)
        {
            await DeleteUserAccount(userAccount, cancellationToken).ConfigureAwait(false);
        }

        public UserAccount? GetUserAccount(string? authToken)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            if (tokenHandler.CanReadToken(authToken ?? string.Empty))
            {
                JwtSecurityToken token = tokenHandler.ReadJwtToken(authToken);
                string? userId = GetClaimValueFromToken(token, "sub");
                string? userGivenName = GetClaimValueFromToken(token, "given_name");
                string? userFamilyName = GetClaimValueFromToken(token, "family_name");
                string? idp = GetClaimValueFromToken(token, "idp");
                string? emailsValue = GetClaimValueFromToken(token, "emails");
                string[]? emails = emailsValue?.Split(',');

                if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(userGivenName) && !string.IsNullOrWhiteSpace(userFamilyName) && emails?.Any() == true && !string.IsNullOrWhiteSpace(idp))
                {
                    return new UserAccount(userId, userGivenName, userFamilyName, emails, idp);
                }
            }

            return null;
        }

        private async Task DeleteUserAccount(UserAccount? userAccount, CancellationToken cancellationToken)
        {
            if(userAccount == null)
            {
                throw new UserNotFoundException();
            }
            try
            {
                UserItemRequestBuilder userToDelete = _graphServiceClient.Users[userAccount.UserId] ?? throw new UserNotFoundException();
                await userToDelete.DeleteAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            }catch(Exception ex)
            {
                throw new UserDeletionException(ex);
            }
        }

        private static string? GetClaimValueFromToken(JwtSecurityToken token, string claimKey)
        {
            Claim? idClaim = token?.Claims?.FirstOrDefault(c => c.Type.Equals(claimKey, StringComparison.OrdinalIgnoreCase));

            return idClaim?.Value;
        }

        #region will remove later
        //private async Task<string?> GetAccessToken()
        //{
        //    string tenantId = _configurationProvider.AuthTenantId;
        //    string clientId = _configurationProvider.AuthClientId;
        //    string clientSecret = _configurationProvider.AuthClientSecret;

        //    HttpContent reqBody = new StringContent($"client_id={clientId}&client_secret={clientSecret}&grant_type={Constants.Constants.GRANT_TYPE}&scope={Constants.Constants.SCOPE}", System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

        //    HttpClient httpClient = new()
        //    {
        //        BaseAddress = new Uri("https://login.microsoftonline.com/")
        //    };

        //    HttpResponseMessage response = await httpClient.PostAsync($"{tenantId}/oauth2/v2.0/token", reqBody);

        //    string resBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        //    dynamic dynaBody = JsonConvert.DeserializeObject<dynamic>(resBody);

        //    return dynaBody?.access_token;
        //}

        //private async Task<string> GetUserAsync(string accessToken, string userId)
        //{
        //    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        //    HttpResponseMessage response = await _httpClient.GetAsync($"users/{userId}").ConfigureAwait(false);

        //    string resBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        //    return resBody;

        //}

        //private async Task<bool> DeleteUserAsync(string accessToken, string userId)
        //{
        //    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        //    HttpResponseMessage response = await _httpClient.DeleteAsync($"users/{userId}").ConfigureAwait(false);

        //    return response.IsSuccessStatusCode;
        //}

        //private static string GetUserId(HttpRequest req)
        //{
        //    if (req.HttpContext.Request.Headers.ContainsKey("Authorization"))
        //    {
        //        string authHeader = req.HttpContext.Request.Headers["Authorization"][0];
        //        if (authHeader?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
        //        {
        //            string token = authHeader["Bearer ".Length..];
        //            return GetClaimValueFromToken("sub", token);
        //        }
        //    }

        //    return null;
        //}

        #endregion
    }
}
