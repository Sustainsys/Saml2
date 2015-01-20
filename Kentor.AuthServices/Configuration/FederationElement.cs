using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Configuration of a federation.
    /// </summary>
    public class FederationElement : ConfigurationElement
    {
        private const string metadataUrl = "metadataUrl";
        /// <summary>
        /// Url to download metdata for the federation from.
        /// </summary>
        [ConfigurationProperty(metadataUrl, IsRequired = true)]
        public Uri MetadataUrl
        {
            get
            {
                return (Uri)base[metadataUrl];
            }
        }

        private const string allowUnsolicitedAuthnResponse = "allowUnsolicitedAuthnResponse";
        /// <summary>
        /// Are unsolicited responses from the idps in the federation allowed?
        /// </summary>
        [ConfigurationProperty(allowUnsolicitedAuthnResponse)]
        public bool AllowUnsolicitedAuthnResponse
        {
            get
            {
                return (bool)base[allowUnsolicitedAuthnResponse];
            }
        }

        private const string demandMetadataSignature = "metadataValidationMethod";
        /// <summary>
        /// Determines if we should demand that all metadata should be signed to use it.
        /// </summary>
        [ConfigurationProperty(demandMetadataSignature)]
        public SignatureValidationMethod MetadataValidationMethod
        {
            get
            {
                return (SignatureValidationMethod)base[demandMetadataSignature];
            }
        }


        private const string signingCertificate = "signingCertificate";
        /// <summary>
        /// Certificate location for the certificate the federation uses to sign its metadata.
        /// </summary>
        [ConfigurationProperty(signingCertificate)]
        public CertificateElement SigningCertificate
        {
            get
            {
                return (CertificateElement)base[signingCertificate];
            }
            internal set
            {
                base[signingCertificate] = value;
            }
        }
    }
}
