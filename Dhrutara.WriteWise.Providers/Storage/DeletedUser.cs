using Dhrutara.WriteWise.Providers.UserServiceProvider;

namespace Dhrutara.WriteWise.Providers.Storage
{
    public class DeletedUser
    {
        public DeletedUser() { }
        public DeletedUser(string id, string givenName, string familyName, string[] emails, string identityProvider)
        {
            this.id = id;
            this.givenName = givenName;
            this.familyName = familyName;
            this.emails = emails;
            this.identityProvider = identityProvider;
        }
        public DeletedUser(UserAccount userAccount) : this(userAccount.UserId, userAccount.GivenName, userAccount.FamilyName, userAccount.Emails, userAccount.IdentityProvider) { }

        public string? id { get; set; }
        public string? givenName { get; set; }
        public string? familyName { get; set; }
        public string[]? emails { get; set; }
        public string? identityProvider { get; set; }
    }
}
