using FluentAssertions;
using Sustainsys.Saml2.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Sustainsys.Saml2.Metadata.Tests
{
    public class MetadataSerializerTests
    {
        private XmlReader GetXmlReader([CallerMemberName] string? fileName = null)
            => TestData.GetXmlReader<MetadataSerializerTests>(fileName);

        [Fact]
        public void ReadBasicEntityDescriptor()
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
    }
}
