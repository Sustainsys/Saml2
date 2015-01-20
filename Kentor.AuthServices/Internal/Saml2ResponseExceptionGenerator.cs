using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Internal
{
    internal class Saml2ResponseExceptionGenerator : IExceptionGenerator
    {
        public Exception SignatureMissing()
        {
            return new Saml2ResponseFailedValidationException("The SAML Response is not signed and contains unsigned Assertions. Response cannot be trusted.");
        }

        public Exception NoReferences()
        {
            return new Saml2ResponseFailedValidationException("No reference found in Xml signature, it doesn't validate the Xml data.");
        }

        public Exception MultipleReferences()
        {
            return new Saml2ResponseFailedValidationException("Multiple references for Xml signatures are not allowed.");
        }

        public Exception IncorrectReference()
        {
            return new Saml2ResponseFailedValidationException("Incorrect reference on Xml signature. The reference must be to the root element of the element containing the signature.");
        }

        public Exception DisallowedTransform(string transformAlgorithm)
        {
            return new Saml2ResponseFailedValidationException(
                        "Transform \"" + transformAlgorithm + "\" found in Xml signature is not allowed in SAML.");
        }

        public Exception SignatureValidationFail()
        {
            return new Saml2ResponseFailedValidationException("Signature validation failed on SAML response or contained assertion.");
        }
    }
}
