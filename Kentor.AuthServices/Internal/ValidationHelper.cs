using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Kentor.AuthServices.Internal
{
    /// <summary>
    /// Static methods that helps with the validation of Saml2 messages. 
    /// </summary>
    internal class ValidationHelper
    {
        /// <summary>
        /// The allowed transform algorithms in SAML2 messages.
        /// </summary>
        private static readonly string[] allowedTransforms = new string[]
        {
            SignedXml.XmlDsigEnvelopedSignatureTransformUrl,
            SignedXml.XmlDsigExcC14NTransformUrl,
            SignedXml.XmlDsigExcC14NWithCommentsTransformUrl
        };

        /// <summary>
        /// Ctor that needs an exception generator that helps throw the correct exceptions.
        /// </summary>
        /// <param name="exceptionGenerator">The exceptionGenerator will throw the correct exceptions.</param>
        public ValidationHelper(IExceptionGenerator exceptionGenerator)
        {
            exception = exceptionGenerator;
        }

        private IExceptionGenerator exception;
        
        ///<summary>Checks the signature.</summary>
        /// <param name="signedRootElement">The signed root element.</param>
        /// <param name="idpKey">The assymetric key of the algorithm.</param>
        public void CheckSignature(XmlElement signedRootElement, AsymmetricAlgorithm idpKey)
        {
            CheckSignature(signedRootElement, idpKey, null);
        }

        ///<summary>Checks the signature.</summary>
        /// <param name="signedRootElement">The signed root element.</param>
        /// <param name="idpKey">The assymetric key of the algorithm.</param>
        /// <param name="keyTransform">If the <c>idpKey</c> is null this function will allow you to fetch the key from the 
        /// signature or by other means.</param>
        public void CheckSignature(XmlElement signedRootElement, AsymmetricAlgorithm idpKey, Func<Signature, AsymmetricAlgorithm> keyTransform)
        {
            var xmlDocument = new XmlDocument { PreserveWhitespace = true };
            xmlDocument.LoadXml(signedRootElement.OuterXml);

            var signature = xmlDocument.DocumentElement["Signature", SignedXml.XmlDsigNamespaceUrl];
            if (signature == null)
            {
                throw exception.SignatureMissing();
            }

            var signedXml = new SignedXml(xmlDocument);
            signedXml.LoadXml(signature);

            if (keyTransform != null && idpKey == null)
            {
                idpKey = keyTransform(signedXml.Signature);
            }

            var signedRootElementId = "#" + signedRootElement.GetAttribute("ID");

            if (signedXml.SignedInfo.References.Count == 0)
            {
                throw exception.NoReferences();
            }

            if (signedXml.SignedInfo.References.Count != 1)
            {
                throw exception.MultipleReferences();
            }

            var reference = signedXml.SignedInfo.References.Cast<Reference>().Single();

            if (reference.Uri != signedRootElementId)
            {
                throw exception.IncorrectReference();
            }

            foreach (Transform transform in reference.TransformChain)
            {
                if (!allowedTransforms.Contains(transform.Algorithm))
                {
                    throw exception.DisallowedTransform(transform.Algorithm);
                }
            }

            if (!signedXml.CheckSignature(idpKey))
            {
                throw exception.SignatureValidationFail();
            }
        }
    }
}
