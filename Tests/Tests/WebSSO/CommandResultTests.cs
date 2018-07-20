using System;
using System.Net;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sustainsys.Saml2.WebSso;

namespace Sustainsys.Saml2.Tests.WebSSO
{
    [TestClass]
    public class CommandResultTests
    {
        [TestMethod]
        public void CommandResult_Defaults()
        {
            var expected = new
            {
                HttpStatusCode = HttpStatusCode.OK,
                Cacheability = Cacheability.NoCache,
                Location = (Uri)null,
                Principal = (ClaimsPrincipal)null,
                ContentType = (string)null,
                Content = (string)null,
                RelayState = (string)null,
                RelayData = (object)null,
                TerminateLocalSession = false,
                SetCookieName = (string)null,
                RequestState = (StoredRequestState)null,
                ClearCookieName = (string)null,
                HandledResult = false,
                SessionNotOnOrAfter = (DateTime?)null
            };

            new CommandResult().ShouldBeEquivalentTo(expected);
        }
    }
}
