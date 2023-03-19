namespace Dhrutara.WriteWise.Providers.Exceptions
{
    public class UserDeletionException : ApplicationException
    {
        private const string MESSAGE = "User account deletion failed!";
        public UserDeletionException() : base(MESSAGE) { }
        public UserDeletionException(Exception innerException) : base(MESSAGE, innerException) { }
    }
}
