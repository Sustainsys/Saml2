using System;
using System.IdentityModel.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Xml.Linq;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class IndexedProtocolEndpointExtensionsTests
    {
        [TestMethod]
        public void IndexedProtocolEndpointExtensions_ToXElement_NullCheck_IndexedProtocolEndpoint()
        {
            IndexedProtocolEndpoint endpoint = null;

            Action a = () => endpoint.ToXElement(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("indexedProtocolEndpoint");
        }

        [TestMethod]
        public void IndexedProtocolEndpointExtensions_ToXElement_NullCheck_ElementName()
        {
            XName xName = null;
            var endpoint = new IndexedProtocolEndpoint();

            Action a = () => endpoint.ToXElement(xName);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("elementName");
        }

        [TestMethod]
        public void IndexedProtocolEndpointExtensions_ToXElement_BasicAttributes()
        {
            string sampleAcsUri = "https://some.uri.example.com/acs";

            var acs = new IndexedProtocolEndpoint()
            {
                IsDefault = false,
                Index = 17,
                Binding = Saml2Binding.HttpPostUri,
                Location = new Uri(sampleAcsUri)
            };

            var elementName = Saml2Namespaces.Saml2Metadata + "AssertionConsumerService";

            var subject = acs.ToXElement(elementName);

            subject.Name.Should().Be(elementName);
            subject.Attribute("isDefault").Value.Should().Be("false");
            subject.Attribute("index").Value.Should().Be("17");
            subject.Attribute("Binding").Value.Should().Be(Saml2Binding.HttpPostUri.ToString());
            subject.Attribute("Location").Value.Should().Be(sampleAcsUri);
        }
    }
}
