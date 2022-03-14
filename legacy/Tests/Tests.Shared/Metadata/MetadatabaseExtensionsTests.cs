using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sustainsys.Saml2.Metadata;
using System.Xml.Linq;
using System.Security.Cryptography.Xml;
using Sustainsys.Saml2.WebSso;
using FluentAssertions;
using Sustainsys.Saml2.Metadata.Descriptors;
using Sustainsys.Saml2.Metadata.Extensions;
using Sustainsys.Saml2.Metadata.Services;
using Sustainsys.Saml2.TestHelpers;

namespace Sustainsys.Saml2.Tests.Metadata
{
    [TestClass]
    public class MetadatabaseExtensionsTests
    {
        [TestMethod]
        public void MetadatabaseExtensions_ToXmlString_IncludesKeyInfo()
        {
            var metadata = new EntityDescriptor
            {
                EntityId = new EntityId("http://idp.example.com/metadata"),
                CacheDuration = new XsdDuration(hours: 1)
            };

            var idpSsoDescriptor = new IdpSsoDescriptor();
            idpSsoDescriptor.ProtocolsSupported.Add(new Uri("urn:oasis:names:tc:SAML:2.0:protocol"));
            metadata.RoleDescriptors.Add(idpSsoDescriptor);

            idpSsoDescriptor.SingleSignOnServices.Add(new SingleSignOnService
            {
                Binding = Saml2Binding.HttpRedirectUri,
                Location = new Uri("http://idp.example.com/sso")
            });

            idpSsoDescriptor.Keys.Add(SignedXmlHelper.TestKeyDescriptor);

            var subject = XDocument.Parse((metadata.ToXmlString(null, "")));

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
