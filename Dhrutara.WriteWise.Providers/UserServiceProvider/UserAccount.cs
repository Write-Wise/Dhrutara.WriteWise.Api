namespace Dhrutara.WriteWise.Providers.UserServiceProvider
{
    public record UserAccount(string UserId, string GivenName, string FamilyName, string[] Emails, string IdentityProvider);
}
