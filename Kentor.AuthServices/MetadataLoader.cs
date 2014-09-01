using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Helper for loading SAML2 metadata
    /// </summary>
    public static class MetadataLoader
    {
        /// <summary>
        /// Load and parse metadata.
        /// </summary>
        /// <param name="metadataUri">Uri to metadata</param>
        /// <returns>EntityDescriptor containing metadata</returns>
        public static EntityDescriptor Load(Uri metadataUri)
        {
            if (metadataUri == null)
            {
                throw new ArgumentNullException("metadataUri");
            }

            var metaDataXml = XDocument.Load(metadataUri.ToString());

            return LoadEntityDescriptor(metaDataXml.Root);
        }

        /// <summary>
        /// Parse xml into an EntityDescriptor.
        /// </summary>
        /// <param name="metadataXml"></param>
        /// <returns>EntityDescriptor with parsed data.</returns>
        public static EntityDescriptor LoadEntityDescriptor(XElement metadataXml)
        {
            if (metadataXml == null)
            {
                throw new ArgumentNullException("metadataXml");
            }

            ValidateEntityDescriptor(metadataXml);

            var entityDescriptor = new EntityDescriptor()
            {
                EntityId = new EntityId(metadataXml.Attribute("EntityID").Value)
            };

            LoadRoleDescriptors(metadataXml, entityDescriptor);

            return entityDescriptor;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "EntityID")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "EntityDescriptor")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "protocolSupportEnumeration")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "tc")]
        private static void ValidateEntityDescriptor(XElement metadataXml)
        {
            if (metadataXml.Name != Saml2Namespaces.Saml2Metadata + "EntityDescriptor")
            {
                var msg = string.Format(CultureInfo.InvariantCulture,
                    "Unexpected element \"{0}\", expected \"{{urn:oasis:names:tc:SAML:2.0:metadata}}EntityDescriptor\".",
                    metadataXml.Name);
                throw new InvalidMetadataException(msg);
            }

            if (metadataXml.Attribute("EntityID") == null)
            {
                throw new InvalidMetadataException("Missing EntityID in EntityDescriptor.");
            }
        }

        private static void LoadRoleDescriptors(XElement metadataXml, EntityDescriptor entityDescriptor)
        {
            foreach(var idpSsoDescriptorXml in metadataXml.Elements(Saml2Namespaces.Saml2Metadata + "IDPSSODescriptor"))
            {
                var idpSsoDescriptor = new IdentityProviderSingleSignOnDescriptor();
                idpSsoDescriptor.Load(idpSsoDescriptorXml);
                entityDescriptor.RoleDescriptors.Add(idpSsoDescriptor);
            }
        }
    }
}
