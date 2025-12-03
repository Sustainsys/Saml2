// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer;
using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.AspNetCore.WebUtilities;
using NSubstitute;
using Sustainsys.Saml2.Bindings;
using Sustainsys.Saml2.DuendeIdentityServer.Endpoints;
using Sustainsys.Saml2.DuendeIdentityServer.Endpoints.Results;
using Sustainsys.Saml2.DuendeIdentityServer.Models;
using Sustainsys.Saml2.DuendeIdentityServer.ResponseHandling.Default;
using Sustainsys.Saml2.DuendeIdentityServer.Services;
using Sustainsys.Saml2.DuendeIdentityServer.Validation;
using Sustainsys.Saml2.Serialization;
using Sustainsys.Saml2.Tests.Helpers;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Xml;

namespace Sustainsys.Saml2.DuendeIdentityServer.Tests.Endpoints;
public class SingleSignOnServiceEndpointTests
{
    static XmlDocument? GetXmlDocument([CallerMemberName] string? fileName = null) =>
         TestData.GetXmlDocument<SingleSignOnServiceEndpointTests>(fileName);

    static (HttpContext, SingleSignOnServiceEndpoint) CreateSubject(
        ClaimsPrincipal? currentUser = null,
        [CallerMemberName] string? caller = null)
    {
        var httpContext = Substitute.For<HttpContext>();


        var fileName = $"{caller}_Request";
        var requestDoc = GetXmlDocument(fileName);
        if (requestDoc != null)
        {
            var message = new Saml2Message()
            {
                Name = Constants.SamlRequest,
                Xml = requestDoc.DocumentElement ?? throw new InvalidOperationException(),
                Destination = "https://idp.example.com/Saml2/Sso",
                Binding = Constants.BindingUris.HttpRedirect
            };

            var queryString = HttpRedirectBinding.GetQueryString(message);
            httpContext.Request.Query = new QueryCollection(QueryHelpers.ParseQuery(queryString));
            httpContext.Request.QueryString = new(queryString);
        }

        httpContext.Request.Method = HttpMethods.Get;
        httpContext.Request.Scheme = HttpScheme.Https.ToString();
        httpContext.Request.Host = new("idp.example.com");
        httpContext.Request.Path = "/Saml2/Sso";

        IEnumerable<IFrontChannelBinding> frontChannelBindings = [new HttpRedirectBinding(), new HttpPostBinding()];

        var clientStore = new InMemoryClientStore(
            [
                new Saml2Sp()
                {
                    EntityId = "https://sp.example.com/Saml2",
                    AsssertionConsumerServices = { "https://sp.example.com/Saml2/Acs"},
                },
                new Client()
                {
                    ClientId = "urn:notSaml2"
                }
            ]);

        var userSession = Substitute.For<IUserSession>();
        userSession.GetUserAsync().Returns(currentUser);

        IdentityServerOptions idSrvOptions = new()
        {
            UserInteraction = new()
            {
                LoginUrl = "/X123/Login456",
                LoginReturnUrlParameter = "goHereWhenDone"
            }
        };

        DateTimeOffset utcNow = new(2025, 10, 08, 13, 46, 32, TimeSpan.Zero);
        var clock = Substitute.For<IClock>();
        clock.UtcNow.Returns(utcNow);

        var saml2IssuerNameService = Substitute.For<ISaml2IssuerNameService>();
        saml2IssuerNameService.GetCurrentAsync().Returns("https://idp.example.com/Saml2");

        var resourceValidator = Substitute.For<IResourceValidator>();

        var profileService = Substitute.For<IProfileService>();

        var subject = new SingleSignOnServiceEndpoint(
            frontChannelBindings,
            new SamlXmlReaderPlus(),
            userSession,
            idSrvOptions,
            new AuthnRequestValidator(clientStore, resourceValidator),
            new Saml2SsoInteractionResponseGenerator(),
            new Saml2SSoResponseGenerator(saml2IssuerNameService, clock, new SamlXmlWriterPlus(), profileService),
            saml2IssuerNameService);

        return (httpContext, subject);
    }

    // Simple happy path test
    [Fact]
    public async Task Process()
    {
        ClaimsIdentity identity = new(
            [new("sub", "x123456")], "pwd", "name", "role");

        (var httpContext, var subject) = CreateSubject(new(identity));

        var iActual = await subject.ProcessAsync(httpContext);

        iActual.Should().BeOfType<Saml2FrontChannelResult>();
        var actual = (Saml2FrontChannelResult)iActual;

        var expectedXml = GetXmlDocument();

        // ID is generated, set value in expected to value from actual to make comparison pass.
        expectedXml!.DocumentElement!.Attributes["ID"]!.Value = actual.Message!.Xml.GetAttribute("ID");
        expectedXml!.DocumentElement![Constants.Elements.Assertion, Constants.Namespaces.SamlUri]!.Attributes["ID"]!.Value =
            actual.Message!.Xml[Constants.Elements.Assertion, Constants.Namespaces.SamlUri]!.Attributes["ID"]!.Value;

        actual.Message.Should().NotBeNull();
        actual.Message!.Xml.Should().BeEquivalentTo(expectedXml);
    }

    [Fact]
    public async Task Process_NoUserSession()
    {
        (var httpContext, var subject) = CreateSubject();

        var iActual = await subject.ProcessAsync(httpContext);

        iActual.Should().BeOfType<LoginPageResult>();
        var actual = (LoginPageResult)iActual;

        var expectedRedirectUrl = "/X123/Login456";

        actual.RedirectUrl.Should().Be(expectedRedirectUrl);
        actual.ReturnUrlParameterName.Should().Be("goHereWhenDone");
    }

    [Fact]
    public async Task Process_MissingSaml2Sp()
    {
        (var httpContext, var subject) = CreateSubject();

        var iActual = await subject.ProcessAsync(httpContext);
        iActual.Should().BeOfType<Saml2FrontChannelResult>();
        var actual = (Saml2FrontChannelResult)iActual;

        actual.Error.Should().Be("Invalid SP EntityId.");
        actual.SpEntityId.Should().Be("https://invalid-sp.example.com/Saml2");
    }

    [Fact]
    public async Task Process_NotSaml2Sp()
    {
        (var httpContext, var subject) = CreateSubject();

        var iActual = await subject.ProcessAsync(httpContext);
        iActual.Should().BeOfType<Saml2FrontChannelResult>();
        var actual = (Saml2FrontChannelResult)iActual;

        actual.Error.Should().Be("Invalid SP EntityId.");
        actual.SpEntityId.Should().Be("urn:notSaml2");
    }
}
