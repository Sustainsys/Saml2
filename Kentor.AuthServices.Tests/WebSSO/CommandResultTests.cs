using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Net;
using NSubstitute;
using System.Web;
using System.Security.Claims;
using System.Security.Principal;
using Kentor.AuthServices.WebSso;

namespace Kentor.AuthServices.Tests.WebSso
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
