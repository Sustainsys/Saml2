using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using NSubstitute;
using Microsoft.Extensions.Logging;
using Sustainsys.Saml2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Sustainsys.Saml2.AspNetCore2.Tests
{
    [TestClass]
    public class PostConfigureSaml2OptionsTests
    {
        [TestMethod]
        public void PostConfigureSaml2Options_PostConfigure_Nullcheck()
        {
            var subject = new PostConfigureSaml2Options(
                null, TestHelpers.GetAuthenticationOptions());

            subject.Invoking(s => s.PostConfigure(null, null))
                .Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("options");
        }

        [TestMethod]
        public void PostConfigureSaml2Options_PostConfigure_Logger()
        {
            var options = new Saml2Options();

            options.SPOptions.Logger.Should().BeNull("Precondition");

            var loggerFactory = Substitute.For<ILoggerFactory>();
            var logger = new MockLogger();
            loggerFactory.CreateLogger<Saml2Handler>().Returns(logger);

            var subject = new PostConfigureSaml2Options(
                loggerFactory, TestHelpers.GetAuthenticationOptions());

            subject.PostConfigure(null, options);

            logger.ReceivedLogLevel.Should().Be(LogLevel.Debug);
            logger.ReceivedMessage.Should().Match("*enabled*");
        }

        [TestMethod]
        public void PostConfigureSaml2Options_PostConfigure_NullLoggerFactoryGivesNullLogger()
        {
            var options = new Saml2Options();

            options.SPOptions.Logger.Should().BeNull("Precondition");

            var subject = new PostConfigureSaml2Options(
                null, TestHelpers.GetAuthenticationOptions());

            subject.PostConfigure(null, options);

            options.SPOptions.Logger.Should().BeOfType<NullLoggerAdapter>();
        }

        [TestMethod]
        public void PostConfigureSaml2Options_PostConfigure_EnsureSignInSchemeAssignsDefaultSignInScheme()
        {
            var options = new Saml2Options();

            options.SignInScheme.Should().BeNull("Precondition");

            var subject = new PostConfigureSaml2Options(null,
                TestHelpers.GetAuthenticationOptions());

            subject.PostConfigure(null, options);

            options.SignInScheme.Should().Be(TestHelpers.defaultSignInScheme);
        }

        [TestMethod]
        public void PostConfigureSaml2Options_PostConfigure_EnsureSignInSchemeFallbackDefaultScheme()
        {
            var options = new Saml2Options();

            options.SignInScheme.Should().BeNull("Precondition");

            var authOptions = TestHelpers.GetAuthenticationOptions();
            authOptions.Value.DefaultSignInScheme = null;
            authOptions.Value.DefaultScheme = "defaultScheme";

            var subject = new PostConfigureSaml2Options(null,authOptions);

            subject.PostConfigure(null, options);

            options.SignInScheme.Should().Be("defaultScheme");
        }

        [TestMethod]
        public void PostConfigureSaml2Options_PostConfigure_EnsureSignInSchemePreservesSetScheme()
        {
            var options = new Saml2Options();
            options.SignInScheme = "specificSignInScheme";

            var subject = new PostConfigureSaml2Options(null,
                TestHelpers.GetAuthenticationOptions());

            subject.PostConfigure(null, options);

            options.SignInScheme.Should().Be("specificSignInScheme");
        }

        [TestMethod]
        public void PostConfigureSaml2Options_PostConfigure_EnsureSignOutSchemeAssignsDefaultSignOutScheme()
        {
            var options = new Saml2Options();

            options.SignOutScheme.Should().BeNull("Precondition");

            var subject = new PostConfigureSaml2Options(null,
                TestHelpers.GetAuthenticationOptions());

            subject.PostConfigure(null, options);

            options.SignOutScheme.Should().Be(TestHelpers.defaultSignOutScheme);
        }

        [TestMethod]
        public void PostConfigureSaml2Options_PostConfigure_EnsureSignOutSchemeFallbackDefaultAuthenticateScheme()
        {
            var options = new Saml2Options();

            options.SignInScheme.Should().BeNull("Precondition");

            var authOptions = TestHelpers.GetAuthenticationOptions();
            authOptions.Value.DefaultSignOutScheme = null;
            authOptions.Value.DefaultAuthenticateScheme = "defaultAuthenticateScheme";

            var subject = new PostConfigureSaml2Options(null, authOptions);

            subject.PostConfigure(null, options);

            options.SignOutScheme.Should().Be("defaultAuthenticateScheme");
        }

        [TestMethod]
        public void PostConfigureSaml2Options_PostConfigure_EnsureSignOutSchemePreservesSetScheme()
        {
            var options = new Saml2Options();
            options.SignOutScheme = "specificSignOutScheme";

            var subject = new PostConfigureSaml2Options(null,
                TestHelpers.GetAuthenticationOptions());

            subject.PostConfigure(null, options);

            options.SignOutScheme.Should().Be("specificSignOutScheme");
        }
    }
}
