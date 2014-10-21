using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Kentor.AuthServices
{
    class ExtendedMetadataSerializer : MetadataSerializer
    {
        private static ExtendedMetadataSerializer instance = new ExtendedMetadataSerializer();
        public static ExtendedMetadataSerializer Instance
        {
            get
            {
                return instance;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification="Method is only called by base class no validation needed.")]
        protected override void WriteCustomAttributes<T>(XmlWriter writer, T source)
        {
            var extendedEntityDescriptor = source as ExtendedEntityDescriptor;
            if (extendedEntityDescriptor != null)
            {
                writer.WriteAttributeString(
                    "cacheDuration",
                    XmlConvert.ToString(extendedEntityDescriptor.CacheDuration));

                // This is really an element. But it must be placed first of the child elements
                // and WriteCustomAttributes is called at the right place for that.
                if (extendedEntityDescriptor.Extensions.DiscoveryResponse != null)
                {
                    writer.WriteStartElement("Extensions", Saml2Namespaces.Saml2MetadataName);
                    WriteIndexedProtocolEndpoint(
                        writer,
                        extendedEntityDescriptor.Extensions.DiscoveryResponse,
                        new XmlQualifiedName("DiscoveryResponse", Saml2Namespaces.Saml2IdpDiscoveryName));
                    writer.WriteEndElement();
                }
            }
        }
    }
}
