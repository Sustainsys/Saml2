using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Net;
using System.Web;
using Kentor.AuthServices.Configuration;
using System.IdentityModel.Metadata;
using Kentor.AuthServices.WebSso;
using System.Collections.Generic;

namespace Kentor.AuthServices.Tests.WebSso
{
    [TestClass]
    public class SignInCommandTests
    {
        [TestMethod]
        public void SignInCommand_Run_ReturnsAuthnRequestForDefaultIdp()
        {
            var idp = Options.FromConfiguration.IdentityProviders.Default;
            var defaultDestination = idp.SingleSignOnServiceUrl;

            var result = new SignInCommand().Run(
                new HttpRequestData("GET", new Uri("http://example.com")),
                Options.FromConfiguration);

            result.HttpStatusCode.Should().Be(HttpStatusCode.SeeOther);
            result.Cacheability.Should().Be((Cacheability)HttpCacheability.NoCache);
            result.Location.Host.Should().Be(defaultDestination.Host);

            var queries = HttpUtility.ParseQueryString(result.Location.Query);

            queries.Should().HaveCount(2);
            queries["SAMLRequest"].Should().NotBeEmpty();
            queries["RelayState"].Should().NotBeEmpty();
        }

        [TestMethod]
        public void SignInCommand_Run_MapsReturnUrl()
        {
            var defaultDestination = Options.FromConfiguration.IdentityProviders.Default.SingleSignOnServiceUrl;

            var httpRequest = new HttpRequestData("GET", new Uri("http://localhost/signin?ReturnUrl=%2FReturn.aspx"));

            var actual = new SignInCommand().Run(httpRequest, Options.FromConfiguration);

            actual.RequestState.ReturnUrl.Should().Be("/Return.aspx");
        }

        [TestMethod]
        public void SignInCommand_Run_With_Idp2_ReturnsAuthnRequestForSecondIdp()
        {
            var secondIdp = Options.FromConfiguration.IdentityProviders[1];
            var secondDestination = secondIdp.SingleSignOnServiceUrl;
            var secondEntityId = secondIdp.EntityId;

            var request = new HttpRequestData("GET",
                new Uri("http://sp.example.com?idp=" + Uri.EscapeDataString(secondEntityId.Id)));

            var subject = new SignInCommand().Run(request, Options.FromConfiguration);

            subject.Location.Host.Should().Be(secondDestination.Host);
        }

        [TestMethod]
        public void SignInCommand_Run_With_InvalidIdp_ThrowsException()
        {
            var request = new HttpRequestData("GET", new Uri("http://localhost/signin?idp=no-such-idp-in-config"));

            Action a = () => new SignInCommand().Run(request, Options.FromConfiguration);

            a.ShouldThrow<InvalidOperationException>().WithMessage("Unknown idp");
        }

        [TestMethod]
        public void SignInCommand_Run_NullCheckRequest()
        {
            Action a = () => new SignInCommand().Run(null, Options.FromConfiguration);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("request");
        }

        [TestMethod]
        public void SignInCommand_Run_NullCheckOptions()
        {
            Action a = () => new SignInCommand().Run(new HttpRequestData("GET", new Uri("http://localhost")), null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("options");
        }

        [TestMethod]
        public void SignInCommand_Run_ReturnsRedirectToDiscoveryService()
        {
            var dsUrl = new Uri("http://ds.example.com");

            var options = new Options(new SPOptions
                {
                    DiscoveryServiceUrl = dsUrl,
                    EntityId = new EntityId("https://github.com/KentorIT/authservices")
                });

			var relayData = new Dictionary<string, string>()
			{
				{ "test", "value" }
			};

			var request = new HttpRequestData("GET", new Uri("http://localhost/signin?ReturnUrl=%2FReturn%2FPath"));
			request.StoredRequestState = new StoredRequestState(null, null, null, relayData);

            var result = new SignInCommand().Run(request, options);

			result.RequestState.RelayData.Should().Equal(relayData);
			result.SetCookieName.Should().NotBeEmpty();

			result.HttpStatusCode.Should().Be(HttpStatusCode.SeeOther);

			// check result scheme and host
			result.Location.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped).Should().Be(dsUrl.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped));
			result.Location.AbsolutePath.Should().Be(dsUrl.AbsolutePath);

			// check querystring separately
			var resultLocationQueryString = HttpUtility.ParseQueryString(result.Location.Query);
			resultLocationQueryString["entityId"].Should().Be(options.SPOptions.EntityId.Id);
			resultLocationQueryString["returnIDParam"].Should().Be("idp");

