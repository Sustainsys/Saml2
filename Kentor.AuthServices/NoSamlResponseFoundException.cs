using System;
using System.Runtime.Serialization;

namespace Kentor.AuthServices
{
    [Serializable]
    class NoSamlResponseFoundException : AuthServicesException
    {
        public NoSamlResponseFoundException()
            : this("No Saml2 Response found in the http request.")
        {
        }

        public NoSamlResponseFoundException(string message)
            : base(message)
        { }

        public NoSamlResponseFoundException(string message, Exception innerException)
            :base(message, innerException)
        { }

        protected NoSamlResponseFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            throw new NotImplementedException();
        }
    }
}
