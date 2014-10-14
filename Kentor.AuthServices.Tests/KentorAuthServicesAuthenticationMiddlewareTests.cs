using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Owin;
using FluentAssertions;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kentor.AuthServices.TestHelpers;
using System.IO;
using System.Text;
using System.Security.Claims;
using Kentor.AuthServices.Configuration;
using System.Net.Http;
using NSubstitute;
using System.Xml.Linq;
using System.Xml;
using System.Threading.Tasks;
using System.IdentityModel.Tokens;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class KentorAuthServicesAuthenticationMiddlewareTests
    {
        [TestCleanup]
        public void TestCleanup()
        {
            if(!KentorAuthServicesSection.Current.IsReadOnly())
            {
                KentorAuthServicesSection.Current.DiscoveryServiceResponseUrl = 
                    new Uri("http://localhost/Saml2AuthenticationModule/SignIn");
                KentorAuthServicesSection.Current.AllowConfigEdit(false);
            }
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
                new StubOwinMiddleware(0, null), null, new KentorAuthServicesAuthenticationOptions());

            a.ShouldThrow<ArgumentNullException>("app");
        }

        const string DefaultSignInAsAuthenticationType = "MyDefaultSignAsAuthTypeForTesting";

        private static IAppBuilder CreateAppBuilder()
        {
            var app = Substitute.For<IAppBuilder>();
            app.Properties.Returns(new Dictionary<string, object>());
            app.SetDefaultSignInAsAuthenticationType(DefaultSignInAsAuthenticationType);
            return app;
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationMiddleware_CtorSetsDefaultAuthOption()
        {
            var options = new KentorAuthServicesAuthenticationOptions();

            options.SignInAsAuthenticationType.Should().BeNull();

            var middleware = new KentorAuthServicesAuthenticationMiddleware(new StubOwinMiddleware(0, null),  
                CreateAppBuilder(), options);

            options.SignInAsAuthenticationType.Should().Be(DefaultSignInAsAuthenticationType);
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_RedirectsOnAuthChallenge()
        {
            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401, new AuthenticationResponseChallenge(
                    new string[] { "KentorAuthServices" }, null)), CreateAppBuilder(),
                new KentorAuthServicesAuthenticationOptions());

            var context = OwinTestHelpers.CreateOwinContext();

            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(302);
            context.Response.Headers["Location"].Should().StartWith("https://idp.example.com/idp");
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_NoRedirectOnNon401()
        {
            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(200, new AuthenticationResponseChallenge(
                    new string[] { "KentorAuthServices" }, null)), CreateAppBuilder(),
                new KentorAuthServicesAuthenticationOptions());

            var context = OwinTestHelpers.CreateOwinContext();

            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(200);
            context.Response.Headers["Location"].Should().BeNull();
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_NoRedirectWithoutChallenge()
        {
            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401, null), CreateAppBuilder(),
                new KentorAuthServicesAuthenticationOptions());

            var context = OwinTestHelpers.CreateOwinContext();

            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(401);
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_RedirectoToSecondIdp_AuthenticationProperties()
        {
            var secondIdp = IdentityProvider.ActiveIdentityProviders.Skip(1).First();
            var secondDestination = secondIdp.SingleSignOnServiceUrl;
            var secondEntityId = secondIdp.EntityId;

            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401, new AuthenticationResponseChallenge(
                    new string[] { "KentorAuthServices" }, new AuthenticationProperties(
                        new Dictionary<string, string>()
                        {
                            { "idp", secondEntityId.Id }
                        }))), 
                        CreateAppBuilder(), new KentorAuthServicesAuthenticationOptions());

            var context = OwinTestHelpers.CreateOwinContext();
            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(302);
            context.Response.Headers["Location"].Should().StartWith(secondDestination.ToString());
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_RedirectoToSecondIdp_OwinEnvironment()
        {
            var secondIdp = IdentityProvider.ActiveIdentityProviders.Skip(1).First();
            var secondDestination = secondIdp.SingleSignOnServiceUrl;
            var secondEntityId = secondIdp.EntityId;

            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401, new AuthenticationResponseChallenge(
                    new string[] { "KentorAuthServices" }, new AuthenticationProperties())),
                        CreateAppBuilder(), new KentorAuthServicesAuthenticationOptions());

            var context = OwinTestHelpers.CreateOwinContext();
            context.Environment["KentorAuthServices.idp"] = secondEntityId;
            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(302);
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
                new KentorAuthServicesAuthenticationOptions()
                {
                    AuthenticationType = authenticationType
                });

            var context = OwinTestHelpers.CreateOwinContext();

            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(302);
            context.Response.Headers["Location"].Should().StartWith("https://idp.example.com/idp");
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_RedirectRemembersReturnPath()
        {
            var returnUri = "http://sp.example.com/returnuri";

            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401, new AuthenticationResponseChallenge(
                    new string[] { "KentorAuthServices" }, new AuthenticationProperties()
                    {
                        RedirectUri = returnUri
                    })),
                    CreateAppBuilder(), new KentorAuthServicesAuthenticationOptions());

            var context = OwinTestHelpers.CreateOwinContext();

            await middleware.Invoke(context);

            var requestId = AuthnRequestHelper.GetRequestId(new Uri(context.Response.Headers["Location"]));

            StoredRequestState storedAuthnData;
            PendingAuthnRequests.TryRemove(new Saml2Id(requestId), out storedAuthnData);

            storedAuthnData.ReturnUri.Should().Be(returnUri);
        }

        [NotReRunnable]
        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_AcsWorks()
        {
            var context = OwinTestHelpers.CreateOwinContext();
            context.Request.Method = "POST";

            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = ""KentorAuthServicesAuthenticationMiddleware_AcsWorks"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>
                    https://idp.example.com
                </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion
                Version=""2.0"" ID=""KentorAuthServicesAuthenticationMiddleware_AcsWorks_Assertion1""
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
            context.Request.Path = new PathString("/Saml2AuthenticationModule/acs");

            var signInAsAuthenticationType = "AuthType";
            var ids = new ClaimsIdentity[] { new ClaimsIdentity(signInAsAuthenticationType),
                new ClaimsIdentity(signInAsAuthenticationType) };
            ids[0].AddClaim(new Claim(ClaimTypes.NameIdentifier, "SomeUser", null, "https://idp.example.com"));
            ids[1].AddClaim(new Claim(ClaimTypes.Role, "RoleFromClaimsAuthManager", 
                null, "ClaimsAuthenticationManagerStub"));

            var middleware = new KentorAuthServicesAuthenticationMiddleware(null, CreateAppBuilder(),
                new KentorAuthServicesAuthenticationOptions()
                {
                    SignInAsAuthenticationType = "AuthType"
                });

            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(302);
            context.Response.Headers["Location"].Should().Be("http://localhost/LoggedIn");

            context.Authentication.AuthenticationResponseGrant.Principal.Identities.ShouldBeEquivalentTo(ids,
                opt => opt.IgnoringCyclicReferences());
        }

        [TestMethod]
        public async Task KentorAuthServicesAuthenticationMiddleware_MetadataWorks()
        {
            var context = OwinTestHelpers.CreateOwinContext();
            context.Request.Host = new HostString("localhost");
            var metadataPath = "/SomeMetadataPath";
            context.Request.Path = new PathString(metadataPath);

            var middleware = new KentorAuthServicesAuthenticationMiddleware(null, CreateAppBuilder(),
                new KentorAuthServicesAuthenticationOptions()
                {
                    MetadataPath = new PathString(metadataPath)
                });

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
            var signinPath = "/Saml2AuthenticationModule/SignIn";
            context.Request.Path = new PathString(signinPath);
            context.Request.QueryString = new QueryString("ReturnUrl=%2FHome&idp=https%3A%2F%2Fidp2.example.com");

            var middleware = new KentorAuthServicesAuthenticationMiddleware(null, CreateAppBuilder(),
                new KentorAuthServicesAuthenticationOptions());

            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(303);
            context.Response.Headers["Location"].Should().StartWith("https://idp2.example.com/idp?SAMLRequest");

            var requestId = AuthnRequestHelper.GetRequestId(new Uri(context.Response.Headers["Location"]));

            StoredRequestState storedAuthnData;
            PendingAuthnRequests.TryRemove(new Saml2Id(requestId), out storedAuthnData);

            storedAuthnData.ReturnUri.Should().Be("http://localhost/Home");
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationMiddleware_WorksOnNullDiscoveryResponseUrl()
        {
            var context = OwinTestHelpers.CreateOwinContext();

            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(200, null),
                CreateAppBuilder(),
                new KentorAuthServicesAuthenticationOptions());

            KentorAuthServicesSection.Current.AllowConfigEdit(true);
            KentorAuthServicesSection.Current.DiscoveryServiceResponseUrl = null;

            Func<Task> f = async () => await middleware.Invoke(context);

            f.ShouldNotThrow();
        }
    }
}
