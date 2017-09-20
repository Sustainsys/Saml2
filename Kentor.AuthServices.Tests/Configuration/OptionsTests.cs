using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.IdentityModel.Metadata;
using System.Collections.Generic;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.WebSso;

namespace Kentor.AuthServices.Tests.Configuration
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

            a.ShouldThrow<KeyNotFoundException>().And.Message.Should().Be("No Idp with entity id \"urn:Non.Existent.EntityId\" found.");
        }
    }
}
