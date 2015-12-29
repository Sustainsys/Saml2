using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Net;
using System.Web;
using System.Linq;
using NSubstitute;
using System.IO.Compression;
using System.IO;
using System.Xml.Linq;
using Kentor.AuthServices.TestHelpers;
using Kentor.AuthServices.Configuration;
using System.IdentityModel.Metadata;
using Kentor.AuthServices.Internal;
using Kentor.AuthServices.WebSso;

namespace Kentor.AuthServices.Tests.WebSso
{
    [TestClass]
    public class SingleLogoutCommandTests
    {
        [TestMethod]
        public void SingleLogoutCommand_Run_MapsReturnUrl()
        {
            var httpRequest = new HttpRequestData("GET", new Uri("http://localhost/logout?ReturnUrl=%2FReturn.aspx"));

            var subject = new SingleLogOutCommand().Run(httpRequest, Options.FromConfiguration);

            var requestId = AuthnRequestHelper.GetRequestId(subject.Location);

            StoredRequestState storedAuthnData;
            PendingAuthnRequests.TryRemove(new System.IdentityModel.Tokens.Saml2Id(requestId), out storedAuthnData);

            storedAuthnData.ReturnUrl.Should().Be("http://localhost/Return.aspx");
        }

        [TestMethod]
        public void SingleLogoutCommand_Run_With_InvalidIdp_ThrowsException()
        {
            var request = new HttpRequestData("GET", new Uri("http://localhost/logout?idp=no-such-idp-in-config"));

            Action a = () => new SingleLogOutCommand().Run(request, Options.FromConfiguration);

            a.ShouldThrow<InvalidOperationException>().WithMessage("Unknown idp");
        }

        [TestMethod]
        public void SingleLogoutCommand_Run_NullCheckRequest()
        {
            Action a = () => new SingleLogOutCommand().Run(null, Options.FromConfiguration);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("request");
        }

        [TestMethod]
        public void SingleLogoutCommand_Run_NullCheckOptions()
        {
            Action a = () => new SingleLogOutCommand().Run(new HttpRequestData("GET", new Uri("http://localhost")), null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("options");
        }

        [TestMethod]
        public void SingleLogoutCommand_Run_ReturnsRedirectToDiscoveryService()
        {
            var dsUrl = new Uri("http://ds.example.com");

            var options = new Options(new SPOptions
                {
                    DiscoveryServiceUrl = dsUrl,
                    EntityId = new EntityId("https://github.com/KentorIT/authservices")
                });

            var request = new HttpRequestData("GET", new Uri("http://localhost/logout?ReturnUrl=%2FReturn%2FPath"));

            var result = new SingleLogOutCommand().Run(request, options);

            result.HttpStatusCode.Should().Be(HttpStatusCode.SeeOther);

            var queryString = string.Format("?entityID={0}&return={1}&returnIDParam=idp",
                Uri.EscapeDataString(options.SPOptions.EntityId.Id),
                Uri.EscapeDataString(
                    "http://localhost/AuthServices/SingleLogout?ReturnUrl="
                    + Uri.EscapeDataString("/Return/Path")));

            var expectedLocation = new Uri(dsUrl + queryString);

            result.Location.Should().Be(expectedLocation);
        }
    }
}