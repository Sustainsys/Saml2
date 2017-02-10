using Kentor.AuthServices.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Xml;

namespace Kentor.AuthServices.Metadata
{
    /// <summary>
    /// Extensions for Metadatabase.
    /// </summary>
    public static class MetadataBaseExtensions
    {
        /// <summary>
        /// Use a MetadataSerializer to create an XML string out of metadata.
        /// </summary>
        /// <param name="metadata">Metadata to serialize.</param>
        /// <param name="signingCertificate">Certificate to sign the metadata
        /// with. Supply null to not sign.</param>
        /// <param name="signingAlgorithm">Algorithm to use when signing.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static string ToXmlString(
            this MetadataBase metadata,
            X509Certificate2 signingCertificate,
            string signingAlgorithm)
        {
            var serializer = ExtendedMetadataSerializer.WriterInstance;

            var xmlDoc = new XmlDocument();
            using (var xmlWriter = xmlDoc.CreateNavigator().AppendChild())
            {
                serializer.WriteMetadata(xmlWriter, metadata);
            }

            if (signingCertificate != null)
            {
                xmlDoc.Sign(signingCertificate, true, signingAlgorithm);
            }

            return xmlDoc.OuterXml;
        }
    }
}