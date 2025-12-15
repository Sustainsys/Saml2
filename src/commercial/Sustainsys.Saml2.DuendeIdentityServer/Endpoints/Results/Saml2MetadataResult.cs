// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer.Endpoints.Results;
using Duende.IdentityServer.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;
using System.Xml;

namespace Sustainsys.Saml2.DuendeIdentityServer.Endpoints.Results;

/// <summary>
/// Result of Saml2 Metadata generation
/// </summary>
public class Saml2MetadataResult : EndpointResult<Saml2MetadataResult>
{
    /// <summary>
    /// The metadata as an Xml document
    /// </summary>
    public required XmlDocument Xml { get; set; }

    // TODO: Add Certificate property to support signed metadata.
}

/// <summary>
/// Write a Saml2 Metadata document to the HttpContext
/// </summary>
public class Saml2MetadataResultWriter : IHttpResponseWriter<Saml2MetadataResult>
{
    /// <inheritdoc/>
    public async Task WriteHttpResponse(Saml2MetadataResult result, HttpContext context)
    {
        // The content-type for the metadata MUST be application/samlmetadata+xml
        // but the requirement is rarely honoured by implementations and it
        // makes the browser download the file instead of displaying. A middle ground
        // is to check if accept header indicates it's a browser and then give it XML,
        // and if not respond with the correct content type.
        context.Response.ContentType =
        context.Request.Headers.Accept.First()?.Contains("text/html") ?? false
            ? "text/xml"
            : Constants.MetadataContentType;

        // TODO: Add metadata signing.

        await context.Response.WriteAsync(result.Xml.OuterXml);
    }
}