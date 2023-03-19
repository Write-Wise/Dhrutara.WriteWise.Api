namespace Dhrutara.WriteWise.Providers.Exceptions
{
    public class InValidAuthToken : ApplicationException
    {
        private const string MESSAGE = "Authentication token is not valid!";
        public InValidAuthToken() : base(MESSAGE) { }
        public InValidAuthToken(Exception innerException) : base(MESSAGE, innerException) { }
    }
}
