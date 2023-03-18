using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Dhrutara.WriteWise.Providers.UserServiceProvider;
using System.Threading;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Dhrutara.WriteWise.Api
{
    public class DeleteUserData
    {
        private readonly IUserAccountService _userAccountService;

        public DeleteUserData(IUserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }
        [FunctionName("DeleteUserData")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.User, "delete", Route = null)] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            string userId = GetUserId(req);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new UnauthorizedResult();
            }

            try
            {
                await _userAccountService.DeleteUserAsync(userId, cancellationToken).ConfigureAwait(false);
                return new OkObjectResult(userId);
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                ObjectResult status500 = new("")
                {
                    StatusCode = 500
                };
                return status500;
            }
        }

        private static string GetUserId(HttpRequest req)
        {
            if (req.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                var authHeader = req.HttpContext.Request.Headers["Authorization"][0];
                if (authHeader?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
                {
                    var token = authHeader["Bearer ".Length..];
                    return GetClaimValueFromToken("sub", token);
                }
            }

            return null;
        }

        private static string GetClaimValueFromToken(string claimKey, string tokenString)
        {
            var jwtHandler = new JwtSecurityTokenHandler();

            if (jwtHandler.CanReadToken(tokenString))
            {
                var token = jwtHandler.ReadJwtToken(tokenString);

                Claim idClaim = token?.Claims?.FirstOrDefault(c => c.Type.Equals(claimKey, StringComparison.OrdinalIgnoreCase));

                return idClaim?.Value;
            }

            return null;
        }
    }
}
