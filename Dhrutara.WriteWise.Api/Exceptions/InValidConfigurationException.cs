using System;

namespace Dhrutara.WriteWise.Api.Exceptions
{
    public class InValidConfigurationException : ApplicationException
    {
        
        public InValidConfigurationException(string configurationKey):base($"Configure a valid value for {configurationKey}")
        {
        }
    }
}
