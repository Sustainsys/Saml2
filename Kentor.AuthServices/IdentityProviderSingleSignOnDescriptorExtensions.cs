using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Kentor.AuthServices
{
    static class IdentityProviderSingleSignOnDescriptorExtensions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IDPSSODescriptor")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "protocolSupportEnumeration")]
        public static void Load(this IdentityProviderSingleSignOnDescriptor idpSsoDescriptor, XElement xmlData)
        {
            if(idpSsoDescriptor == null)
            {
                throw new ArgumentNullException("idpSsoDescriptor");
            }
            
            if(xmlData == null)
            {
                throw new ArgumentNullException("xmlData");
            }

            var protocolSupportEnumeration = xmlData.Attribute("protocolSupportEnumeration");
            if (protocolSupportEnumeration == null)
            {
                throw new InvalidMetadataException("Missing protocolSupportEnumeration attribute in IDPSSODescriptor.");
            }

            if (protocolSupportEnumeration.Value != "urn:oasis:names:tc:SAML:2.0:protocol")
            {
                var msg = string.Format(CultureInfo.InvariantCulture, "Invalid protocolSupportEnumeration \"{0}\".",
                    protocolSupportEnumeration.Value);
                throw new InvalidMetadataException(msg);
            }

            foreach(var ssoServiceXml in xmlData.Elements(Saml2Namespaces.Saml2Metadata + "SingleSignOnService"))
            {
                var endpoint = new ProtocolEndpoint();
                endpoint.Load(ssoServiceXml);
                idpSsoDescriptor.SingleSignOnServices.Add(endpoint);
            }
        }
    }
}
