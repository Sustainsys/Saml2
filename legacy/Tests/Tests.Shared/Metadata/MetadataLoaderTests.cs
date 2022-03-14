using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Xml.Linq;
using System.Linq;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Metadata.Descriptors;

namespace Sustainsys.Saml2.Tests.Metadata
{
    [TestClass]
    public class MetadataLoaderTests
    {
        [TestMethod]
        public void MetadataLoader_LoadIdp()
        {
            var entityId = "http://localhost:13428/idpMetadata";
            var subject = MetadataLoader.LoadIdp(entityId);

            subject.EntityId.Id.Should().Be(entityId);
        }

        [TestMethod]
        public void MetadataLoader_LoadIdp_Nullcheck()
        {
            Action a = () => MetadataLoader.LoadIdp(null);

            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("metadataLocation");
        }

        [TestMethod]
        public void MetadataLoader_LoadIdp_ExplanatoryExceptionIfEntitiesDescriptorFound()
        {
            var metadataLocation = "http://localhost:13428/federationMetadata";

            Action a = () => MetadataLoader.LoadIdp(metadataLocation);

            a.Should().Throw<InvalidOperationException>().
                WithMessage(MetadataLoader.LoadIdpFoundEntitiesDescriptor);
        }

        [TestMethod]
        public void MetadataLoader_LoadFederation_Nullcheck()
        {
            Action a = () => MetadataLoader.LoadFederation(null);

            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("metadataLocation");
        }

        [TestMethod]
        public void MetadataLoader_LoadFederation()
        {
            var metadataLocation = "http://localhost:13428/federationMetadata";

            var subject = MetadataLoader.LoadFederation(metadataLocation);

            subject.ChildEntities.First().EntityId.Id.Should().Be("http://idp.federation.example.com/metadata");
        }

        [TestMethod]
        public void MetadataLoader_LoadFederation_FromFile()
        {
            var metadataLocation = "~/Metadata/SambiMetadata.xml";

            var result = MetadataLoader.LoadFederation(metadataLocation);

            result.ChildEntities.First().EntityId.Id.Should().Be("https://idp.maggie.bif.ost.se:9445/idp/saml");
        }

        [TestMethod]
        public void MetadataLoader_LoadFederation_ExplanatoryExceptionIfEntitiesDescriptorFound()
        {
            var entityId = "http://localhost:13428/idpMetadata";

            Action a = () => MetadataLoader.LoadFederation(entityId);

            a.Should().Throw<InvalidOperationException>().
                WithMessage(MetadataLoader.LoadFederationFoundEntityDescriptor);
        }

        [TestMethod]
        public void MetadataLoader_LoadIdentityProvider_UnpacksEntitiesDescriptorIfFlagSet()
        {
            var metadataLocation = "~/Metadata/SingleIdpInEntitiesDescriptor.xml";

            var actual = MetadataLoader.LoadIdp(metadataLocation, true);

            actual.Should().BeOfType<EntityDescriptor>();
        }

        [TestMethod]
        public void MetadataLoader_LoadIdentityProvider_ThrowsOnMultipleEntityDescriptorsWhenUnpackingEntitiesDescriptor()
        {
            var metadataLocation = "~/Metadata/SambiMetadata.xml";

            Action a = () => MetadataLoader.LoadIdp(metadataLocation, true);

            a.Should().Throw<InvalidOperationException>()
                .WithMessage(MetadataLoader.LoadIdpUnpackingFoundMultipleEntityDescriptors);
        }
    }
}
