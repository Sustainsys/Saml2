// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer;
using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.AspNetCore.WebUtilities;
using NSubstitute;
using Sustainsys.Saml2.Bindings;
using Sustainsys.Saml2.DuendeIdentityServer.Endpoints;
using Sustainsys.Saml2.DuendeIdentityServer.Models;
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
                LoginUrl = "/X123/Login456"
            }
        };

        DateTimeOffset utcNow = new(2025, 10, 08, 13, 46, 32, TimeSpan.Zero);
        var clock = Substitute.For<IClock>();
        clock.UtcNow.Returns(utcNow);

        var subject = new SingleSignOnServiceEndpoint(
            frontChannelBindings,
            new SamlXmlReaderPlus(),
            clientStore,
            userSession,
            idSrvOptions,
            new SamlXmlWriterPlus(),
            clock);

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

        iActual.Should().BeOfType<Saml2FrontChannelResult>();
        var actual = (Saml2FrontChannelResult)iActual;

        var currentUrl = "/Saml2/Sso" + httpContext.Request.QueryString.Value;

        var expectedRedirectUrl = "/X123/Login456?ReturnUrl=" + Uri.EscapeDataString(currentUrl);

        actual.RedirectUrl.Should().Be(expectedRedirectUrl);
    }

    [Fact]
    public async Task Process_MissingSaml2Sp()
    {
        (var httpContext, var subject) = CreateSubject();

        var iActual = await subject.ProcessAsync(httpContext);
        iActual.Should().BeOfType<Saml2FrontChannelResult>();
        var actual = (Saml2FrontChannelResult)iActual;

        actual.Error.Should().Be("Invalid SP EntityID.");
        actual.SpEntityID.Should().Be("https://invalid-sp.example.com/Saml2");
    }

    [Fact]
    public async Task Process_NotSaml2Sp()
    {
        (var httpContext, var subject) = CreateSubject();

        var iActual = await subject.ProcessAsync(httpContext);
        iActual.Should().BeOfType<Saml2FrontChannelResult>();
        var actual = (Saml2FrontChannelResult)iActual;

        actual.Error.Should().Be("Invalid SP EntityID.");
        actual.SpEntityID.Should().Be("urn:notSaml2");
    }
}
