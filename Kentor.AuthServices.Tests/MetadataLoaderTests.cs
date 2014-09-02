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
            var subject = MetadataLoader.Load(new Uri(entityId));

            subject.EntityId.Id.Should().Be(entityId);
        }

        [TestMethod]
        public void MetadataLoader_Load_Nullcheck()
        {
            Action a = () => MetadataLoader.Load(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("metadataUri");
        }
    }
}
