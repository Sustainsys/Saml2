using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Configuration;
using System.IdentityModel.Metadata;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class SPOptionsTests
    {
        const string entityId = "http://localhost/idp";
        string otherEntityId = "http://something.else.com";

        [TestMethod]
        public void SPOptions_Saml2PSecurityTokenHandler_DefaultInstanceCreated()
        {
            var subject = new SPOptions
            {
                EntityId = new EntityId(entityId)
            };

            subject.Saml2PSecurityTokenHandler.Should().NotBeNull();
            subject.Saml2PSecurityTokenHandler.Configuration.AudienceRestriction.AllowedAudienceUris
                .Should().Contain(new Uri(entityId));
        }

        [TestMethod]
        public void SPOptions_Saml2PSecurityTokenHandler_ManuallySettingInstanceWorks()
        {
            var subject = new SPOptions
            {
                EntityId = new EntityId(entityId)
            };

            subject.Saml2PSecurityTokenHandler = new Saml2PSecurityTokenHandler(new EntityId(otherEntityId));

            subject.Saml2PSecurityTokenHandler.Should().NotBeNull();
            subject.Saml2PSecurityTokenHandler.Configuration.AudienceRestriction.AllowedAudienceUris
                .Should().Contain(new Uri(otherEntityId));
        }

        [TestMethod]
        public void SPOptions_EntityId_SettingThrowsIfAutomaticTokenHandlerCreated()
        {
            var subject = new SPOptions
            {
                EntityId = new EntityId(entityId)
            };

            subject.Saml2PSecurityTokenHandler.Should().NotBeNull();

            Action a = () => subject.EntityId = new EntityId(otherEntityId);

            a.ShouldThrow<InvalidOperationException>("Can't change entity id when a token handler has been instantiated.");
        }

        [TestMethod]
        public void SPOptions_EntityId_SettingAllowedWithManualTokenHandler()
        {
            var subject = new SPOptions
            {
                EntityId = new EntityId(entityId)
            };

            var autoHandler = subject.Saml2PSecurityTokenHandler;

            var handler = new Saml2PSecurityTokenHandler(new EntityId(otherEntityId));
            subject.Saml2PSecurityTokenHandler = handler;

            Action a = () => subject.EntityId = new EntityId("http://whatever.example.com");
            a.ShouldNotThrow();
        }

        [TestMethod]
        public void SPOPtions_ModulePath_Default()
        {
            var subject = new SPOptions();
            subject.ModulePath.Should().Be("/AuthServices");
        }

        [TestMethod]
        public void SPOPtions_ModulePath_NonDefault()
        {
            var subject = new SPOptions();
            subject.ModulePath = "/Foo";
            subject.ModulePath.Should().Be("/Foo");
        }

        [TestMethod]
        public void SPOptions_ModulePath_FixesSlashes()
        {
            var subject = new SPOptions();
            subject.ModulePath = "Foo/";
            subject.ModulePath.Should().Be("/Foo");
        }

        [TestMethod]
        public void SPOptions_ModulePath_RejectsNull()
        {
            var subject = new SPOptions();
            Action a = () => subject.ModulePath = null;

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("value");
        }
    }
}
