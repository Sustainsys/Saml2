using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
    private readonly static DateTimeUtc CurrentFakeTime = new(2023, 09, 08, 14, 53, 02);
    const string SchemeName = "Saml2";

    private static async Task<(
        Saml2Handler subject,
        HttpContext httpContext)>
        CreateSubject(Saml2Options options)
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

        keyedServiceProvider.GetService(typeof(IValidator<Response, ResponseValidationParameters>)).Returns(
            new ResponseValidator(new AssertionValidator(timeProvider)));

        keyedServiceProvider.GetService(typeof(IClaimsFactory)).Returns(new ClaimsFactory());

        var authenticationService = Substitute.For<IAuthenticationService>();
        keyedServiceProvider.GetService(typeof(IAuthenticationService)).Returns(authenticationService);

        var scheme = new AuthenticationScheme(SchemeName, "Saml 2.0", typeof(Saml2Handler));

        var httpContext = Substitute.For<HttpContext>();
        httpContext.Response.HttpContext.Returns(httpContext);

        httpContext.Request.Scheme = "https";
        httpContext.Request.Host = new HostString("sp.example.com", 8888);
        httpContext.Request.Path = "/Saml2/Acs";
        httpContext.Request.Method = "POST";

        httpContext.RequestServices.GetService(Arg.Is<Type>(t => t == typeof(IAuthenticationService)))
            .Returns(authenticationService);

        await handler.InitializeAsync(scheme, httpContext);

        return (handler, httpContext);
    }

    private static Saml2Options CreateOptions()
    {
        var options = new Saml2Options()
        {
            EntityId = "https://sp.example.com/Metadata",
            IdentityProvider = new()
            {
                EntityId = "https://idp.example.com/Saml2",
                SsoServiceUrl = "https://idp.example.com/sso",
                SsoServiceBinding = Constants.BindingUris.HttpRedirect
            },
            TimeProvider = new FakeTimeProvider(CurrentFakeTime),
            StateCookieManager = Substitute.For<ICookieManager>()
        };

        var postConfigure = new Saml2PostConfigureOptions(new FakeDataProtectionProvider());
        postConfigure.PostConfigure(SchemeName, options);

        return options;
    }

    [Fact]
    public async Task ChallengeAsync()
    {
        var options = CreateOptions();
        var cookieManager = Substitute.For<ICookieManager>();
        options.StateCookieManager = cookieManager;

        AuthnRequest? authnRequest = null;
        AuthenticationProperties? authenticationProperties = null;

        options.Events = new()
        {
            // Use event to validate contents of AuthnRequest
            OnAuthnRequestGeneratedAsync = ctx =>
            {
                // Profile 4.1.4.1: Issuer is required for WebSSO
                ctx.AuthnRequest.Issuer?.Value.Should().Be("https://sp.example.com/Metadata");

                // Core 3.2.1: Issue instant is required
                ctx.AuthnRequest.IssueInstant.Should().Be(CurrentFakeTime);

                // Core 3.4.1 AssertionConsumerServicerUrl is optional, but our StubIdp requires it
                ctx.AuthnRequest.AssertionConsumerServiceUrl.Should().Be("https://sp.example.com:8888/Saml2/Acs");

                authnRequest = ctx.AuthnRequest;
                authenticationProperties = ctx.Properties;

                return Task.CompletedTask;
            }
        };

        (var subject, var httpContext) = await CreateSubject(options);

        var props = new AuthenticationProperties()
        {
            RedirectUri = "https://example.com/redirectUri"
        };

        var idpEntityIdHash = options.IdentityProvider!.EntityId!.Sha256(10);

        httpContext.Response.Redirect(Arg.Do<string>(validateLocation));

        cookieManager.AppendResponseCookie(
            httpContext,
            Arg.Do<string>(validateCookieName),
            Arg.Do<string>(validateCookieContents),
            Arg.Do<CookieOptions>(validateCookieOptions));

        await subject.ChallengeAsync(props);

        void validateLocation(string location)
        {
            location.Should().StartWith("https://idp.example.com/sso?SAMLRequest=");

            var message = new HttpRedirectBinding().UnBindAsync(location, _ => throw new NotImplementedException()).Result;

            message.RelayState.Should().Be($"{idpEntityIdHash}.{authnRequest!.Id}");

            var deserializedAuthnRequest = new SamlXmlReader()
                .ReadAuthnRequest(message.Xml.GetXmlTraverser());

            deserializedAuthnRequest.Should().BeEquivalentTo(authnRequest);
        }

        void validateCookieName(string cookieName)
        {
            var expectedName = "Saml2." + idpEntityIdHash;
            cookieName.Should().Be(expectedName);
        }

        void validateCookieContents(string contents)
        {
            var actualAuthProps = options.StateCookieDataFormat.Unprotect(contents)!;

            // We should store the ID of the request and the Idp we're interacting with.
            actualAuthProps.Items[".reqId"].Should().Be(authnRequest!.Id);
            actualAuthProps.Items[".idp"].Should().Be(options.IdentityProvider!.EntityId);

            actualAuthProps.Should().BeEquivalentTo(authenticationProperties);
        }

        void validateCookieOptions(CookieOptions options)
        {
            options.HttpOnly.Should().BeTrue();
            options.Secure.Should().BeTrue();
            options.Expires.Should().Be(((DateTimeOffset)CurrentFakeTime).AddHours(1));
            options.Path.Should().Be("/Saml2/Acs");
        }

        httpContext.Response.Received().Redirect(Arg.Any<string>());

        cookieManager.Received().AppendResponseCookie(
            Arg.Any<HttpContext>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CookieOptions>());
    }

    [Fact]
    public async Task HandleRemoteAuthenticate_CannotUnbind()
    {
        var options = CreateOptions();
        (var subject, _) = await CreateSubject(options);

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

        var xmlDoc = TestData.GetXmlDocument<Saml2HandlerTests>();

        var encodedResponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlDoc.OuterXml));

        var relayState = "bil3roIxTb.123";
        var cookieName = "Saml2." + relayState.Split('.')[0];

        httpContext.Request.Form = new FormCollection(new()
        {
            { "SAMLResponse", new(encodedResponse) },
            { "RelayState",  relayState }
        });

        var authProps = new AuthenticationProperties();
        authProps.Items[".idp"] = "https://idp.example.com";
        authProps.Items[".reqId"] = "123";

        options.StateCookieManager.GetRequestCookie(httpContext, cookieName)
            .Returns(options.StateCookieDataFormat.Protect(authProps));

        var result = await subject.HandleRequestAsync();

        result.Should().BeTrue();

        options.StateCookieManager.Received()
            .DeleteCookie(httpContext, cookieName, Arg.Any<CookieOptions>());
    }

    [Fact]
    public async Task HandleRemoteAsync_RejectsStoredInResponseDoesntMatchState()
    {
        var options = CreateOptions();
        var (subject, httpContext) = await CreateSubject(options);

        var xmlDoc = TestData.GetXmlDocument<Saml2HandlerTests>();

        var encodedResponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlDoc.OuterXml));

        var relayState = "bil3roIxTb.123";
        var cookieName = "Saml2." + relayState.Split('.')[0];

        httpContext.Request.Form = new FormCollection(new()
        {
            { "SAMLResponse", new(encodedResponse) },
            { "RelayState",  relayState }
        });

        var authProps = new AuthenticationProperties();
        authProps.Items[".idp"] = "https://idp.example.com";
        authProps.Items[".reqId"] = "456";

        options.StateCookieManager.GetRequestCookie(httpContext, cookieName)
            .Returns(options.StateCookieDataFormat.Protect(authProps));

        await subject.Invoking(s => s.HandleRequestAsync())
            .Should().ThrowAsync<AuthenticationFailureException>()
            .WithInnerException<AuthenticationFailureException, ValidationException<Saml2Message>>()
            .WithMessage("*InResponseTo*");
    }

    [Fact]
    public async Task HandleRemoteAsync_RejectsStoredIssuerDoesntMatchState()
    {
        var options = CreateOptions();
        var (subject, httpContext) = await CreateSubject(options);

        var xmlDoc = TestData.GetXmlDocument<Saml2HandlerTests>();

        var encodedResponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlDoc.OuterXml));

        httpContext.Request.Form = new FormCollection(new()
        {
            { "SAMLResponse", new(encodedResponse) },
            { "RelayState", "abc.123" }
        });

        var authProps = new AuthenticationProperties();
        authProps.Items[".idp"] = "https://idp.example.com";

        options.StateCookieManager.GetRequestCookie(httpContext, "Saml2.abc")
            .Returns(options.StateCookieDataFormat.Protect(authProps));

        await subject.Invoking(s => s.HandleRequestAsync())
            .Should().ThrowAsync<AuthenticationFailureException>()
            .WithInnerException<AuthenticationFailureException, ValidationException<Saml2Message>>()
            .WithMessage("*RelayState*Idp*");
    }

    [Fact]
    public async Task HandleRemoteAsync_RejectsOnMissingStateCookie()
    {
        var options = CreateOptions();
        var (subject, httpContext) = await CreateSubject(options);

        var xmlDoc = TestData.GetXmlDocument<Saml2HandlerTests>();

        var encodedResponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlDoc.OuterXml));

        httpContext.Request.Form = new FormCollection(new()
        {
            { "SAMLResponse", new(encodedResponse) },
            { "RelayState", "abc.123" }
        });

        await subject.Invoking(s => s.HandleRequestAsync())
            .Should().ThrowAsync<AuthenticationFailureException>()
            .WithInnerException<AuthenticationFailureException, ValidationException<Saml2Message>>()
            .WithMessage("*cookie*");
    }

    [Fact]
    public async Task HandleRemoteAsync_RejectsOnMissingRelaystateParam()
    {
        var options = CreateOptions();
        var (subject, httpContext) = await CreateSubject(options);

        var xmlDoc = TestData.GetXmlDocument<Saml2HandlerTests>();

        var encodedResponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlDoc.OuterXml));

        httpContext.Request.Form = new FormCollection(new()
        {
            { "SAMLResponse", new(encodedResponse) },
        });

        await subject.Invoking(s => s.HandleRequestAsync())
            .Should().ThrowAsync<AuthenticationFailureException>()
            .WithInnerException<AuthenticationFailureException, ValidationException<Saml2Message>>()
            .WithMessage("*RelayState*");
    }

    // TODO: Use event to resolve IdentityProvider - presence of EntityId indicates if challenge or response processing
    // TODO: Event when Xml was created
}