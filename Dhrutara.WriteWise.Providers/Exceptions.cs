namespace Dhrutara.WriteWise.Providers
{
    public class UserNotFoundException : Exception
    {
        private const string MESSAGE = "User account not found!";
        public UserNotFoundException():base(MESSAGE) { }
        public UserNotFoundException(Exception innerException) : base(MESSAGE, innerException) { }
    }
    public class UserDeletionException : Exception {
        private const string MESSAGE = "User account deletion failed!";
        public UserDeletionException():base(MESSAGE) { }
        public UserDeletionException(Exception innerException) : base(MESSAGE, innerException) { }
    }

    public class InValidAuthToken : Exception
    {
        private const string MESSAGE = "Authentication token is not valid!";
        public InValidAuthToken() : base(MESSAGE) { }
        public InValidAuthToken(Exception innerException) : base(MESSAGE, innerException) { }
    }
}
