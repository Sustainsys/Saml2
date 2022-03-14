using FluentAssertions;
using Sustainsys.Saml2.Tests.Helpers;
using System.Runtime.CompilerServices;
using System.Xml;
using Xunit;

namespace Sustainsys.Saml2.Metadata.Tests
{
    public class MetadataSerializerTests
    {
        private XmlReader GetXmlReader([CallerMemberName] string? fileName = null)
            => TestData.GetXmlReader<MetadataSerializerTests>(fileName);

        [Fact]
        public void ReadEntityDescriptor_EntityId()
        {
            var xmlReader = GetXmlReader();

            var subject = new MetadataSerializer();

            var actual = subject.ReadEntityDescriptor(xmlReader);

            var expected = new EntityDescriptor()
            {
                EntityId = "https://stubidp.sustainsys.com/Metadata"
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ReadEntityDescriptor_ValidatesNamespace()
        {
            var xmlReader = GetXmlReader();

            var subject = new MetadataSerializer();

            subject.Invoking(s => s.ReadEntityDescriptor(xmlReader))
                .Should().Throw<Saml2XmlException>()
                .WithMessage("*namespace*");
        }
        
        [Fact]
        public void ReadEntityDescriptor_ValidatesLocalName()
        {
            var xmlReader = GetXmlReader();

            var subject = new MetadataSerializer();

            subject.Invoking(s => s.ReadEntityDescriptor(xmlReader))
                .Should().Throw<Saml2XmlException>()
                .WithMessage("*name*EntityDescriptor*");
        }
    }
}
