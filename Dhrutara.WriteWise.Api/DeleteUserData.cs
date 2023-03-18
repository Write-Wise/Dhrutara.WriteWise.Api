using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Dhrutara.WriteWise.Providers.UserServiceProvider;
using Dhrutara.WriteWise.Providers.Storage;
using Dhrutara.WriteWise.Providers;

namespace Dhrutara.WriteWise.Api
{
    public class DeleteUserData
    {
        private readonly IUserAccountService _userAccountService;
        private readonly IStorageProvider _storageProvider;
        private readonly ILogger<DeleteUserData> _logger;

        public DeleteUserData(IUserAccountService userAccountService, IStorageProvider storageProvider, ILogger<DeleteUserData> logger)
        {
            _userAccountService = userAccountService;
            _storageProvider = storageProvider;
            _logger = logger;
        }
        [FunctionName("DeleteUserData")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.User, "delete", Route = null)] HttpRequest req, CancellationToken cancellationToken)
        {
            try
            {
                string? authToken = GetAuthToken(req) ?? throw new InValidAuthToken();

                UserAccount? userAccount = _userAccountService.GetUserAccount(authToken) ?? throw new InValidAuthToken();

                await _storageProvider.AddDeletedUserAsync(userAccount, cancellationToken).ConfigureAwait(false);
                try
                {
                    await _userAccountService.DeleteUserAsync(userAccount, cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    await _storageProvider.RemoveDeletedUserAsync(userAccount, cancellationToken).ConfigureAwait(false);
                    throw;
                }

                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new StatusCodeResult(500);
            }
        }

        private static string? GetAuthToken(HttpRequest request)
        {
            if (request?.HttpContext?.Request?.Headers?.ContainsKey("Authorization") == true)
            {
                string authHeader = request.HttpContext.Request.Headers["Authorization"][0];
                if (authHeader?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
                {
                    return authHeader["Bearer ".Length..];
                }
            }

            return null;
        }
    }
}
