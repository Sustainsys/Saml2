using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Xml.Linq;

namespace Kentor.AuthServices.IntegrationTests
{
    [TestClass]
    public class MetadataTests
    {
        [TestMethod]
        public void Metadata_GetMetadata_Saml2AuthenticationModule()
        {
            var client = new WebClient();
            var metadata = client.DownloadString("http://localhost:17009/SamplePath/Saml2AuthenticationModule/");
            var mimeType = client.ResponseHeaders["Content-Type"];

            XDocument.Parse(metadata).Root.Name.Should()
                .Be(XNamespace.Get("urn:oasis:names:tc:SAML:2.0:metadata") + "EntityDescriptor");
        }

        [TestMethod]
        public void Metadata_GetMetadata_Mvc()
        {
            Assert.Inconclusive();
        }
    }
}
