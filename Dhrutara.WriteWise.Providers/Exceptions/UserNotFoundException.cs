namespace Dhrutara.WriteWise.Providers.Exceptions
{
    public class UserNotFoundException : ApplicationException
    {
        private const string MESSAGE = "User account not found!";
        public UserNotFoundException() : base(MESSAGE) { }
        public UserNotFoundException(Exception innerException) : base(MESSAGE, innerException) { }
    }
}
