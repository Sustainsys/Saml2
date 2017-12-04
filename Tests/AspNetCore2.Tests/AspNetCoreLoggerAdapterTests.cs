using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.AspNetCore2.Tests
{
    [TestClass]
    public class AspNetCoreLoggerAdapterTests
    {
        [TestMethod]
        public void AspNetCoreLoggerAdapter_WriteError()
        {
            var mock = new MockLogger();
            var subject = new AspNetCoreLoggerAdapter(mock);
            var exception = new InvalidOperationException();

            subject.WriteError("error", exception);

            mock.ReceivedLogLevel.Should().Be(LogLevel.Error);
            mock.ReceivedException.Should().BeSameAs(exception);
            mock.ReceivedMessage.Should().Be("error");
        }

        [TestMethod]
        public void AspNetCoreLoggerAdapter_WriteInformation()
        {
            var mock = new MockLogger();
            var subject = new AspNetCoreLoggerAdapter(mock);

            subject.WriteInformation("info");

            mock.ReceivedException.Should().BeNull();
            mock.ReceivedLogLevel.Should().Be(LogLevel.Information);
            mock.ReceivedMessage.Should().Be("info");
        }

        [TestMethod]
        public void AspNetCoreLoggerAdapter_WriteVerbose()
        {
            var mock = new MockLogger();
            var subject = new AspNetCoreLoggerAdapter(mock);

            subject.WriteVerbose("verbose");

            mock.ReceivedException.Should().BeNull();
            mock.ReceivedLogLevel.Should().Be(LogLevel.Debug);
            mock.ReceivedMessage.Should().Be("verbose");
        }
    }
}