			// check return url
			var returnUrl = new Uri(resultLocationQueryString["return"]);
			returnUrl.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped).Should().Be("http://localhost");
			returnUrl.AbsolutePath.Should().Be("/AuthServices/SignIn");

			var returnUrlQueryString = HttpUtility.ParseQueryString(returnUrl.Query);
			returnUrlQueryString["ReturnUrl"].Should().Be("/Return/Path");
			returnUrlQueryString["RelayState"].Should().NotBeEmpty();

		}

        [TestMethod]
        public void SignInCommand_Run_PublicOrigin()
        {
            var options = StubFactory.CreateOptionsPublicOrigin(new Uri("https://my.public.origin:8443"));
            var idp = options.IdentityProviders.Default;

            var request = new HttpRequestData("GET",
                new Uri("http://sp.example.com?idp=" + Uri.EscapeDataString(idp.EntityId.Id)));

            var subject = new SignInCommand().Run(request, Options.FromConfiguration);

            subject.Location.Host.Should().Be(new Uri("https://idp.example.com").Host);
        }

        [TestMethod]
        public void SignInCommand_Run_NullcheckOptions()
        {
            Action a = () => SignInCommand.Run(null, null, null, null, null);

            a.ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("options");
        }

        [TestMethod]
        public void SignInCommand_Run_Calls_Notifications()
        {
            var options = StubFactory.CreateOptions();
            var idp = options.IdentityProviders.Default;
            var relayData = new Dictionary<string, string>();
            options.SPOptions.DiscoveryServiceUrl = null;
           
            var request = new HttpRequestData("GET",
                new Uri("http://sp.example.com"));

            var selectedIdpCalled = false;
            options.Notifications.SelectIdentityProvider =
                (ei, r) =>
            {
                ei.Should().BeSameAs(idp.EntityId);
                r.Should().BeSameAs(relayData);
                selectedIdpCalled = true;
                return null;
            };
            
            var authnRequestCreatedCalled = false;
            options.Notifications.AuthenticationRequestCreated = (a, i, r) => 
                {
                    a.Should().NotBeNull();
                    i.Should().BeSameAs(idp);
                    r.Should().BeSameAs(relayData);
                    authnRequestCreatedCalled = true;
                };

            CommandResult notifiedCommandResult = null;
            options.Notifications.SignInCommandResultCreated = (cr, r) =>
                {
                    notifiedCommandResult = cr;
                    r.Should().BeSameAs(relayData);                    
                };

            SignInCommand.Run(idp.EntityId, null, request, options, relayData)
                .Should().BeSameAs(notifiedCommandResult);

            authnRequestCreatedCalled.Should().BeTrue("the AuthenticationRequestCreated notification should have been called");
            selectedIdpCalled.Should().BeTrue("the SelectIdentityProvider notification should have been called.");
        }

        [TestMethod]
        public void SignInCommand_Run_Uses_IdpFromNotification()
        {
            var options = StubFactory.CreateOptions();
            var idp = options.IdentityProviders.Default;
            var entityId = new EntityId("urn:invalid");
            options.SPOptions.DiscoveryServiceUrl.Should().NotBeNull("this test assumes a non-null DS url");

            var request = new HttpRequestData("GET",
                new Uri("http://sp.example.com"));

            options.Notifications.SelectIdentityProvider = (ei, r) =>
            {
                return idp;
            };

            var authnRequestCreatedCalled = false;
            options.Notifications.AuthenticationRequestCreated = (a, i, r) =>
            {
                authnRequestCreatedCalled = true;
                i.Should().BeSameAs(idp, "the idp from the SelectIdentityProvider notification should override the default behaviour");
            };

            SignInCommand.Run(entityId, null, request, options, null);

            authnRequestCreatedCalled.Should().BeTrue("an AuthenticateRequest should have been created instead of going to the Discovery Service.");
        }

        [TestMethod]
        public void SignInCommand_Run_Calls_CommandResultCreated_OnRedirectToDS()
        {
            var options = StubFactory.CreateOptions();
            var idp = options.IdentityProviders.Default;
            options.SPOptions.DiscoveryServiceUrl.Should().NotBeNull("this test assumes a non-null DS url");

            var request = new HttpRequestData("GET",
                new Uri("http://sp.example.com"));

            CommandResult notifiedCommandResult = null;
            options.Notifications.SignInCommandResultCreated = (cr, r) =>
            {
                notifiedCommandResult = cr;
            };

            SignInCommand.Run(null, null, request, options, null)
                .Should().BeSameAs(notifiedCommandResult);
        }
    }
}