using FluentAssertions;
using Sustainsys.Saml2.Metadata.Attributes;
using Sustainsys.Saml2.Metadata.Elements;
using System;
using System.Linq;
using System.Security.Cryptography.Xml;
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

        var expected = new IDPSSODescriptor
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

    [Fact]
    public void ReadIDPSSODescriptor_Large()
    {
        var xmlTraverser = GetXmlTraverser();

        var actual = new MetadataSerializer(null, null).ReadEntityDescriptor(xmlTraverser)
            .RoleDescriptors.OfType<IDPSSODescriptor>().Single();

        var expectedKeyInfo = new KeyInfo();
        expectedKeyInfo.AddClause(new KeyInfoX509Data(Convert.FromBase64String("MIICFTCCAYKgAwIBAgIQzfcJCkM1YahDtRGYsLphrDAJBgUrDgMCHQUAMCExHzAdBgNVBAMTFnN0dWJpZHAuc3VzdGFpbnN5cy5jb20wHhcNMTcxMjE0MTE1NDUwWhcNMzkxMjMxMjM1OTU5WjAhMR8wHQYDVQQDExZzdHViaWRwLnN1c3RhaW5zeXMuY29tMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDSSq8EX46J1yprfaBdh4pWII+/E7ypHM1NjG7mCwFwbkjq2tpSBuoASrQftbjIKqjVzxtxETw802VJu5CJR4d3Zdy5jD8NRTesfaQDazX7iiqisfnxmIdDhtJS0lXeBlj4MipoUW6l8Qsjx7ltZSwdfFLyh+bMqIrwOhMWGs82vQIDAQABo1YwVDBSBgNVHQEESzBJgBCBBNba7KNF5wnXqmYcejn6oSMwITEfMB0GA1UEAxMWc3R1YmlkcC5zdXN0YWluc3lzLmNvbYIQzfcJCkM1YahDtRGYsLphrDAJBgUrDgMCHQUAA4GBAHonBGahlldp7kcN5HGGnvogT8a0nNpM7GMdKhtzpLO3Uk3HyT3AAIKWiSoEv2n1BTalJ/CY/+te/JZPVGhWVzoi5JYytpj5gM0O7RH0a3/yUE8S8YLV2h0a2gsdoMvTRTnTm9CnXezCKqhjYjwsmOZtiCIYuFqX71d/pg5uoJfs")));

        // The expected value deliberately does not include all the data available in the
        // XML. The purpose of this test is to ensure that the supported data is read and
        // that the schema validation of the reader works even when there is more data that
        // is not supported by this implementation (yet).
        var expected = new IDPSSODescriptor
        {
            ProtocolSupportEnumeration = "urn:oasis:names:tc:SAML:2.0:protocol",
            Keys =
            {
                new KeyDescriptor
                {
                    Use = KeyUse.Both,
                    KeyInfo = expectedKeyInfo
                },
                new KeyDescriptor
                {
                    Use = KeyUse.Signing,
                    KeyInfo = expectedKeyInfo
                }
            },
            ArtifactResolutionServices =
            {
                new IndexedEndpoint
                {
                    Binding = Binding.SOAP,
                    Location = "https://stubidp.sustainsys.com/ArtifactResolve",
                    Index = 2,
                    IsDefault = true
                }
            },
            SingleLogoutServices =
            {
                new Endpoint
                {
                    Binding = Binding.HttpRedirect,
                    Location = "https://stubidp.sustainsys.com/Logout"
                },
                new Endpoint
                {
                    Binding = Binding.HttpPOST,
                    Location = "https://stubidp.sustainsys.com/Logout"
                }
            },
            WantAuthnRequestsSigned = true,
            SingleSignOnServices =
            {
                new Endpoint
                {
                    Binding = Binding.HttpRedirect,
                    Location = "https://stubidp.sustainsys.com/"
                },
                new Endpoint
                {
                    Binding = Binding.HttpPOST,
                    Location = "https://stubidp.sustainsys.com/"
                }
            }
        };

        actual.Should().BeEquivalentTo(expected);
    }
}
