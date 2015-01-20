using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Internal
{
    internal class Saml2MetadataExceptionGenerator : IExceptionGenerator
    {
        public Exception SignatureMissing()
        {
            return new MetadataFailedValidationException("The SAML Metadata is not signed. Response cannot be trusted.");
        }

        public Exception NoReferences()
        {
            return new MetadataFailedValidationException("No reference found in Xml signature, it doesn't validate the Xml data.");
        }

        public Exception MultipleReferences()
        {
            return new MetadataFailedValidationException("Multiple references for Xml signatures are not allowed.");
        }

        public Exception IncorrectReference()
        {
            return new MetadataFailedValidationException("Incorrect reference on Xml signature. The reference must be to the root element of the element containing the signature."); ;
        }

        public Exception DisallowedTransform(string transformAlgorithm)
        {
            return new MetadataFailedValidationException(
                        "Transform \"" + transformAlgorithm + "\" found in Xml signature is not allowed in SAML.");
        }

        public Exception SignatureValidationFail()
        {
            return new MetadataFailedValidationException("Signature validation failed on SAML metadata.");
        }
    }
}
