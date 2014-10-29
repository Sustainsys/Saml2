using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IdentityModel.Metadata;
using FluentAssertions;
using System.Xml.Linq;
using System.Linq;
using Kentor.AuthServices.Metadata;

namespace Kentor.AuthServices.Tests.Metadata
{
    [TestClass]
    public class MetadataLoaderTests
    {
        [TestMethod]
        public void MetadataLoader_LoadIdp()
        {
            var entityId = "http://localhost:13428/idpMetadata";
            var subject = MetadataLoader.LoadIdp(new Uri(entityId));

            subject.EntityId.Id.Should().Be(entityId);
        }

        [TestMethod]
        public void MetadataLoader_LoadIdp_Nullcheck()
        {
            Action a = () => MetadataLoader.LoadIdp(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("metadataUrl");
        }

        [TestMethod]
        public void MetadataLoader_LoadFederation_Nullcheck()
        {
            Action a = () => MetadataLoader.LoadFederation(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("metadataUrl");
        }

        [TestMethod]
        public void MetadataLoader_LoadFederation()
        {
            var metadataUrl = new Uri("http://localhost:13428/federationMetadata");

            var subject = MetadataLoader.LoadFederation(metadataUrl);

            subject.ChildEntities.First().EntityId.Id.Should().Be("http://idp.federation.example.com/metadata");
        }
    }
}
