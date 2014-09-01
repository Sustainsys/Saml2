using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IdentityModel.Metadata;
using FluentAssertions;
using System.Xml.Linq;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class MetadataLoaderTests
    {
        [TestMethod]
        public void MetadataLoader_Load_IdpMetadata()
        {
            var entityId = "http://localhost:13428/idpmetadata";
            var expected = new EntityDescriptor(new EntityId("http://localhost:13428/idpmetadata"));

            var subject = MetadataLoader.Load(new Uri(entityId));

            subject.EntityId.Id.Should().Be(entityId);
        }

        [TestMethod]
        public void MetadataLoader_Load_Nullcheck()
        {
            Action a = () => MetadataLoader.Load(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("metadataUri");
        }

        private string unexpectedElementMessage = "Unexpected element \"{0}\", expected \"{{urn:oasis:names:tc:SAML:2.0:metadata}}EntityDescriptor\".";

        [TestMethod]
        public void MetadataLoader_ParseEntityDescriptor_ChecksElementName()
        {
            var metadata = new XElement(Saml2Namespaces.Saml2Metadata + "NotAnEntityDescriptor");

            Action a = () => MetadataLoader.ParseEntityDescriptor(metadata);

            var expectedMessage = string.Format(unexpectedElementMessage, metadata.Name);

            a.ShouldThrow<InvalidMetadataException>().And.Message.Should().Be(expectedMessage);
        }

        [TestMethod]
        public void MetadataLoader_ParseEntityDescriptor_ChecksElementNamespace()
        {
            var metadata = new XElement(Saml2Namespaces.Saml2P + "EntityDescriptor");

            Action a = () => MetadataLoader.ParseEntityDescriptor(metadata);

            var expectedMessage = string.Format(unexpectedElementMessage, metadata.Name);

            a.ShouldThrow<InvalidMetadataException>().And.Message.Should().Be(expectedMessage);
        }

        [TestMethod]
        public void MetadataLoader_ParseEntityDescriptor_NullCheck()
        {
            Action a = () => MetadataLoader.ParseEntityDescriptor(null);

            a.ShouldThrow<ArgumentNullException>("metadataXml");
        }

        [TestMethod]
        public void MetadataLoader_ParseEntityDescriptor_EntityId()
        {
            var metadata = new XElement(Saml2Namespaces.Saml2Metadata + "EntityDescriptor",
                new XAttribute("EntityID", "SomeEntityID"));

            var subject = MetadataLoader.ParseEntityDescriptor(metadata);

            subject.EntityId.Id.Should().Be("SomeEntityID");
        }

        [TestMethod]
        public void MetadataLoader_ParseEntityDescriptor_MissingEntityId()
        {
            var metadata = new XElement(Saml2Namespaces.Saml2Metadata + "EntityDescriptor");

            Action a = () => MetadataLoader.ParseEntityDescriptor(metadata);

            a.ShouldThrow<InvalidMetadataException>().And.Message.Should().Be("Missing EntityID in EntityDescriptor.");
        }
    }
}
