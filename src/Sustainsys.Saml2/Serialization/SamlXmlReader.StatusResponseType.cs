using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;
using System.Security.Cryptography.Xml;
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
        response.Id = source.GetRequiredAttribute(AttributeNames.ID);
        response.Version = source.GetRequiredAttribute(AttributeNames.Version);
        response.IssueInstant = source.GetRequiredDateTimeAttribute(AttributeNames.IssueInstant);
        response.InResponseTo = source.GetAttribute(AttributeNames.InResponseTo);
        response.Destination = source.GetAttribute(AttributeNames.Destination);
    }

    /// <summary>
    /// Read elements of abstract StatusResponseType
    /// </summary>
    /// <param name="source">XML Traverser</param>
    /// <param name="response">Response to populate</param>
    protected virtual void ReadElements(XmlTraverser source, StatusResponseType response)
    {
        source.MoveNext();

        var trustedSigningKeys = TrustedSigningKeys;
        var allowedHashAlgorithms = AllowedHashAlgorithms;

        if (source.HasName(Namespaces.SamlUri, Elements.Issuer))
        {
            response.Issuer = ReadNameId(source);

            source.MoveNext();
        }

        if (source.HasName(SignedXml.XmlDsigNamespaceUrl, Elements.Signature))
        {
            if (response.Issuer == null)
            {
                source.Errors.Add(new(ErrorReason.MissingElement, Elements.Issuer, source.CurrentNode,
                    "A signature was found, but there was no Issuer specified. See profile spec 4.1.4.2, 4.4.4.2"));
            }
            else
            {
                if (EntityResolver != null)
                {
                    var entity = EntityResolver(response.Issuer.Value);
                    trustedSigningKeys = entity.TrustedSigningKeys;
                    allowedHashAlgorithms = entity.AllowedHashAlgorithms ?? AllowedHashAlgorithms;
                }
            }
        }

        if (source.ReadAndValidateOptionalSignature(trustedSigningKeys, allowedHashAlgorithms, out var trustLevel))
        {
            response.TrustLevel = trustLevel;
            source.MoveNext();
        }

        if (source.HasName(Namespaces.SamlpUri, Elements.Extensions))
        {
            response.Extensions = ReadExtensions(source);
            source.MoveNext();
        }

        if (source.EnsureName(Namespaces.SamlpUri, Elements.Status))
        {
            response.Status = ReadStatus(source);
            source.MoveNext(true);
        }
    }
}
