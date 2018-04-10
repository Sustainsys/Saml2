using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Sustainsys.Saml2.Metadata;
using System.Collections.Generic;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.WebSso;
using Sustainsys.Saml2.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using Sustainsys.Saml2.TestHelpers;

namespace Sustainsys.Saml2.Tests.Configuration
{
    [TestClass]
    public class OptionsTests
    {
        [TestMethod]
        public void Options_FromConfiguration_IdentityProviders_IncludeIdpFromFederation()
        {
            var subject = Options.FromConfiguration.IdentityProviders[
                new EntityId("http://idp.federation.example.com/metadata")];

            subject.EntityId.Id.Should().Be("http://idp.federation.example.com/metadata");
            subject.Binding.Should().Be(Saml2BindingType.HttpRedirect);
        }

        [TestMethod]
        public void Options_FromConfiguration_IdentityProviders_ThrowsOnInvalidEntityId()
        {
            Action a = () =>
            {
                var i = Options.FromConfiguration.IdentityProviders[
                new EntityId("urn:Non.Existent.EntityId")];
            };

            a.Should().Throw<KeyNotFoundException>().And.Message.Should().Be("No Idp with entity id \"urn:Non.Existent.EntityId\" found.");
        }
    }
}
