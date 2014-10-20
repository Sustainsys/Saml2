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

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class SPOptionsExtensionsTests
    {
        [TestMethod]
        public void SPOPtionsExtensions_CreateMetadata_RequiredFields()
        {
            var metadata = Options.FromConfiguration.SPOptions.CreateMetadata(TestObjects.authServicesUrls);

            metadata.EntityId.Id.Should().Be("https://github.com/KentorIT/authservices");

            var spMetadata = metadata.RoleDescriptors.OfType<ServiceProviderSingleSignOnDescriptor>().Single();
            spMetadata.Should().NotBeNull();

            var acs = spMetadata.AssertionConsumerServices.First().Value;

            acs.Index.Should().Be(0);
            acs.IsDefault.Should().HaveValue();
            acs.Binding.ToString().Should().Be("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST");
            acs.Location.ToString().Should().Be("http://localhost/AuthServices/Acs");
        }
    }
}
