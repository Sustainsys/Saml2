using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Sustainsys.Saml2.AspNetCore;
using Sustainsys.Saml2.Bindings;
using Sustainsys.Saml2.Common;
using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Serialization;
using Sustainsys.Saml2.Tests.Helpers;
using Sustainsys.Saml2.Validation;
using Sustainsys.Saml2.Xml;
using System.Text;
using System.Text.Encodings.Web;

namespace Sustainsys.Saml2.Tests.AspNetCore;
public class Saml2HandlerTests
{
    private readonly static DateTime CurrentFakeTime = new(2023, 09, 08, 14, 53, 02, DateTimeKind.Utc);

    private static async Task<(Saml2Handler subject, HttpContext httpContext)> CreateSubject(Saml2Options options)
    {
        var optionsMonitor = Substitute.For<IOptionsMonitor<Saml2Options>>();
        optionsMonitor.Get(Arg.Any<string>()).Returns(options);

        var loggerFactory = Substitute.For<ILoggerFactory>();

        var keyedServiceProvider = Substitute.For<IKeyedServiceProvider>();
        keyedServiceProvider.GetService(typeof(ISamlXmlReader)).Returns(new SamlXmlReader());
        keyedServiceProvider.GetService(typeof(ISamlXmlWriter)).Returns(new SamlXmlWriter());
        keyedServiceProvider.GetRequiredKeyedService(typeof(IEnumerable<IFrontChannelBinding>), Arg.Any<string>())
            .Returns(Enumerable.Empty<IFrontChannelBinding>());
        keyedServiceProvider.GetService(typeof(IEnumerable<IFrontChannelBinding>)).Returns(
            new IFrontChannelBinding[]
        {
            new HttpRedirectBinding(),
            new HttpPostBinding()
        });

        var handler = new Saml2Handler(
            optionsMonitor,
            loggerFactory,
            UrlEncoder.Default,
            keyedServiceProvider);

        FakeTimeProvider timeProvider = new FakeTimeProvider(new(2025, 05, 28, 11, 14, 53, TimeSpan.Zero));

        keyedServiceProvider.GetService(typeof(IResponseValidator)).Returns(
            new ResponseValidator(new AssertionValidator(timeProvider)));

        keyedServiceProvider.GetService(typeof(IClaimsFactory)).Returns(new ClaimsFactory());

        var authenticationService = Substitute.For<IAuthenticationService>();
        keyedServiceProvider.GetService(typeof(IAuthenticationService)).Returns(authenticationService);

        var scheme = new AuthenticationScheme("Saml2", "Saml2", typeof(Saml2Handler));

        var httpContext = Substitute.For<HttpContext>();
        httpContext.Response.HttpContext.Returns(httpContext);

        httpContext.Request.Scheme = "https";
        httpContext.Request.Host = new HostString("sp.example.com", 8888);
        httpContext.Request.Path = "/Saml2/Acs";

        httpContext.RequestServices.GetService(Arg.Is<Type>(t => t == typeof(IAuthenticationService)))
            .Returns(authenticationService);

        await handler.InitializeAsync(scheme, httpContext);

        return (handler, httpContext);
    }

    private static Saml2Options CreateOptions()
    {
        return new Saml2Options()
        {
            EntityId = "https://sp.example.com/Metadata",
            IdentityProvider = new()
            {
                EntityId = "https://idp.example.com/Saml2",
                SsoServiceUrl = "https://idp.example.com/sso",
                SsoServiceBinding = Constants.BindingUris.HttpRedirect
            },
            TimeProvider = new Microsoft.Extensions.Time.Testing.FakeTimeProvider(CurrentFakeTime),
        };
    }

    [Fact]
    public async Task ChallengeCreatesAuthnRequest()
    {
        var options = CreateOptions();

        bool eventCalled = false;

        options.Events = new()
        {
            // Use event to validate contents of AuthnRequest
            OnAuthnRequestGeneratedAsync = ctx =>
            {
                // Profile 4.1.4.1: Issuer is required for WebSSO
                ctx.AuthnRequest.Issuer?.Value.Should().Be("https://sp.example.com/Metadata");

                // Core 3.2.1: Issue instant is required
                ctx.AuthnRequest.IssueInstant.Should().Be((DateTimeUtc)CurrentFakeTime);

                // Core 3.4.1 AssertionConsumerServicerUrl is optional, but our StubIdp requires it
                ctx.AuthnRequest.AssertionConsumerServiceUrl.Should().Be("https://sp.example.com:8888/Saml2/Acs");

                eventCalled = true;
                return Task.CompletedTask;
            }
        };

        (var subject, var httpContext) = await CreateSubject(options);

        var props = new AuthenticationProperties();

        await subject.ChallengeAsync(props);

        eventCalled.Should().BeTrue("The OnAuthnRequestGeneratedAsync event should have been called");
    }

    [Fact]
    public async Task ChallengeSetsRedirect()
    {
        // This test only validates that the AuthnRequest ends up as a redirect. The contents of
        // the AuthnRequest are validated in ChallengeCreatesAuthnRequest through the event.

        var options = CreateOptions();

        AuthnRequest? authnRequest = null;

        options.Events = new()
        {
            OnAuthnRequestGeneratedAsync = ctx =>
            {
                authnRequest = ctx.AuthnRequest;
                return Task.CompletedTask;
            }
        };

        (var subject, var httpContext) = await CreateSubject(options);

        var props = new AuthenticationProperties();

        httpContext.Response.Redirect(Arg.Do<string>(validateLocation));

        bool validated = false;

        await subject.ChallengeAsync(props);

        void validateLocation(string location)
        {
            location.Should().StartWith("https://idp.example.com/sso?SAMLRequest=");

            var message = new HttpRedirectBinding().UnBindAsync(location, _ => throw new NotImplementedException()).Result;

            var deserializedAuthnRequest = new SamlXmlReader()
                .ReadAuthnRequest(message.Xml.GetXmlTraverser());

            deserializedAuthnRequest.Should().BeEquivalentTo(authnRequest);

            validated = true;
        }

        httpContext.Response.Received().Redirect(Arg.Any<string>());

        validated.Should().BeTrue("The validation should have been called.");
    }


    [Fact]
    public async Task HandleRemoteAuthenticate_CannotUnbind()
    {
        var options = CreateOptions();
        var (subject, _) = await CreateSubject(options);

        await subject.Invoking(async s => await s.HandleRequestAsync())
            .Should().ThrowAsync<AuthenticationFailureException>()
            .WithInnerException<AuthenticationFailureException, AuthenticationFailureException>()
            .WithMessage("No binding*");
    }


    [Fact]
    // Full happy path test case for a signed response via Http POST binding
    public async Task HandleRemoteAuthenticate()
    {
        var options = CreateOptions();
        var (subject, httpContext) = await CreateSubject(options);

        // Have to match callback path.
        httpContext.Request.Path = "/Saml2/Acs";
        httpContext.Request.Method = "POST";

        var xmlDoc = TestData.GetXmlDocument<Saml2HandlerTests>();

        var encodedResponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlDoc.OuterXml));

        httpContext.Request.Form = new FormCollection(new()
        {
            { "SAMLResponse", new(encodedResponse) }
        });

        var result = await subject.HandleRequestAsync();

        result.Should().BeTrue();
    }

    // TODO: Use event to resolve IdentityProvider - presence of EntityId indicates if challenge or response processing
    // TODO: Event when Xml was created
}