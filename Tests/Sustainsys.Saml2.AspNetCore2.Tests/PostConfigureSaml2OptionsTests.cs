using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using NSubstitute;
using Microsoft.Extensions.Logging;
using Kentor.AuthServices;

namespace Sustainsys.Saml2.AspNetCore2.Tests
{
    [TestClass]
    public class PostConfigureSaml2OptionsTests
    {
        [TestMethod]
        public void PostConfigureSaml2Options_PostConfigure_Nullcheck()
        {
            var subject = new PostConfigureSaml2Options(null);

            subject.Invoking(s => s.PostConfigure(null, null))
                .ShouldThrow<ArgumentNullException>().
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

            var subject = new PostConfigureSaml2Options(loggerFactory);

            subject.PostConfigure(null, options);

            logger.ReceivedLogLevel.Should().Be(LogLevel.Debug);
            logger.ReceivedMessage.Should().Match("*enabled*");
        }

        [TestMethod]
        public void PostConfigureSaml2Options_PostConfigure_NullLoggerFactoryGivesNullLogger()
        {
            var options = new Saml2Options();

            options.SPOptions.Logger.Should().BeNull("Precondition");

            var subject = new PostConfigureSaml2Options(null);

            subject.PostConfigure(null, options);

            options.SPOptions.Logger.Should().BeOfType<NullLoggerAdapter>();
        }
    }
}
