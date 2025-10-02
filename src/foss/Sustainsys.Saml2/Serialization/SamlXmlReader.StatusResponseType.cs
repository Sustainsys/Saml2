// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlReader
{
    /// <summary>
    /// Read attributes of a status response
    /// </summary>
    /// <param name="source">Xml traverser</param>
    /// <param name="response">StatusResponse</param>
    protected virtual void ReadAttributes(XmlTraverser source, StatusResponseType response)
    {
        response.Id = source.GetRequiredAttribute(Attributes.ID);
        response.Version = source.GetRequiredAttribute(Attributes.Version);
        response.IssueInstant = source.GetRequiredDateTimeAttribute(Attributes.IssueInstant);
        response.InResponseTo = source.GetAttribute(Attributes.InResponseTo);
        response.Destination = source.GetAttribute(Attributes.Destination);
    }

    /// <summary>
    /// Read elements of abstract StatusResponseType
    /// </summary>
    /// <param name="source">XML Traverser</param>
    /// <param name="response">Response to populate</param>
    protected virtual void ReadElements(XmlTraverser source, StatusResponseType response)
    {
        source.MoveNext();

        if (source.HasName(Elements.Issuer, Namespaces.SamlUri))
        {
            response.Issuer = ReadNameId(source);

            source.MoveNext();
        }

        (var trustedSigningKeys, var allowedHashAlgorithms) =
             GetSignatureValidationParametersFromIssuer(source, response.Issuer);

        if (source.ReadAndValidateOptionalSignature(trustedSigningKeys, allowedHashAlgorithms))
        {
            response.TrustLevel = source.TrustLevel;
            source.MoveNext();
        }

        if (source.HasName(Elements.Extensions, Namespaces.SamlpUri))
        {
            response.Extensions = ReadExtensions(source);
            source.MoveNext();
        }

        // Status is optional on XML schema level, but Core 2.3.3. says that
        // "an assertion without a subject has no defined meaning in this specification."
        // so we are treating it as mandatory.
        if (source.EnsureName(Elements.Status, Namespaces.SamlpUri))
        {
            response.Status = ReadStatus(source);
            source.MoveNext(true);
        }
    }
}