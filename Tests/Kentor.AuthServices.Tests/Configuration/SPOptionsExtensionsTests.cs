using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Text.RegularExpressions;
using System.IdentityModel.Metadata;
using Kentor.AuthServices.Configuration;
using System.Globalization;
using Kentor.AuthServices.Metadata;
using Kentor.AuthServices.WebSso;
using Kentor.AuthServices.Tests.Helpers;

namespace Kentor.AuthServices.Tests.Configuration
{
    [TestClass]
    public class SPOptionsExtensionsTests
    {
        [TestMethod]
        public void SPOPtionsExtensions_CreateMetadata_RequiredFields()
        {
            var metadata = StubFactory.CreateSPOptions().CreateMetadata(StubFactory.CreateAuthServicesUrls());

            metadata.CacheDuration.Should().Be(new TimeSpan(0, 0, 42));
            metadata.EntityId.Id.Should().Be("https://github.com/KentorIT/authservices");

            var spMetadata = metadata.RoleDescriptors.OfType<ServiceProviderSingleSignOnDescriptor>().Single();
            spMetadata.Should().NotBeNull();
            spMetadata.Keys.Count.Should().Be(0);

            var acs = spMetadata.AssertionConsumerServices.First().Value;

            acs.Index.Should().Be(0);
            acs.IsDefault.Should().HaveValue();
            acs.Binding.ToString().Should().Be("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST");
            acs.Location.ToString().Should().Be("http://localhost/AuthServices/Acs");

            // No service certificate configured, so no SLO endpoint should be
            // exposed in metadata.
            spMetadata.SingleLogoutServices.Should().BeEmpty();
        }

        [TestMethod]
        public void SPOptionsExtensions_CreateMetadata_WithServiceCertificateConfigured()
        {
            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(new ServiceCertificate { Certificate = SignedXmlHelper.TestCert2 });
            var metadata = options.SPOptions.CreateMetadata(StubFactory.CreateAuthServicesUrls());

            var spMetadata = metadata.RoleDescriptors.OfType<ServiceProviderSingleSignOnDescriptor>().Single();
            spMetadata.Should().NotBeNull();
            spMetadata.Keys.Count.Should().Be(1);
            spMetadata.Keys.Single().Use.Should().Be(KeyType.Unspecified);

            // When there is a service certificate, expose SLO endpoints.
            var sloRedirect = spMetadata.SingleLogoutServices.Single(
                slo => slo.Binding == Saml2Binding.HttpRedirectUri);
            sloRedirect.Location.Should().Be("http://localhost/AuthServices/Logout");
            sloRedirect.ResponseLocation.Should().BeNull();
            var sloPost = spMetadata.SingleLogoutServices.Single(
                slo => slo.Binding == Saml2Binding.HttpPostUri);
            sloPost.Location.Should().Be("http://localhost/AuthServices/Logout");
            sloPost.ResponseLocation.Should().BeNull();
        }

        [TestMethod]
        public void SPOptionsExtensions_CreateMetadata_MultipleServiceCertificate()
        {
            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(new ServiceCertificate { Certificate = SignedXmlHelper.TestCert2, Use = CertificateUse.Encryption });
            options.SPOptions.ServiceCertificates.Add(new ServiceCertificate { Certificate = SignedXmlHelper.TestCert2, Use = CertificateUse.Signing });
            var metadata = options.SPOptions.CreateMetadata(StubFactory.CreateAuthServicesUrls());

            var spMetadata = metadata.RoleDescriptors.OfType<ServiceProviderSingleSignOnDescriptor>().Single();
            spMetadata.Should().NotBeNull();
            spMetadata.Keys.Count.Should().Be(2);
            spMetadata.Keys.Where(k => k.Use == KeyType.Encryption).Count().Should().Be(1);
            spMetadata.Keys.Where(k => k.Use == KeyType.Signing).Count().Should().Be(1);
        }

        [TestMethod]
        public void SPOptionsExtensions_CreateMetadata_IncludesOrganization()
        {
            var subject = StubFactory
                .CreateSPOptions()
                .CreateMetadata(StubFactory.CreateAuthServicesUrls())
                .Organization;

            subject.Should().NotBeNull();
            subject.Names.First().Name.Should().Be("Kentor.AuthServices");
        }

        [TestMethod]
        public void SPOptionsExtensions_CreateMetadata_IncludesContactPersons()
        {
            var spOptions = StubFactory.CreateSPOptions();

            var subject = spOptions.CreateMetadata(StubFactory.CreateAuthServicesUrls()).Contacts;

            subject.Should().Contain(spOptions.Contacts);
        }

        [TestMethod]
        public void SPOptionsExtensions_CreateMetadata_IncludeDiscoveryServiceResponse()
        {
            var spOptions = StubFactory.CreateSPOptions();
            var urls = StubFactory.CreateAuthServicesUrls();

            spOptions.DiscoveryServiceUrl = new Uri("http://ds.example.com");

            var subject = spOptions.CreateMetadata(urls).RoleDescriptors
                .Single().As<ExtendedServiceProviderSingleSignOnDescriptor>()
                .Extensions.DiscoveryResponse;

            var expected = new IndexedProtocolEndpoint
            {
                Binding = Saml2Binding.DiscoveryResponseUri,
                Index = 0,
                IsDefault = true,
                Location = urls.SignInUrl
            };

            subject.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void SPOptionsExtensions_CreateMetadata_IncludeAttributeConsumingService()
        {
            var spOptions = StubFactory.CreateSPOptions();
            var urls = StubFactory.CreateAuthServicesUrls();

            var attributeConsumingService = new AttributeConsumingService("Name");

            spOptions.AttributeConsumingServices.Clear();
            spOptions.AttributeConsumingServices.Add(attributeConsumingService);
            attributeConsumingService.RequestedAttributes.Add(new RequestedAttribute("AttributeName"));

            var subject = spOptions
                .CreateMetadata(urls)
                .RoleDescriptors
                .Cast<ExtendedServiceProviderSingleSignOnDescriptor>()
                .First();

            subject.AttributeConsumingServices.First().Should().BeSameAs(attributeConsumingService);
        }
    }
}
