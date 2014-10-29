using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using FluentAssertions;
using Kentor.AuthServices.Metadata;
using System.IdentityModel.Metadata;
using System.Xml.Linq;
using Kentor.AuthServices.WebSso;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class ExtendedMetadataSerializerTests
    {
        [TestMethod]
        public void ExtendedMetadataSerializer_Read_EntityDescriptorValidUntil()
        {
            var data =
@"<EntityDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
  entityID=""http://idp.example.com/"" validUntil=""2100-01-02T14:42:43Z"" />";

            var entityDescriptor = ExtendedMetadataSerializer.Instance.ReadMetadata(
                new MemoryStream(Encoding.UTF8.GetBytes(data)));

            var subject = entityDescriptor as ExtendedEntityDescriptor;

            subject.Should().NotBeNull();
            subject.ValidUntil.Should().Be(new DateTime(2100, 01, 02, 14, 42, 43, DateTimeKind.Utc));
            subject.CacheDuration.Should().NotHaveValue();
        }

        [TestMethod]
        public void ExtendedMetadataSerializer_Read_EntityDescriptorCacheDuration()
        {
            var data =
@"<EntityDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
  entityID=""http://idp.example.com/"" cacheDuration=""PT42M"" />";

            var entityDescriptor = ExtendedMetadataSerializer.Instance.ReadMetadata(
                new MemoryStream(Encoding.UTF8.GetBytes(data)));

            var subject = entityDescriptor as ExtendedEntityDescriptor;

            subject.Should().NotBeNull();
            subject.ValidUntil.Should().NotHaveValue();
            subject.CacheDuration.Should().Be(new TimeSpan(0, 42, 0));
        }

        [TestMethod]
        public void ExtendedMetadataSerializer_Read_EntitiesDescriptorValidUntil()
        {
            var data =
@"<EntitiesDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
  validUntil=""2100-01-02T14:42:43Z"">
  <EntityDescriptor entityID=""http://idp.example.com"" />
</EntitiesDescriptor>
";

            var entitiesDescriptor = ExtendedMetadataSerializer.Instance.ReadMetadata(
                new MemoryStream(Encoding.UTF8.GetBytes(data)));

            var subject = entitiesDescriptor as ExtendedEntitiesDescriptor;

            subject.Should().NotBeNull();
            subject.ValidUntil.Should().Be(new DateTime(2100, 01, 02, 14, 42, 43, DateTimeKind.Utc));
            subject.CacheDuration.Should().NotHaveValue();
        }

        [TestMethod]
        public void ExtendedMetadataSerializer_Read_EntitiesDescriptorCacheDuration()
        {
            var data =
@"<EntitiesDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
  cacheDuration=""PT15M"">
  <EntityDescriptor entityID=""http://idp.example.com"" />
</EntitiesDescriptor>"
;

            var entitiesDescriptor = ExtendedMetadataSerializer.Instance.ReadMetadata(
                new MemoryStream(Encoding.UTF8.GetBytes(data)));

            var subject = entitiesDescriptor as ExtendedEntitiesDescriptor;

            subject.Should().NotBeNull();
            subject.ValidUntil.Should().NotHaveValue();
            subject.CacheDuration.Should().Be(new TimeSpan(0, 15, 0));
        }

        [TestMethod]
        public void ExtendedMetadataSerializer_Write_EntitiesDescriptorCacheDuration()
        {
            var metadata = new ExtendedEntitiesDescriptor
            {
                Name = "Federation Name",
                CacheDuration = new TimeSpan(0, 42, 0)
            };

            var entity = new ExtendedEntityDescriptor
                {
                    EntityId = new EntityId("http://some.entity.example.com")
                };

            var idpSsoDescriptor = new IdentityProviderSingleSignOnDescriptor();
            idpSsoDescriptor.ProtocolsSupported.Add(new Uri("urn:oasis:names:tc:SAML:2.0:protocol"));

            idpSsoDescriptor.SingleSignOnServices.Add(new ProtocolEndpoint
                {
                    Binding = Saml2Binding.HttpRedirectUri,
                    Location = new Uri("http://some.entity.example.com/sso")
                });
            entity.RoleDescriptors.Add(idpSsoDescriptor);

            metadata.ChildEntities.Add(entity);

            var stream = new MemoryStream();
            ExtendedMetadataSerializer.Instance.WriteMetadata(stream, metadata);
            stream.Seek(0, SeekOrigin.Begin);

            var result = XDocument.Load(stream).Root;

            result.Name.Should().Be(Saml2Namespaces.Saml2Metadata + "EntitiesDescriptor");
            result.Attribute("cacheDuration").Value.Should().Be("PT42M");

            result.Element(Saml2Namespaces.Saml2Metadata + "EntityDescriptor").Attribute("cacheDuration")
                .Should().BeNull();
        }
    }
}
