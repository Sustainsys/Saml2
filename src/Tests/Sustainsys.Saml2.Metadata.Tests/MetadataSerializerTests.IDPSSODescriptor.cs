using FluentAssertions;
using Sustainsys.Saml2.Metadata.Attributes;
using Sustainsys.Saml2.Metadata.Elements;
using System.Linq;
using Xunit;

namespace Sustainsys.Saml2.Metadata.Tests;
public partial class MetadataSerializerTests
{
    [Fact]
    public void ReadIDPSSODescriptor_Mandatory()
    {
        var xmlTraverser = GetXmlTraverser();

        var actual = new MetadataSerializer(null, null).ReadEntityDescriptor(xmlTraverser)
            .RoleDescriptors.OfType<IDPSSODescriptor>().Single();

        var expected = new IDPSSODescriptor()
        {
            ProtocolSupportEnumeration = "urn:oasis:names:tc:SAML:2.0:protocol",
            SingleSignOnServices =  
            {
                new Endpoint
                {
                    Binding = Binding.HttpRedirect,
                    Location = "https://stubidp.sustainsys.com/"
                }
            }
        };

        actual.Should().BeEquivalentTo(expected);
    }
}
