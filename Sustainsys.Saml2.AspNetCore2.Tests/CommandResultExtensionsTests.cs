using FluentAssertions;
using Kentor.AuthServices.WebSso;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.AspNetCore2.Tests
{
    [TestClass]
    public class CommandResultExtensionsTests
    {
        [TestMethod]
        public void CommandResultExtensions_Apply()
        {
            var context = TestHelpers.CreateHttpContext();

            var commandResult = new CommandResult()
            {
                HttpStatusCode = System.Net.HttpStatusCode.Redirect,
                Location = new Uri("https://destination.com")
            };

            commandResult.Apply(context);

            context.Response.StatusCode.Should().Be(302);
            context.Response.Headers["Location"].Single().Should().Be("https://destination.com/");
        }

        [TestMethod]
        public void CommandResultExtensions_Apply_Minimal()
        {
            var context = TestHelpers.CreateHttpContext();

            var commandResult = new CommandResult();

            commandResult.Apply(context);

            context.Response.StatusCode.Should().Be(200);
            context.Response.Headers.Keys.Should().NotContain("Location");
        }
    }
}
