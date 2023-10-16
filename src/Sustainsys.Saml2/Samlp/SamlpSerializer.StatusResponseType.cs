using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Sustainsys.Saml2.Samlp.Attributes;
using Sustainsys.Saml2.Samlp.Elements;
using Sustainsys.Saml2.Xml;
using System.Data;

namespace Sustainsys.Saml2.Samlp;
partial class SamlpSerializer
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

        if (source.HasName(Constants.Namespaces.Saml, Saml.Elements.ElementNames.Issuer))
        {
            response.Issuer = samlSerializer.ReadNameId(source);

            if (EntityResolver != null)
            {
                var entity = EntityResolver(response.Issuer.Value);
                TrustedSigningKeys = entity.TrustedSigningKeys;
                AllowedHashAlgorithms = entity.AllowedHashAlgorithms ?? AllowedHashAlgorithms;
            }

            source.MoveNext();
        }

        if (source.ReadAndValidateOptionalSignature(TrustedSigningKeys, AllowedHashAlgorithms, out var trustLevel))
        {
            response.TrustLevel = trustLevel;
            source.MoveNext();
        }

        if (source.HasName(Constants.Namespaces.Samlp, ElementNames.Extensions))
        {
            response.Extensions = ReadExtensions(source);
            source.MoveNext();
        }

        if (source.EnsureName(NamespaceUri, ElementNames.Status))
        {
            response.Status = ReadStatus(source);
            source.MoveNext(true);
        }
    }
}
