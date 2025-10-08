// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer.Endpoints.Results;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.AspNetCore.WebUtilities;
using NSubstitute;
using Sustainsys.Saml2.Bindings;
using Sustainsys.Saml2.DuendeIdentityServer.Endpoints;
using Sustainsys.Saml2.Serialization;
using Sustainsys.Saml2.Tests.Helpers;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Sustainsys.Saml2.DuendeIdentityServer.Tests.Endpoints;
public class SingleSignOnServiceEndpointTests
{
    static XmlDocument? GetXmlDocument([CallerMemberName] string? fileName = null) =>
         TestData.GetXmlDocument<SingleSignOnServiceEndpointTests>(fileName);

    static (HttpContext, SingleSignOnServiceEndpoint) CreateSubject(
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
                Destination = "https://idp.example.com/Saml2/Sso"
            };

            var queryString = HttpRedirectBinding.GetQueryString(message);
            httpContext.Request.Query = new QueryCollection(QueryHelpers.ParseQuery(queryString));
            httpContext.Request.QueryString = new(queryString);
        }

        httpContext.Request.Method = HttpMethods.Get;
        httpContext.Request.Scheme = HttpScheme.Https.ToString();
        httpContext.Request.Host = new("idp.example.com");

        IEnumerable<IFrontChannelBinding> frontChannelBindings = [new HttpRedirectBinding(), new HttpPostBinding()];

        var clientStore = new InMemoryClientStore(
            [
                new Client()
                {
                    ClientId = "https://sp.example.com/Saml2",
                    RedirectUris = { "https://sp.example.com/Saml2/Acs"}
                }
            ]);

        var subject = new SingleSignOnServiceEndpoint(
            frontChannelBindings,
            new SamlXmlReaderPlus(),
            clientStore);

        return (httpContext, subject);
    }

    // Simple happy path test
    [Fact]
    public async Task Process()
    {
        (var httpContext, var subject) = CreateSubject();

        var iActual = await subject.ProcessAsync(httpContext);

        iActual.Should().BeOfType<Saml2FrontChannelResult>();
        var actual = (Saml2FrontChannelResult)iActual;

        var expectedXml = GetXmlDocument();

        actual.Message?.Xml.Should().BeEquivalentTo(expectedXml);
    }

    [Fact]
    public async Task Process_NoUserSession()
    {
        (var httpContext, var subject) = CreateSubject();

        var iActual = await subject.ProcessAsync(httpContext);

        iActual.Should().BeOfType<LoginPageResult>();
        var actual = (RedirectResult)iActual;

        var currentUrl = httpContext.Request.GetEncodedUrl();

        var expectedRedirectUrl = "https://idp.example.com/Account/Login?ReturnUrl=" + currentUrl;

        actual.Url.Should().Be(expectedRedirectUrl);
    }

    [Fact]
    public async Task Process_InvalidClient()
    {
        (var httpContext, var subject) = CreateSubject();

        var iActual = await subject.ProcessAsync(httpContext);
        iActual.Should().BeOfType<Saml2FrontChannelResult>();
        var actual = (Saml2FrontChannelResult)iActual;

        actual.Error.Should().Be("Invalid SP EntityID.");
        actual.SpEntityID.Should().Be("https://invalid-sp.example.com/Saml2");
    }
}
