using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel;
using System.IdentityModel.Metadata;
using System.IdentityModel.Selectors;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Kentor.AuthServices.Metadata
{
    class ExtendedMetadataSerializer : MetadataSerializer
    {
        private ExtendedMetadataSerializer(SecurityTokenSerializer serializer)
            : base(serializer)
        { }

        private ExtendedMetadataSerializer() { }

        private static ExtendedMetadataSerializer readerInstance =
            new ExtendedMetadataSerializer(new KeyInfoSerializer());

        /// <summary>
        /// Use this instance for reading metadata. It uses custom extensions
        /// to increase feature support when reading metadata.
        /// </summary>
        public static ExtendedMetadataSerializer ReaderInstance
        {
            get
            {
                return readerInstance;
            }
        }

        private static ExtendedMetadataSerializer writerInstance =
            new ExtendedMetadataSerializer();

        public static ExtendedMetadataSerializer WriterInstance
        {
            get
            {
                return writerInstance;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Method is only called by base class no validation needed.")]
        protected override void WriteCustomAttributes<T>(XmlWriter writer, T source)
        {
            var cachedMetadata = source as ICachedMetadata;
            if (cachedMetadata != null && cachedMetadata.CacheDuration.HasValue)
            {
                writer.WriteAttributeString(
                    "cacheDuration",
                    XmlConvert.ToString(cachedMetadata.CacheDuration.Value));
            }

            var extendedEntityDescriptor = source as ExtendedEntityDescriptor;
            if (extendedEntityDescriptor != null)
            {
                writer.WriteAttributeString("xmlns", "saml2", null, Saml2Namespaces.Saml2Name);

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Method is only called by base class no validation needed.")]
        protected override void WriteCustomElements<T>(XmlWriter writer, T source)
        {
            // WriteCustomElement is called multiple times with the same SPSSODescriptor element
            // as source. But in one case it is called with the type T being SPSSODescriptor, and
            // that is the time we want to write the data.
            var extendedSPSSODescriptor = source as ExtendedServiceProviderSingleSignOnDescriptor;
            if (extendedSPSSODescriptor != null && typeof(T) == typeof(ServiceProviderSingleSignOnDescriptor))
            {
                int index = 0;
                foreach (var acs in extendedSPSSODescriptor.AttributeConsumingServices)
                {
                    writer.WriteStartElement("AttributeConsumingService", Saml2Namespaces.Saml2MetadataName);
                    writer.WriteAttributeString("index", index.ToString(CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("isDefault", XmlConvert.ToString(acs.IsDefault));
                    writer.WriteStartElement("ServiceName", Saml2Namespaces.Saml2MetadataName);
                    writer.WriteAttributeString("lang", XNamespace.Xml.NamespaceName, "en");
                    writer.WriteString(acs.ServiceName);
                    writer.WriteEndElement();
                    foreach (var ra in acs.RequestedAttributes)
                    {
                        WriteRequestedAttribute(writer, ra);
                    }
                    writer.WriteEndElement();
                    index++;
                }
            }
        }

        private static void WriteRequestedAttribute(XmlWriter writer, RequestedAttribute requestedAttribute)
        {
            writer.WriteStartElement("RequestedAttribute", Saml2Namespaces.Saml2MetadataName);
            writer.WriteAttributeString("Name", requestedAttribute.Name);
            writer.WriteAttributeString("isRequired", XmlConvert.ToString(requestedAttribute.IsRequired));
            if (requestedAttribute.NameFormat != null)
            {
                writer.WriteAttributeString("NameFormat", requestedAttribute.NameFormat.OriginalString);
            }
            if (!string.IsNullOrEmpty(requestedAttribute.FriendlyName))
            {
                writer.WriteAttributeString("FriendlyName", requestedAttribute.FriendlyName);
            }
            foreach (var av in requestedAttribute.Values)
            {
                writer.WriteStartElement("AttributeValue", Saml2Namespaces.Saml2Name);
                writer.WriteString(av);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        protected override EntityDescriptor CreateEntityDescriptorInstance()
        {
            return new ExtendedEntityDescriptor();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        protected override void ReadCustomAttributes<T>(XmlReader reader, T target)
        {
            var cachedMetadata = target as ICachedMetadata;
            if (cachedMetadata != null)
            {
                var validUntilString = reader.GetAttribute("validUntil");

                if (!string.IsNullOrEmpty(validUntilString))
                {
                    cachedMetadata.ValidUntil = XmlConvert.ToDateTime(
                        validUntilString,
                        XmlDateTimeSerializationMode.Utc);
                }

                var cacheDurationString = reader.GetAttribute("cacheDuration");

                if (!string.IsNullOrEmpty(cacheDurationString))
                {
                    cachedMetadata.CacheDuration =
                        XmlConvert.ToTimeSpan(cacheDurationString);
                }
            }
        }

        protected override EntitiesDescriptor CreateEntitiesDescriptorInstance()
        {
            return new ExtendedEntitiesDescriptor();
        }

        protected override Organization ReadOrganization(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentException("reader was null");

            var orgNames = new LocalizedEntryCollection<LocalizedName>();
            var orgDisplayNames = new LocalizedEntryCollection<LocalizedName>();
            var orgUrls = new LocalizedEntryCollection<LocalizedUri>();
            var isSafeForMetadataSerializer = true;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    // TODO: is xml:lang opt?
                    var lang = reader.GetAttribute("xml:lang");

                    if (reader.Name == "OrganizationName" || reader.Name == "OrganizationDisplayName")
                    {
                        LocalizedEntryCollection<LocalizedName> collection = orgNames;
                        if (reader.Name == "OrganizationDisplayName")
                            collection = orgDisplayNames;

                        reader.Read(); // Move to value

                        if (!collection.Any(x => x.Language == CultureInfo.GetCultureInfo(lang)))
                        {
                            collection.Add(new LocalizedName
                            {
                                Name = reader.Value,
                                Language = CultureInfo.GetCultureInfo(lang)
                            });
                            continue;
                        }
                    }
                    if (reader.Name == "OrganizationURL")
                    {
                        reader.Read(); // Move to value

                        if (!orgUrls.Any(x => x.Language == CultureInfo.GetCultureInfo(lang)))
                        {
                            orgUrls.Add(new LocalizedUri()
                            {
                                Uri = new Uri(reader.Value),
                                Language = CultureInfo.GetCultureInfo(lang)
                            });
                            continue;
                        }
                    }
                    if (reader.Name != "Organization")
                        isSafeForMetadataSerializer = false;
                }
                if (reader.Name == "Organization" && reader.NodeType == XmlNodeType.EndElement)
                    break;
            }
            if (isSafeForMetadataSerializer)
            {
                return base.ReadOrganization(reader);
            }

            return new Organization(orgNames, orgDisplayNames, orgUrls);
        }
    }
}
