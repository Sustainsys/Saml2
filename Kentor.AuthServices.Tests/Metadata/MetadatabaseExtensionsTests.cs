using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Metadata;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.Xml.Linq;
using Kentor.AuthServices.Tests.Helpers;
using System.Security.Cryptography.Xml;
using Kentor.AuthServices.WebSso;
using FluentAssertions;

namespace Kentor.AuthServices.Tests.Metadata
{
    [TestClass]
    public class MetadatabaseExtensionsTests
    {
        [TestMethod]
        public void MetadatabaseExtensions_ToXmlString_IncludesKeyInfo()
        {
            var metadata = new ExtendedEntityDescriptor
            {
                EntityId = new EntityId("http://idp.example.com/metadata"),
                CacheDuration = new TimeSpan(1, 0, 0)
            };

            var idpSsoDescriptor = new IdentityProviderSingleSignOnDescriptor();
            idpSsoDescriptor.ProtocolsSupported.Add(new Uri("urn:oasis:names:tc:SAML:2.0:protocol"));
            metadata.RoleDescriptors.Add(idpSsoDescriptor);

            idpSsoDescriptor.SingleSignOnServices.Add(new ProtocolEndpoint
            {
                Binding = Saml2Binding.HttpRedirectUri,
                Location = new Uri("http://idp.example.com/sso")
            });

            idpSsoDescriptor.Keys.Add(SignedXmlHelper.TestKeyDescriptor);

            var subject = XDocument.Parse((metadata.ToXmlString(null)));

            var ds = XNamespace.Get(SignedXml.XmlDsigNamespaceUrl);

            subject.Element(Saml2Namespaces.Saml2Metadata + "EntityDescriptor")
                .Element(Saml2Namespaces.Saml2Metadata + "IDPSSODescriptor")
                .Element(Saml2Namespaces.Saml2Metadata + "KeyDescriptor")
                .Element(ds + "KeyInfo")
                .Element(ds + "X509Data")
                .Element(ds + "X509Certificate")
                .Value.Should().StartWith("MIIDIzCCAg+gAwIBAgIQg7mOjTf994NAVxZu4jqXpzAJBgUrDgM");
        }
    }
}
