using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using FluentAssertions;
using Kentor.AuthServices.Metadata;

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
  enetityID=""http://idp.example.com/"" validUntil=""2100-01-02T14:42:43Z"" />";

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
  enetityID=""http://idp.example.com/"" cacheDuration=""PT42M"" />";

            var entityDescriptor = ExtendedMetadataSerializer.Instance.ReadMetadata(
                new MemoryStream(Encoding.UTF8.GetBytes(data)));

            var subject = entityDescriptor as ExtendedEntityDescriptor;

            subject.Should().NotBeNull();
            subject.ValidUntil.Should().NotHaveValue();
            subject.CacheDuration.Should().Be(new TimeSpan(0, 42, 0));
        }
    }
}
