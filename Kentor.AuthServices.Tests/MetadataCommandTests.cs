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

            payloadXml.Root.Name.Should().Be(Saml2Namespaces.Saml2Metadata + "EntityDescriptor");
            payloadXml.Elements().Single().Attribute(XName.Get("entityId")).Value.Should().Be("https://github.com/KentorIT/authservices");

            throw new NotImplementedException("TODO: Check all attributes from app.config");
            throw new NotImplementedException("TODO: mime type according to spec");
        }
    }
}
