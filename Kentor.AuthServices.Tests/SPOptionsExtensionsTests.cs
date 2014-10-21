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

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class SPOptionsExtensionsTests
    {
        [TestMethod]
        public void SPOPtionsExtensions_CreateMetadata_RequiredFields()
        {
            var metadata = Options.FromConfiguration.SPOptions.CreateMetadata(StubFactory.CreateAuthServicesUrls());

            metadata.CacheDuration.Should().Be(new TimeSpan(0, 0, 42));
            metadata.EntityId.Id.Should().Be("https://github.com/KentorIT/authservices");

            var spMetadata = metadata.RoleDescriptors.OfType<ServiceProviderSingleSignOnDescriptor>().Single();
            spMetadata.Should().NotBeNull();

            var acs = spMetadata.AssertionConsumerServices.First().Value;

            acs.Index.Should().Be(0);
            acs.IsDefault.Should().HaveValue();
            acs.Binding.ToString().Should().Be("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST");
            acs.Location.ToString().Should().Be("http://localhost/AuthServices/Acs");
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

            var subject = spOptions.CreateMetadata(urls).Extensions.DiscoveryResponse;

            var expected = new IndexedProtocolEndpoint
            {
                Binding = Saml2Binding.DiscoveryResponseUri,
                Index = 0,
                IsDefault = true,
                Location = urls.SignInUrl
            };

            subject.ShouldBeEquivalentTo(expected);
        }
    }
}
