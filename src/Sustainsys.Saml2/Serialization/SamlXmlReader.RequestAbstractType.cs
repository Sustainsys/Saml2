using Microsoft.AspNetCore.Antiforgery;
using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        if (source.HasName(Namespaces.SamlUri, Elements.Issuer))
        {
            request.Issuer = ReadNameId(source);
            source.MoveNext(true);
        }

        (var trustedSigningKeys, var allowedHashAlgorithms) =
            GetSignatureValidationParametersFromIssuer(source, request.Issuer);

        if (source.ReadAndValidateOptionalSignature(trustedSigningKeys, allowedHashAlgorithms, out var trustLevel))
        {
            request.TrustLevel = trustLevel;
            source.MoveNext();
        }

        if (source.HasName(Namespaces.SamlpUri, Elements.Extensions))
        {
            request.Extensions = ReadExtensions(source);
            source.MoveNext(true);
        }
    }
}
