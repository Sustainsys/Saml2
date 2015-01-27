using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IdentityModel.Metadata;
using FluentAssertions;
using System.Xml.Linq;
using System.Linq;
using Kentor.AuthServices.Metadata;
using Kentor.AuthServices.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.IdentityModel.Tokens;

namespace Kentor.AuthServices.Tests.Metadata
{
    [TestClass]
    public class MetadataLoaderTests
    {
        [TestCleanup]
        public void Cleanup()
        {
            MetadataServer.SignFederationMetadata = false;
        }

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

        [TestMethod]
        public void MetadataLoader_LoadFederationSignedWithoutKey()
        {
            MetadataServer.SignFederationMetadata = true;
            var metadataUrl = new Uri("http://localhost:13428/federationMetadata");

            Action a = () => MetadataLoader.LoadFederation(metadataUrl);

            a.ShouldThrow<MetadataFailedValidationException>();
        }

        [TestMethod]
        public void MetadataLoader_LoadFederationDemandShouldFailOnNoSignature()
        {
            var metadataUrl = new Uri("http://localhost:13428/federationMetadata");

            Action a = () => MetadataLoader.LoadFederation(metadataUrl, null, SignatureValidationMethod.Demand);

            a.ShouldThrow<MetadataFailedValidationException>().And.Message.Should().Be("A signature was demanded but the metadata was delivered unsigned.");
        }

        [TestMethod]
        public void MetadataLoader_LoadFederation_ShouldFailOnMissingKey()
        {
            // We do not return a key in metadata to validate with. With no key provided from config or from outside we will
            // fall back on trying to find the key in the message. 
            MetadataServer.SignFederationMetadata = true;
            var metadataUrl = new Uri("http://localhost:13428/federationMetadata");
            Action a = () => MetadataLoader.LoadFederation(metadataUrl, null, SignatureValidationMethod.Default);
            a.ShouldThrow<MetadataFailedValidationException>().And.Message.Should().Be("No key was provided and there was no certificate in the message. Cannot validate signature.");

        }

        [TestMethod]
        [Ignore]
        public void MetadataLoader_LoadFederation_ShouldPickUpKey()
        {
            // Should validate the key but complain that the key is not present in the certificate store. 
            MetadataServer.SignFederationMetadata = false;
            var metadataUrl = new Uri("http://localhost:13428/federationMetadataSigned");
            Action a = () => MetadataLoader.LoadFederation(metadataUrl, null, SignatureValidationMethod.Default);
            a.ShouldThrow<SecurityTokenValidationException>();
        }

        [TestMethod]
        [Ignore]
        public void MetadataLoader_LoadFederation_SignedShouldPassWithKey()
        {
            MetadataServer.SignFederationMetadata = true;
            var metadataUrl = new Uri("http://localhost:13428/federationMetadata");
            var loadedCertificate = new X509Certificate2("Kentor.AuthServices.Tests.pfx");
            var subject = MetadataLoader.LoadFederation(metadataUrl, loadedCertificate, SignatureValidationMethod.Default);

            subject.ChildEntities.First().EntityId.Id.Should().Be("http://idp.federation.example.com/metadata");
        }

        [TestMethod]
        public void MetadataLoader_LoadFederation_SignedShouldFailWithWrongKey()
        {
            MetadataServer.SignFederationMetadata = true;
            var metadataUrl = new Uri("http://localhost:13428/federationMetadata");
            var loadedCertificate = new X509Certificate2("Kentor.AuthServices.Badcertificate.pfx", "password");
            Action a = () => MetadataLoader.LoadFederation(metadataUrl, loadedCertificate, SignatureValidationMethod.Default);

            a.ShouldThrow<MetadataFailedValidationException>().And.Message.Should().Be("Signature validation failed on SAML metadata.");
        }


    }
}
