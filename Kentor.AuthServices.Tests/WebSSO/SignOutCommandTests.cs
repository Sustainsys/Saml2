using FluentAssertions;
using Kentor.AuthServices.WebSso;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Tests.WebSSO
{
    [TestClass]
    public class SignOutCommandTests
    {
        [TestMethod]
        public void SignOutCommand_Run_NullcheckRequest()
        {
            CommandFactory.GetCommand(CommandFactory.SignOutCommandName)
                .Invoking(c => c.Run(null, StubFactory.CreateOptions()))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("request");
        }

        [TestMethod]
        public void SignOutCommand_Run_ReturnsLogoutRequest()
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "NameId", "https://idp.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", "https://idp.example.com")
                }, "Federation"));

            var request = new HttpRequestData("GET", new Uri("http://sp.example.com/AuthServices/SignOut"));

            var actual = CommandFactory.GetCommand(CommandFactory.SignOutCommandName)
                .Run(request, StubFactory.CreateOptions());

            actual.HttpStatusCode.Should().Be(HttpStatusCode.SeeOther);
            actual.Location.GetLeftPart(UriPartial.Path).Should().Be("https://idp.example.com/logout");
        }
    }
}
