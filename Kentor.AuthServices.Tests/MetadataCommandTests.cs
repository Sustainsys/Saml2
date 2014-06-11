using System;
using System.Web;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using FluentAssertions;
using System.IdentityModel.Metadata;
using System.Xml;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class MetadataCommandTests
    {
        [TestMethod]
        public void MetadataCommand_Run_SuccessfulResult()
        {
            var subject = new MetadataCommand().Run(Substitute.For<HttpRequestBase>());

            XDocument payloadXml = XDocument.Parse(subject.Content);

            var expectedXml = new XDocument(new XElement(Saml2Namespaces.Saml2Metadata + "EntityDescriptor",
                new XAttribute("entityId", "https://github.com/KentorIT/authservices"),
                new XElement(Saml2Namespaces.Saml2Metadata + "SPSSODescriptor",
                    new XElement(Saml2Namespaces.Saml2Metadata + "AssertionConsumerService",
                        new XAttribute("isDefault", true),
                        new XAttribute("index", 0),
                        new XAttribute("Binding", Saml2Binding.HttpPostUri),
                        new XAttribute("Location", "http://localhost/Saml2AuthenticationModule/acs")))));

            // Must parse expectedXml-string. 
            // See (http://stackoverflow.com/questions/24156689/xnode-deepequals-unexpectedly-returns-false/24156847#24156847)
            XNode.DeepEquals(payloadXml, XDocument.Parse(expectedXml.ToString())).Should().BeTrue();

            subject.ContentType.Should().Be("application/samlmetadata+xml");
        }
    }
}
