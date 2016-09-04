using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Owin;
using FluentAssertions;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kentor.AuthServices.Tests.Helpers;
using System.IO;
using System.Text;
using System.Security.Claims;
using Kentor.AuthServices.Configuration;
using System.Net.Http;
using NSubstitute;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens;
using System.IdentityModel.Metadata;
using System.Reflection;
using System.Threading;
using Kentor.AuthServices.Saml2P;
using Kentor.AuthServices.WebSso;
using System.Security.Principal;
namespace Kentor.AuthServices.Tests.Owin
{
    using Microsoft.Owin.Security.DataProtection;
    using System.Configuration;
    using AuthenticateDelegate = Func<string[], Action<IIdentity, IDictionary<string, string>, IDictionary<string, object>, object>, object, Task>;

    [TestClass]
    public class KentorAuthServicesAuthenticationMiddlewareTests
    {
        ClaimsPrincipal originalPrincipal;

        [TestInitialize]
        public void Initialize()
        {
            originalPrincipal = ClaimsPrincipal.Current;
        }

        [TestCleanup]
        public void Cleanup()
        {
            Thread.CurrentPrincipal = originalPrincipal;
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationMiddleware_CtorNullChecksOptions()
        {
            Action a = () => new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(0, null), CreateAppBuilder(),
                null);

            a.ShouldThrow<ArgumentNullException>("options");
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationMiddleware_CtorNullChecksApp()
        {
            Action a = () => new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(0, null), null, new KentorAuthServicesAuthenticationOptions(true));

            a.ShouldThrow<ArgumentNullException>("app");
        }

        const string DefaultSignInAsAuthenticationType = "MyDefaultSignInAsAuthTypeForTesting";

        private static IAppBuilder CreateAppBuilder()
        {
            var app = Substitute.For<IAppBuilder>();
            app.Properties.Returns(new Dictionary<string, object>()
            {
                { "host.AppName", "ABCD1234" }
            });
            app.SetDefaultSignInAsAuthenticationType(DefaultSignInAsAuthenticationType);
            return app;
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationMiddleware_CtorSetsDefaultAuthOption()
        {
            var options = new KentorAuthServicesAuthenticationOptions(true);

            options.SignInAsAuthenticationType.Should().BeNull();

            var middleware = new KentorAuthServicesAuthenticationMiddleware(new StubOwinMiddleware(0, null),
                CreateAppBuilder(), options);

            options.SignInAsAuthenticationType.Should().Be(DefaultSignInAsAuthenticationType);
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_RedirectsOnSpecificAuthChallenge_WhenPassive()
        {
            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401, new AuthenticationResponseChallenge(
                    new string[] { "KentorAuthServices" }, null)), CreateAppBuilder(),
                new KentorAuthServicesAuthenticationOptions(true)
                { AuthenticationMode = AuthenticationMode.Passive });

            var context = OwinTestHelpers.CreateOwinContext();

            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(303);
            context.Response.Headers["Location"].Should().StartWith("https://idp.example.com/idp");
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_DoesntRedirectOnUnSpecificAuthChallenge_WhenPassive()
        {
            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401, new AuthenticationResponseChallenge(
                    new string[0], null)), CreateAppBuilder(),
                new KentorAuthServicesAuthenticationOptions(true)
                { AuthenticationMode = AuthenticationMode.Passive });

            var context = OwinTestHelpers.CreateOwinContext();

            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(401);
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_RedirectsOnAuthChallenge_WhenActive()
        {
            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401, new AuthenticationResponseChallenge(
                    new string[0], null)), CreateAppBuilder(),
                new KentorAuthServicesAuthenticationOptions(true)
                { AuthenticationMode = AuthenticationMode.Active });

