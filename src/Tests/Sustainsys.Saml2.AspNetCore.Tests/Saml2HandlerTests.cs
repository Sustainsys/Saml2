using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Sustainsys.Saml2.Bindings;
using Sustainsys.Saml2.Xml;
using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Samlp;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Xml;

namespace Sustainsys.Saml2.AspNetCore.Tests;
public class Saml2HandlerTests
{
    private readonly static DateTimeOffset CurrentFakeTime = new(2023, 09, 08, 14, 53, 02, TimeSpan.Zero);

    private static async Task<(Saml2Handler subject, HttpContext httpContext)> CreateSubject(Saml2Options options)
    {
        var optionsMonitor = Substitute.For<IOptionsMonitor<Saml2Options>>();
        optionsMonitor.Get(Arg.Any<string>()).Returns(options);

        var loggerFactory = Substitute.For<ILoggerFactory>();

        var systemClock = Substitute.For<ISystemClock>();
        systemClock.UtcNow.Returns(CurrentFakeTime);

        var handler = new Saml2Handler(
            optionsMonitor,
            loggerFactory,
            UrlEncoder.Default,
            systemClock);

        var scheme = new AuthenticationScheme("Saml2", "Saml2", typeof(Saml2Handler));

        var httpContext = Substitute.For<HttpContext>();
        httpContext.Request.Returns(Substitute.For<HttpRequest>());
        httpContext.Request.Scheme = "https";
        httpContext.Request.Host = new HostString("sp.example.com", 8888);
        httpContext.Request.Path = "/path";

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
                EntityId = "https://idp.example.com",
                SsoServiceUrl = "https://idp.example.com/sso"
            }
        };
    }

    private static void ValidateAuthnRequest(AuthnRequest authnRequest)
    {
        // Profile 4.1.4.1: Issuer is required for WebSSO
        authnRequest.Issuer?.Value.Should().Be("https://sp.example.com/Metadata");

        // Core 3.2.1: Issue instant is required
        authnRequest.IssueInstant.Should().Be(CurrentFakeTime);

        // Core 3.4.1 AssertionConsumerServicerUrl is optional, but our StubIdp requires it
        authnRequest.AssertionConsumerServiceUrl.Should().Be("https://sp.example.com:8888/Saml2/Acs");
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
                ValidateAuthnRequest(ctx.AuthnRequest);
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

        await subject.ChallengeAsync(props);

        static void validateLocation(string location)
        {
            var message = new RedirectBinding().UnBindAsync(location);
            var deserializedAuthnRequest = new SamlpSerializer(new SamlSerializer())
                .ReadAuthnRequest(message.Xml.GetXmlTraverser());
        }

        httpContext.Response.Received().Redirect(Arg.Do<string>(validateLocation));
    }

    // TODO: Use event to resolve IdentityProvider - reason enum to differ between challenge and response handling
}