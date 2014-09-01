using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IdentityModel.Metadata;
using FluentAssertions;
using System.Xml.Linq;
using System.Linq;

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
        public void MetadataLoader_LoadEntityDescriptor_ChecksElementName()
        {
            var metadata = new XElement(Saml2Namespaces.Saml2Metadata + "NotAnEntityDescriptor");

            Action a = () => MetadataLoader.LoadEntityDescriptor(metadata);

            var expectedMessage = string.Format(unexpectedElementMessage, metadata.Name);

            a.ShouldThrow<InvalidMetadataException>().And.Message.Should().Be(expectedMessage);
        }

        [TestMethod]
        public void MetadataLoader_LoadEntityDescriptor_ChecksElementNamespace()
        {
            var metadata = new XElement(Saml2Namespaces.Saml2P + "EntityDescriptor");

            Action a = () => MetadataLoader.LoadEntityDescriptor(metadata);

            var expectedMessage = string.Format(unexpectedElementMessage, metadata.Name);

            a.ShouldThrow<InvalidMetadataException>().And.Message.Should().Be(expectedMessage);
        }

        [TestMethod]
        public void MetadataLoader_LoadEntityDescriptor_NullCheck()
        {
            Action a = () => MetadataLoader.LoadEntityDescriptor(null);

            a.ShouldThrow<ArgumentNullException>("metadataXml");
        }

        [TestMethod]
        public void MetadataLoader_LoadEntityDescriptor_EntityId()
        {
            var subject = MetadataLoader.LoadEntityDescriptor(CreateBasicMetadata());

            subject.EntityId.Id.Should().Be("SomeEntityID");
        }

        [TestMethod]
        public void MetadataLoader_LoadEntityDescriptor_MissingEntityId()
        {
            var metadata = new XElement(Saml2Namespaces.Saml2Metadata + "EntityDescriptor");

            Action a = () => MetadataLoader.LoadEntityDescriptor(metadata);

            a.ShouldThrow<InvalidMetadataException>().And.Message.Should().Be("Missing EntityID in EntityDescriptor.");
        }

        XElement CreateBasicMetadata()
        {
            return new XElement(Saml2Namespaces.Saml2Metadata + "EntityDescriptor",
                new XAttribute("EntityID", "SomeEntityID"),
                new XElement(Saml2Namespaces.Saml2Metadata + "IDPSSODescriptor",
                    new XAttribute("protocolSupportEnumeration", "urn:oasis:names:tc:SAML:2.0:protocol"),
                    new XElement(Saml2Namespaces.Saml2Metadata + "SingleSignOnService")));
        }

        [TestMethod]
        public void MetadataLoader_LoadEntityDescritor_MultipleIDPSSODescriptor()
        {
            var metadata = CreateBasicMetadata();

            metadata.Add(new XElement(Saml2Namespaces.Saml2Metadata + "IDPSSODescriptor",
                new XAttribute("protocolSupportEnumeration", "urn:oasis:names:tc:SAML:2.0:protocol")),
                new XElement(Saml2Namespaces.Saml2Metadata + "IDPSSODescriptor",
                    new XAttribute("protocolSupportEnumeration", "urn:oasis:names:tc:SAML:2.0:protocol")));

            var subject = MetadataLoader.LoadEntityDescriptor(metadata);

            subject.RoleDescriptors.OfType<IdentityProviderSingleSignOnDescriptor>()
                .Count().Should().Be(3);
        }

        [TestMethod]
        public void MetadataLoader_LoadEntityDescriptor_IdpContainsSignOnServices()
        {
            var metadata = CreateBasicMetadata();
            metadata.Element(Saml2Namespaces.Saml2Metadata + "IDPSSODescriptor")
                .Add(new XElement(Saml2Namespaces.Saml2Metadata + "SingleSignOnService"),
                new XElement(Saml2Namespaces.Saml2Metadata + "SingleSignOnService"));

            var subject = MetadataLoader.LoadEntityDescriptor(metadata);

            subject.RoleDescriptors.OfType<IdentityProviderSingleSignOnDescriptor>()
                .FirstOrDefault().SingleSignOnServices.Count.Should().Be(3);
        }
    }
}