            var context = OwinTestHelpers.CreateOwinContext();

            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(303);
            context.Response.Headers["Location"].Should().StartWith("https://idp.example.com/idp");
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_RedirectToIdp_HonorsCommandResultHandled()
        {
            var options = new KentorAuthServicesAuthenticationOptions(true)
            {
                AuthenticationMode = AuthenticationMode.Active,
                Notifications = new KentorAuthServicesNotifications
                {
                    SignInCommandResultCreated = (cr, r) =>
                    {
                        cr.HandledResult = true;
                    }
                }
            };

            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401, new AuthenticationResponseChallenge(
                    new string[0], null)), CreateAppBuilder(),
                options);

            var context = OwinTestHelpers.CreateOwinContext();

            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(401);
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_CreatesPostOnAuthChallenge()
        {
            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401, new AuthenticationResponseChallenge(
                    new string[] { "KentorAuthServices" }, new AuthenticationProperties(
                        new Dictionary<string, string>()
                        {
                            { "idp", "http://localhost:13428/idpMetadata" }
                        }))),
                CreateAppBuilder(),
                new KentorAuthServicesAuthenticationOptions(true));

            var context = OwinTestHelpers.CreateOwinContext();

            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(200);
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            // Fix to #295, where content length is incorrectly set to 0 by the
            // next middleware. It appears as it works if the content length is
            // simply removed. See discussion in GitHub issue #295.
            context.Response.ContentLength.Should().NotHaveValue();

            using (var reader = new StreamReader(context.Response.Body))
            {
                string bodyContent = reader.ReadToEnd();

                // Checking some random stuff in body to make sure it looks like a SAML Post.
                bodyContent.Should().Contain("<form action");
                bodyContent.Should().Contain("<input type=\"hidden\" name=\"SAMLRequest\"");
            }
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_CreatesRedirectOnAuthRevoke()
        {
            var revoke = new AuthenticationResponseRevoke(new string[0]);

            var options = new KentorAuthServicesAuthenticationOptions(true);
            options.SPOptions.PublicOrigin = new Uri("https://sp.example.com/ExternalPath/");

            var subject = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(200, revoke: revoke),
                CreateAppBuilder(),
                options);

            var context = OwinTestHelpers.CreateOwinContext();
            context.Request.Scheme = "http";
            context.Request.Host = new HostString("sp-internal.example.com");
            context.Request.PathBase = new PathString("/InternalPath");
            context.Request.Path = new PathString("/LoggedOut");
            context.Request.User = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(AuthServicesClaimTypes.LogoutNameIdentifier, ",,,,NameId", null, "https://idp.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }, "Federation"));

            await subject.Invoke(context);

            context.Response.StatusCode.Should().Be(303);
            context.Response.Headers["Location"].Should().StartWith("https://idp.example.com/logout?SAMLRequest");
            var returnUrl = ExtractRequestState(options.DataProtector, context).ReturnUrl;

            returnUrl.Should().Be("/LoggedOut");
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_AuthRevoke_HonorsCommandResultHandled()
        {
            var revoke = new AuthenticationResponseRevoke(new string[0]);

            var options = new KentorAuthServicesAuthenticationOptions(true)
            {
                Notifications = new KentorAuthServicesNotifications
                {
                    LogoutCommandResultCreated = cr =>
                    {
                        cr.HandledResult = true;
                    }
                }
            };

            var subject = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(200, revoke: revoke),
                CreateAppBuilder(),
                options);

            var context = OwinTestHelpers.CreateOwinContext();
            context.Request.Scheme = "http";
            context.Request.Host = new HostString("sp-internal.example.com");
            context.Request.PathBase = new PathString("/InternalPath");
            context.Request.Path = new PathString("/LoggedOut");
            context.Request.User = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "NameId", null, "https://idp.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }, "Federation"));

            await subject.Invoke(context);

            context.Response.StatusCode.Should().Be(200);
        }

        private static StoredRequestState ExtractRequestState(IDataProtector dataProtector, OwinContext context)
        {
            var cookieData = context.Response.Headers["Set-Cookie"].Split(';', '=')[1];

            return new StoredRequestState(
                dataProtector.Unprotect(
                    HttpRequestData.GetBinaryData(cookieData)));
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_CreatesRedirectOnAuthRevoke_UsesAuthPropsReturnUrl()
        {
            var authPropsReturnUrl = "http://sp.exmample.com/AuthPropsLogout";

            var revoke = new AuthenticationResponseRevoke(
                new string[0],
                new AuthenticationProperties { RedirectUri = authPropsReturnUrl });

            var options = new KentorAuthServicesAuthenticationOptions(true);
            options.SPOptions.PublicOrigin = new Uri("https://sp.example.com/ExternalPath/");

            var subject = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(303, revoke: revoke),
                CreateAppBuilder(),
                options);

            var context = OwinTestHelpers.CreateOwinContext();
            context.Response.Headers["Location"] = "http://sp.example.com/locationHeader";
            context.Request.User = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(AuthServicesClaimTypes.LogoutNameIdentifier, ",,,,NameId", null, "https://idp.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }, "Federation"));

            await subject.Invoke(context);

            var cookieValue = context.Response.Headers["Set-Cookie"].Split(';', '=')[1];

            var returnUrl = new StoredRequestState(options.DataProtector.Unprotect(
                HttpRequestData.GetBinaryData(cookieValue))).ReturnUrl;

            returnUrl.Should().Be(authPropsReturnUrl);
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_DoesntRedirectOnUnspecifiedAuthRevoke_WhenPassive()
        {
            var subject = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(200, revoke: new AuthenticationResponseRevoke(new string[0])),
                CreateAppBuilder(),
                new KentorAuthServicesAuthenticationOptions(true)
                {
                    AuthenticationMode = AuthenticationMode.Passive
                });

            var context = OwinTestHelpers.CreateOwinContext();

            Thread.CurrentPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "NameId", null, "https://idp.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }, "Federation"));

            await subject.Invoke(context);

            context.Response.StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_CreatesRedirectOnSpecifiedAuthRevoke_WhenPassive()
        {
            var subject = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(200, revoke: new AuthenticationResponseRevoke(
                    new string[] { "KentorAuthServices" })),
                CreateAppBuilder(),
                new KentorAuthServicesAuthenticationOptions(true)
                {
                    AuthenticationMode = AuthenticationMode.Passive
                });

            var context = OwinTestHelpers.CreateOwinContext();

            context.Request.User = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(AuthServicesClaimTypes.LogoutNameIdentifier, ",,,,NameId", null, "https://idp.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }, "Federation"));

            await subject.Invoke(context);

            context.Response.StatusCode.Should().Be(303);
            context.Response.Headers["Location"].Should().StartWith("https://idp.example.com/logout?SAMLRequest");
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_HandlesLogoutResponse()
        {
            var app = CreateAppBuilder();
            var options = new KentorAuthServicesAuthenticationOptions(true);
            var subject = new KentorAuthServicesAuthenticationMiddleware(
                null,
                app,
                options);

            var context = OwinTestHelpers.CreateOwinContext();

            var relayState = "MyRelayState";
            var response = new Saml2LogoutResponse(Saml2StatusCode.Success)
            {
                DestinationUrl = new Uri("https://sp.example.com/AuthServices/Logout"),
                RelayState = relayState,
                SigningCertificate = SignedXmlHelper.TestCert,
                Issuer = new EntityId("https://idp.example.com")
            };
            var requestUri = Saml2Binding.Get(Saml2BindingType.HttpRedirect).Bind(response).Location;

            var cookieData = HttpRequestData.ConvertBinaryData(
                    options.DataProtector.Protect(
                        new StoredRequestState(null, new Uri("http://loggedout.example.com/"), null, null)
                            .Serialize()));
            context.Request.Headers["Cookie"] = $"Kentor.{relayState}={cookieData}";
            context.Request.Path = new PathString(requestUri.AbsolutePath);
            context.Request.QueryString = new QueryString(requestUri.Query.TrimStart('?'));
            
            Thread.CurrentPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "NameId", null, "https://idp.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }, "Federation"));

            await subject.Invoke(context);

            context.Response.StatusCode.Should().Be(303);
            context.Response.Headers["Location"].Should().Be("http://loggedout.example.com/");
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_LogoutsOnLogoutRequest()
        {
            var options = new KentorAuthServicesAuthenticationOptions(true);
            var subject = new KentorAuthServicesAuthenticationMiddleware(null, CreateAppBuilder(), options);

            var context = OwinTestHelpers.CreateOwinContext();

            var request = new Saml2LogoutRequest()
            {
                SessionIndex = "SessionId",
                DestinationUrl = new Uri("http://sp.example.com/AuthServices/Logout"),
                NameId = new Saml2NameIdentifier("NameId"),
                Issuer = new EntityId("https://idp.example.com"),
                SigningCertificate = SignedXmlHelper.TestCert
            };

            var url = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(request).Location;

            context.Request.Path = new PathString(url.AbsolutePath);
            context.Request.QueryString = new QueryString(url.Query.TrimStart('?'));

            await subject.Invoke(context);

            context.Response.StatusCode.Should().Be(303);
            context.Response.Headers["Location"].Should().StartWith("https://idp.example.com/logout?SAMLResponse");

            context.Authentication.AuthenticationResponseRevoke.Should().NotBeNull();
            context.Authentication.AuthenticationResponseRevoke.AuthenticationTypes
                .Should().BeEmpty();
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_LogoutRequest_HonorsCommandResultHandled()
        {
            var options = new KentorAuthServicesAuthenticationOptions(true)
            {
                Notifications = new KentorAuthServicesNotifications
                {
                    LogoutCommandResultCreated = cr =>
                    {
                        cr.HandledResult = true;
                    }
                }
            };

            var subject = new KentorAuthServicesAuthenticationMiddleware(null, CreateAppBuilder(), options);

            var context = OwinTestHelpers.CreateOwinContext();

            var request = new Saml2LogoutRequest()
            {
                SessionIndex = "SessionId",
                DestinationUrl = new Uri("http://sp.example.com/AuthServices/Logout"),
                NameId = new Saml2NameIdentifier("NameId"),
                Issuer = new EntityId("https://idp.example.com"),
                SigningCertificate = SignedXmlHelper.TestCert
            };

            var url = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(request).Location;

            context.Request.Path = new PathString(url.AbsolutePath);
            context.Request.QueryString = new QueryString(url.Query.TrimStart('?'));

            await subject.Invoke(context);

            context.Response.StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_NoRedirectOnNon401()
        {
            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(200, new AuthenticationResponseChallenge(
                    new string[] { "KentorAuthServices" }, null)),
                CreateAppBuilder(),
                new KentorAuthServicesAuthenticationOptions(true));

            var context = OwinTestHelpers.CreateOwinContext();

            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(200);
            context.Response.Headers["Location"].Should().BeNull();
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_NoRedirectWithChallengeOfDifferentType()
        {
            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401, new AuthenticationResponseChallenge(
                        new string[] { "SomeThingElse" }, null)),
                    CreateAppBuilder(),
                    new KentorAuthServicesAuthenticationOptions(true));

            var context = OwinTestHelpers.CreateOwinContext();

            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(401);
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_RedirectToSecondIdp_AuthenticationProperties()
        {
            var secondIdp = Options.FromConfiguration.IdentityProviders[1];
            var secondDestination = secondIdp.SingleSignOnServiceUrl;
            var secondEntityId = secondIdp.EntityId;

            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401, new AuthenticationResponseChallenge(
                    new string[] { "KentorAuthServices" }, new AuthenticationProperties(
                        new Dictionary<string, string>()
                        {
                            { "idp", secondEntityId.Id }
                        }))),
                        CreateAppBuilder(), new KentorAuthServicesAuthenticationOptions(true));

            var context = OwinTestHelpers.CreateOwinContext();
            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(303);
            context.Response.Headers["Location"].Should().StartWith(secondDestination.ToString());
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_RedirectToSecondIdp_OwinEnvironment()
        {
            var secondIdp = Options.FromConfiguration.IdentityProviders[1];
            var secondDestination = secondIdp.SingleSignOnServiceUrl;
            var secondEntityId = secondIdp.EntityId;

            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401, new AuthenticationResponseChallenge(
                    new string[] { "KentorAuthServices" }, new AuthenticationProperties())),
                        CreateAppBuilder(), new KentorAuthServicesAuthenticationOptions(true));

            var context = OwinTestHelpers.CreateOwinContext();
            context.Environment["KentorAuthServices.idp"] = secondEntityId;
            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(303);
            context.Response.Headers["Location"].Should().StartWith(secondDestination.ToString());
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_RedirectOnChallengeForAuthTypeInOptions()
        {
            var authenticationType = "someAuthName";

            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401, new AuthenticationResponseChallenge(
                    new string[] { authenticationType }, null)),
                CreateAppBuilder(),
                new KentorAuthServicesAuthenticationOptions(true)
                {
                    AuthenticationType = authenticationType
                });

            var context = OwinTestHelpers.CreateOwinContext();

            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(303);
            context.Response.Headers["Location"].Should().StartWith("https://idp.example.com/idp");
        }

        private string ExtractRelayState(OwinContext context)
        {
            return HttpUtility.ParseQueryString(
                new Uri(context.Response.Headers["Location"])
                .Query)["RelayState"];
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_RedirectRemembersReturnPath()
        {
            var returnUrl = "http://sp.example.com/returnurl";

            var options = new KentorAuthServicesAuthenticationOptions(true);
            var subject = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401, new AuthenticationResponseChallenge(
                    new string[] { "KentorAuthServices" }, new AuthenticationProperties()
                    {
                        RedirectUri = returnUrl
                    })),
                    CreateAppBuilder(), options);

            var context = OwinTestHelpers.CreateOwinContext();

            await subject.Invoke(context);

            var storedState = ExtractRequestState(options.DataProtector, context);

            storedState.ReturnUrl.Should().Be(returnUrl);
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_StoresAuthenticationProperties()
        {
            var returnUrl = "http://sp.example.com/returnurl";

            var prop = new AuthenticationProperties()
            {
                RedirectUri = returnUrl
            };
            prop.Dictionary["test"] = "SomeValue";

            var options = new KentorAuthServicesAuthenticationOptions(true);
            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401,
                    new AuthenticationResponseChallenge(
                        new string[] { "KentorAuthServices" }, prop)),
                CreateAppBuilder(),
                options);

            var context = OwinTestHelpers.CreateOwinContext();

            await middleware.Invoke(context);

            var storedAuthnData = ExtractRequestState(options.DataProtector, context);

            new AuthenticationProperties(storedAuthnData.RelayData).Dictionary["test"].Should().Be("SomeValue");
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_RestoresAuthenticationPropertiesFromRelayData()
        {
            var returnUrl = "http://sp.example.com/returnurl";

            var prop = new AuthenticationProperties()
            {
                RedirectUri = returnUrl
            };

            var options = new KentorAuthServicesAuthenticationOptions(true);
            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401,
                    new AuthenticationResponseChallenge(
                        new string[] { "KentorAuthServices" }, prop)),
                CreateAppBuilder(),
                options);

            var context = OwinTestHelpers.CreateOwinContext();

            var testData = new Dictionary<string, string>() { { "test", "SomeValue" } };
            var state = new StoredRequestState(null, null, null, testData);
            context.Request.QueryString = new QueryString("RelayState", "RelayStateValue");
            context.Request.Headers.Add("Cookie", new string[] { "Kentor.RelayStateValue=" + HttpRequestData.ConvertBinaryData(options.DataProtector.Protect(state.Serialize())) });

            await middleware.Invoke(context);

            context.Authentication.AuthenticationResponseChallenge.Properties.Dictionary.Should().Equal(testData);
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_AcsUsesCommandResultLocation()
        {
            // For Owin middleware, the redirect uri is part of the
            // authentication properties, but we don't want to use it as it
            // is because it can be empty (e.g. on unsolicited responses
            // or until #182 is fixed). The redirect uri should be taken
            // from the commandresult location instead.

            var context = OwinTestHelpers.CreateOwinContext();
            context.Request.Method = "POST";

            var response =
                @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0""
                IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>
                    https://idp.example.com
                </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>
            </saml2p:Response>";

            var bodyData = new KeyValuePair<string, string>[] { 
                new KeyValuePair<string, string>("SAMLResponse", 
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(SignedXmlHelper.SignXml(response))))
            };

            var encodedBodyData = new FormUrlEncodedContent(bodyData);

            context.Request.Body = encodedBodyData.ReadAsStreamAsync().Result;
            context.Request.ContentType = encodedBodyData.Headers.ContentType.ToString();
            context.Request.Host = new HostString("localhost");
            context.Request.Path = new PathString("/AuthServices/Acs");

            var middleware = new KentorAuthServicesAuthenticationMiddleware(null, CreateAppBuilder(),
                new KentorAuthServicesAuthenticationOptions(true)
                {
                    SignInAsAuthenticationType = "AuthType"
                });

            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(303);
            context.Response.Headers["Location"].Should().Be("http://localhost/LoggedIn");
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_AcsWorks()
        {
            var context = OwinTestHelpers.CreateOwinContext();
            context.Request.Method = "POST";

            var authProps = new AuthenticationProperties()
            {
                IssuedUtc = new DateTime(1975, 05, 05, 05, 05, 05, DateTimeKind.Utc)
            };
            authProps.Dictionary["Test"] = "TestValue";

            var state = new StoredRequestState(new EntityId("https://idp.example.com"),
                new Uri("http://localhost/LoggedIn"),
                new Saml2Id("InResponseToId"),
                authProps.Dictionary);

            var relayState = SecureKeyGenerator.CreateRelayState();

            var cookieData = HttpRequestData.ConvertBinaryData(
                CreateAppBuilder().CreateDataProtector(
                    typeof(KentorAuthServicesAuthenticationMiddleware).FullName)
                    .Protect(state.Serialize()));

            context.Request.Headers["Cookie"] = $"Kentor.{relayState}={cookieData}";

            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0""
                IssueInstant=""2013-01-01T00:00:00Z"" InResponseTo=""InResponseToId"" >
                <saml2:Issuer>
                    https://idp.example.com
                </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>
            </saml2p:Response>";

            var bodyData = new KeyValuePair<string, string>[] { 
                new KeyValuePair<string, string>("SAMLResponse", 
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(SignedXmlHelper.SignXml(response)))),
                new KeyValuePair<string, string>("RelayState",relayState)
            };

            var encodedBodyData = new FormUrlEncodedContent(bodyData);

            context.Request.Body = encodedBodyData.ReadAsStreamAsync().Result;
            context.Request.ContentType = encodedBodyData.Headers.ContentType.ToString();
            context.Request.Host = new HostString("localhost");
            context.Request.Path = new PathString("/AuthServices/Acs");

            var signInAsAuthenticationType = "AuthType";
            var ids = new ClaimsIdentity[] { new ClaimsIdentity(signInAsAuthenticationType),
                new ClaimsIdentity(signInAsAuthenticationType) };
            ids[0].AddClaim(new Claim(ClaimTypes.NameIdentifier, "SomeUser", null, "https://idp.example.com"));
            ids[1].AddClaim(new Claim(ClaimTypes.Role, "RoleFromClaimsAuthManager", 
                null, "ClaimsAuthenticationManagerStub"));

            var subject = new KentorAuthServicesAuthenticationMiddleware(null, CreateAppBuilder(),
                StubFactory.CreateOwinOptions());

            await subject.Invoke(context);

            context.Response.StatusCode.Should().Be(303);
            context.Response.Headers["Location"].Should().Be("http://localhost/LoggedIn");
            context.Response.Headers["Set-Cookie"].Should().Be($"Kentor.{relayState}=; path=/; expires=Thu, 01-Jan-1970 00:00:00 GMT");

            context.Authentication.AuthenticationResponseGrant.Principal.Identities
                .ShouldBeEquivalentTo(ids, opt => opt.IgnoringCyclicReferences());

            context.Authentication.AuthenticationResponseGrant.Properties.RedirectUri
                .Should().Be("http://localhost/LoggedIn", 
                "the StoredRequestState.ReturnUrl should overtake the value in the AuthProperties and be stored in the AuthProps");

            context.Authentication.AuthenticationResponseGrant.Properties.Dictionary["Test"]
                .Should().Be("TestValue");

            context.Authentication.AuthenticationResponseGrant.Properties.IssuedUtc
                .Should().Be(authProps.IssuedUtc);

            context.Authentication.AuthenticationResponseGrant.Properties.AllowRefresh.Should().NotHaveValue("AllowRefresh should not be specified if no SessionOnOrAfter is specified");
            context.Authentication.AuthenticationResponseGrant.Properties.ExpiresUtc.Should().NotHaveValue("ExpiresUtc should not be set if no SessionNotOnOrAfter is specified");
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_Acs_HonorsCommandResultHandled()
        {
            var context = OwinTestHelpers.CreateOwinContext();
            context.Request.Method = "POST";

            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0""
                IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>
                    https://idp.example.com
                </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>
            </saml2p:Response>";

            var bodyData = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("SAMLResponse",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(SignedXmlHelper.SignXml(response)))),
            };

            var encodedBodyData = new FormUrlEncodedContent(bodyData);

            context.Request.Body = encodedBodyData.ReadAsStreamAsync().Result;
            context.Request.ContentType = encodedBodyData.Headers.ContentType.ToString();
            context.Request.Host = new HostString("localhost");
            context.Request.Path = new PathString("/AuthServices/Acs");

            var options = StubFactory.CreateOwinOptions();
            options.Notifications.AcsCommandResultCreated = (cr, r) =>
            {
                cr.HandledResult = true;
            };

            var subject = new KentorAuthServicesAuthenticationMiddleware(
                null, CreateAppBuilder(), options);

            await subject.Invoke(context);

            context.Response.StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_Acs_HonorsSessionNotOnOrAfter()
        {
            var context = OwinTestHelpers.CreateOwinContext();
            context.Request.Method = "POST";

            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0""
                IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>
                    https://idp.example.com
                </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + $@"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                    <saml2:AuthnStatement AuthnInstant=""{DateTime.UtcNow.ToSaml2DateTimeString()}"" SessionNotOnOrAfter=""2050-01-01T00:00:00Z"">
                        <saml2:AuthnContext>
                            <saml2:AuthnContextClassRef>urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport</saml2:AuthnContextClassRef>
                        </saml2:AuthnContext>
                    </saml2:AuthnStatement>
                </saml2:Assertion>
            </saml2p:Response>";

            var bodyData = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("SAMLResponse",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(SignedXmlHelper.SignXml(response)))),
            };

            var encodedBodyData = new FormUrlEncodedContent(bodyData);

            context.Request.Body = encodedBodyData.ReadAsStreamAsync().Result;
            context.Request.ContentType = encodedBodyData.Headers.ContentType.ToString();
            context.Request.Host = new HostString("localhost");
            context.Request.Path = new PathString("/AuthServices/Acs");

            var options = StubFactory.CreateOwinOptions();

            var subject = new KentorAuthServicesAuthenticationMiddleware(
                null, CreateAppBuilder(), options);

            await subject.Invoke(context);

            context.Authentication.AuthenticationResponseGrant.Properties
                .AllowRefresh.Should().BeFalse("AllowRefresh should be false if SessionNotOnOrAfter is specified");
            context.Authentication.AuthenticationResponseGrant.Properties
                .ExpiresUtc.Should().BeCloseTo(
                new DateTimeOffset(2050, 1, 1, 0, 0, 0, new TimeSpan(0)),
                because: "SessionNotOnOrAfter should be honored.");
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_MetadataWorks()
        {
            var context = OwinTestHelpers.CreateOwinContext();
            context.Request.Host = new HostString("localhost");
            context.Request.Path = new PathString("/AuthServices");

            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                null,
                CreateAppBuilder(),
                new KentorAuthServicesAuthenticationOptions(true));

            await middleware.Invoke(context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            context.Response.ContentType.Should().Contain("application/samlmetadata+xml");

            var xmlData = XDocument.Load(context.Response.Body);

            xmlData.Document.Root.Name.Should().Be(Saml2Namespaces.Saml2Metadata + "EntityDescriptor");
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_SignInUrlRedirectsToIdp()
        {
            var context = OwinTestHelpers.CreateOwinContext();
            context.Request.Host = new HostString("localhost");
            var signinPath = "/AuthServices/SignIn";
            context.Request.Path = new PathString(signinPath);
            context.Request.QueryString = new QueryString("ReturnUrl=%2FHome&idp=https%3A%2F%2Fidp2.example.com");

            var options = new KentorAuthServicesAuthenticationOptions(true);
            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                null, CreateAppBuilder(), options);

            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(303);
            context.Response.Headers["Location"].Should().StartWith("https://idp2.example.com/idp?SAMLRequest");

            var relayState = ExtractRelayState(context);

            var storedAuthnData = ExtractRequestState(options.DataProtector, context);

            storedAuthnData.ReturnUrl.Should().Be("/Home");
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationMiddleware_WorksOnNullDiscoveryResponseUrl()
        {
            var context = OwinTestHelpers.CreateOwinContext();

            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(200, null),
                CreateAppBuilder(),
                new KentorAuthServicesAuthenticationOptions(false)
                {
                    SPOptions = new SPOptions()
                    {
                        EntityId = new EntityId("http://localhost/metadata")
                    }
                });

            Func<Task> f = async () => await middleware.Invoke(context);

            f.ShouldNotThrow();
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_AugmentsGeneratedClaimsWithLogoutInfo()
        {
            var context = OwinTestHelpers.CreateOwinContext();

            string[] specifiedAuthTypes = null;

            string logoutInfoClaimValue = ",,urn:format,,Saml2NameId";

            // Emulate the external cookie middleware.
            context.Set<AuthenticateDelegate>("security.Authenticate",
                (authTypes, callback, state) =>
                {
                    specifiedAuthTypes = authTypes;
                    var logoutInfoClaim = new Claim(
                        AuthServicesClaimTypes.LogoutNameIdentifier,
                        logoutInfoClaimValue,
                        null,
                        "http://idp.example.com");

                    callback(new ClaimsIdentity(new Claim[]
                        {
                            logoutInfoClaim,
                            new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "http://idp.example.com"),
                            new Claim(ClaimTypes.Role, "SomeRole", null, "http://idp.example.com")
                        }, "Federation"),
                        new Dictionary<string, string>(),
                        new Dictionary<string, object>(),
                        state);                       
                    return Task.FromResult(0);
                });

            var options = new KentorAuthServicesAuthenticationOptions(true);

            var subject = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(303, grant: new AuthenticationResponseGrant(
                    new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "ApplicationNameId")
                    }, "ApplicationIdentity"), new AuthenticationProperties())),
                CreateAppBuilder(),
                options);

            await subject.Invoke(context);

            specifiedAuthTypes.Should().HaveCount(1)
                .And.Subject.Single().Should().Be(DefaultSignInAsAuthenticationType);

            var expected = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "ApplicationNameId"),
                new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "http://idp.example.com"),
                new Claim(AuthServicesClaimTypes.LogoutNameIdentifier, logoutInfoClaimValue, null, "http://idp.example.com")
        }, "ApplicationIdentity");

            context.Authentication.AuthenticationResponseGrant.Identity
                .ShouldBeEquivalentTo(expected, opt => opt.IgnoringCyclicReferences());
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_DoesntAugmentsGeneratedClaimsWhenSessionIndexIsMissing()
        {
            var context = OwinTestHelpers.CreateOwinContext();

            string[] specifiedAuthTypes = null;

            context.Set<AuthenticateDelegate>("security.Authenticate",
                (authTypes, callback, state) =>
                {
                    specifiedAuthTypes = authTypes;
                    callback(new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, "Saml2NameId", null, "http://idp.example.com"),
                            new Claim(ClaimTypes.Role, "SomeRole", null, "http://idp.example.com")
                        }, "Federation"),
                        new Dictionary<string, string>(),
                        new Dictionary<string, object>(),
                        state);
                    return Task.FromResult(0);
                });

            var options = new KentorAuthServicesAuthenticationOptions(true);

            var subject = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(303, grant: new AuthenticationResponseGrant(
                    new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "ApplicationNameId")
                    }, "ApplicationIdentity"), new AuthenticationProperties())),
                CreateAppBuilder(),
                options);

            await subject.Invoke(context);

            specifiedAuthTypes.Should().HaveCount(1)
                .And.Subject.Single().Should().Be(DefaultSignInAsAuthenticationType);

            var expected = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "ApplicationNameId"),
            }, "ApplicationIdentity");

            context.Authentication.AuthenticationResponseGrant.Identity
                .ShouldBeEquivalentTo(expected, opt => opt.IgnoringCyclicReferences());
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_DoesntAugmentsGeneratedClaimsWhenNameIdIsMissing()
        {
            var context = OwinTestHelpers.CreateOwinContext();

            string[] specifiedAuthTypes = null;

            context.Set<AuthenticateDelegate>("security.Authenticate",
                (authTypes, callback, state) =>
                {
                    specifiedAuthTypes = authTypes;
                    callback(new ClaimsIdentity(new Claim[]
                        {
                            new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "http://idp.example.com"),
                            new Claim(ClaimTypes.Role, "SomeRole", null, "http://idp.example.com")
                        }, "Federation"),
                        new Dictionary<string, string>(),
                        new Dictionary<string, object>(),
                        state);
                    return Task.FromResult(0);
                });

            var options = new KentorAuthServicesAuthenticationOptions(true);

            var subject = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(303, grant: new AuthenticationResponseGrant(
                    new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "ApplicationNameId")
                    }, "ApplicationIdentity"), new AuthenticationProperties())),
                CreateAppBuilder(),
                options);

            await subject.Invoke(context);

            specifiedAuthTypes.Should().HaveCount(1)
                .And.Subject.Single().Should().Be(DefaultSignInAsAuthenticationType);

            var expected = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "ApplicationNameId"),
            }, "ApplicationIdentity");

            context.Authentication.AuthenticationResponseGrant.Identity
                .ShouldBeEquivalentTo(expected, opt => opt.IgnoringCyclicReferences());
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationMiddleware_Ctor_NullCheckOptionsSpOptions()
        {
            var options = new KentorAuthServicesAuthenticationOptions(false);

            Action a = () => new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(404),
                CreateAppBuilder(),
                options);

            a.ShouldThrow<ConfigurationErrorsException>()
                .WithMessage("The options.SPOptions property cannot be null. There is an implementation class Kentor.AuthServices.Configuration.SPOptions that you can instantiate. The EntityId property of that class is mandatory. It must be set to the EntityId used to represent this system.");
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationMiddleware_Ctor_NullCheckOptionsSpOptionsEntityId()
        {
            var options = new KentorAuthServicesAuthenticationOptions(false)
            {
                SPOptions = new SPOptions()
            };

            Action a = () => new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(404),
                CreateAppBuilder(),
                options);

            a.ShouldThrow<ConfigurationErrorsException>()
                .WithMessage("The SPOptions.EntityId property cannot be null. It must be set to the EntityId used to represent this system.");

        }
    }
}
