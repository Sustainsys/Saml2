using System;

namespace Kentor.AuthServices.WebSso
{
    internal class Saml2ArtifactBinding : Saml2Binding
    {
        protected internal override bool CanUnbind(HttpRequestData request)
        {
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return false;
        }
    }
}