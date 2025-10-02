// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;

partial class SamlXmlReader
{
    /// <inheritdoc/>
    public Response ReadResponse(
        XmlTraverser source,
        Action<ReadErrorInspectorContext<Response>>? errorInspector = null)
    {
        Response response = default!;

        if (source.EnsureName(Elements.Response, Namespaces.SamlpUri))
        {
            response = ReadResponse(source);
        }

        source.MoveNext(true);

        // TODO: Test for error inspector call
        CallErrorInspector(errorInspector, response, source);

        source.ThrowOnErrors();

        return response;
    }

    /// <summary>
    /// Read a Saml Response
    /// </summary>
    /// <param name="source">Source Data</param>
    /// <returns>SamlResponse</returns>
    protected virtual Response ReadResponse(XmlTraverser source)
    {
        var samlResponse = Create<Response>();

        ReadAttributes(source, samlResponse);
        ReadElements(source.GetChildren(), samlResponse);

        return samlResponse;
    }

    /// <summary>
    /// Read elements of SamlResponse
    /// </summary>
    /// <param name="source">XmlTraverser</param>
    /// <param name="response">Response to populate</param>
    protected virtual void ReadElements(XmlTraverser source, Response response)
    {
        ReadElements(source, (StatusResponseType)response);

        while (source.HasName(Elements.Assertion, Namespaces.SamlUri))
        {
            response.Assertions.Add(ReadAssertion(source));
            source.MoveNext(true);
        }
    }
}