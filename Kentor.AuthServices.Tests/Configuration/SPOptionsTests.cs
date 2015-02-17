using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Configuration;
using System.IdentityModel.Metadata;
using FluentAssertions;
using Kentor.AuthServices.Saml2P;

namespace Kentor.AuthServices.Tests.Configuration
{
    [TestClass]
    public class SPOptionsTests
    {
        const string entityId = "http://localhost/idp";
        const string otherEntityId = "http://something.else.com";

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
        public void SPOptions_EntityId_SettingThrowsIfTokenHandlerCreated()
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

        [TestMethod]
        public void SPOptions_Contacts_IsntNull()
        {
            var subject = new SPOptions();

            subject.Contacts.Should().NotBeNull();
        }

        [TestMethod]
        public void SPOptions_MetadataCacheDuration_DefaultValue()
        {
            var subject = new SPOptions();

            subject.MetadataCacheDuration.Should().Be(new TimeSpan(1, 0, 0));
        }
    }
}
