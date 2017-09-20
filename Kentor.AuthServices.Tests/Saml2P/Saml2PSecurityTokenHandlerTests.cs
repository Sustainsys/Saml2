using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Kentor.AuthServices.Saml2P;
using Kentor.AuthServices.Configuration;

namespace Kentor.AuthServices.Tests.Saml2P
{
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.Linq;

    [TestClass]
    public class Saml2PSecurityTokenHandlerTests
    {
        [TestMethod]
        public void Saml2PSecurityTokenHandler_Ctor_Nullcheck()
        {
            Action a = () => new Saml2PSecurityTokenHandler(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("spOptions");
        }

        [TestMethod]
        public void Saml2PSecurityTokenHandler_ShouldHaveDefaultAudienceRestrictionOfEntityId()
        {
            var spOptions = StubFactory.CreateSPOptions();
            spOptions.SystemIdentityModelIdentityConfiguration.AudienceRestriction.AudienceMode
                 = AudienceUriMode.Always;

            spOptions.EntityId.Id = "http://testuri/";

            var subject = new Saml2PSecurityTokenHandler(spOptions);

            subject.Configuration.AudienceRestriction.AllowedAudienceUris.First().AbsoluteUri.Should().Be("http://testuri/");
        }

        [TestMethod]
        public void Saml2PSecurityTokenHandler_ShouldReadAudienceUrisFromIdentityModelConfig()
        {
            var spOptions = StubFactory.CreateSPOptions();
            spOptions.SystemIdentityModelIdentityConfiguration.AudienceRestriction = 
                new AudienceRestriction
                    {
                        AllowedAudienceUris = { new Uri("http://firsturi/"), new Uri("http://seconduri/") }
                    };

            var subject = new Saml2PSecurityTokenHandler(spOptions);

            subject.Configuration.AudienceRestriction.AllowedAudienceUris.Should().Contain(new Uri("http://firsturi/"));
            subject.Configuration.AudienceRestriction.AllowedAudienceUris.Should().Contain(new Uri("http://seconduri/"));
        }

        [TestMethod]
        public void Saml2PSecurityTokenHandler_SaveBootstrapContextDefaultFalse()
        {
            var spOptions = StubFactory.CreateSPOptions();

            var subject = new Saml2PSecurityTokenHandler(spOptions);

            subject.Configuration.SaveBootstrapContext.Should().BeFalse();
        }

        [TestMethod]
        public void Saml2pSecurityTokenHandler_SaveBootstrapContextLoadFromConfig()
        {
            var spOptions = Options.FromConfiguration.SPOptions;

            var subject = new Saml2PSecurityTokenHandler(spOptions);

            subject.Configuration.SaveBootstrapContext.Should().BeTrue();
        }

        [TestMethod]
        public void Saml2PSecurityTokenHandler_SaveBoostrapContextFromConfig()
        {
            var spOptions = StubFactory.CreateSPOptions();
            spOptions.SystemIdentityModelIdentityConfiguration.SaveBootstrapContext = true;

            var subject = new Saml2PSecurityTokenHandler(spOptions);

            subject.Configuration.SaveBootstrapContext.Should().BeTrue();
        }
    }
}
