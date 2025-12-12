// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;

public partial class SamlXmlReader
{
    /// <summary>
    /// Read attributes of <see cref="RequestAbstractType"/>
    /// </summary>
    /// <param name="source">Xml travers</param>
    /// <param name="request">RequestAbstractType</param>
    protected virtual void ReadAttributes(XmlTraverser source, RequestAbstractType request)
    {
        request.Id = source.GetRequiredAttribute(Attributes.ID);
        request.IssueInstant = source.GetRequiredDateTimeAttribute(Attributes.IssueInstant);
        request.Version = source.GetRequiredAttribute(Attributes.Version);
        request.Destination = source.GetAttribute(Attributes.Destination);
        request.Consent = source.GetAttribute(Attributes.Consent);
    }

    /// <summary>
    /// Reads the child elements of a RequestAbstractType
    /// </summary>
    /// <param name="source"></param>
    /// <param name="request"></param>
    protected virtual void ReadElements(XmlTraverser source, RequestAbstractType request)
    {
        source.MoveNext(true);

        if (source.HasName(Elements.Issuer, Namespaces.SamlUri))
        {
            request.Issuer = ReadNameId(source);
            source.MoveNext(true);
        }

        (var trustedSigningKeys, var allowedHashAlgorithms) =
            GetSignatureValidationParametersFromIssuer(source, request.Issuer);

        if (source.ReadAndValidateOptionalSignature(trustedSigningKeys, allowedHashAlgorithms))
        {
            request.TrustLevel = source.TrustLevel;
            source.MoveNext();
        }

        if (source.HasName(Elements.Extensions, Namespaces.SamlpUri))
        {
            request.Extensions = ReadExtensions(source);
            source.MoveNext(true);
        }
    }
}