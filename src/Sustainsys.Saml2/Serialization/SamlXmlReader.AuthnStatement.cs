using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Xml;
using System.Diagnostics;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlReader
{
    /// <summary>
    /// Reads an AuthnStatement.
    /// </summary>
    /// <param name="source">Source data</param>
    /// <returns>authnStatement read</returns>
    protected AuthnStatement ReadAuthnStatement(XmlTraverser source)
    {
        var authnStatement = Create<AuthnStatement>();

        ReadAttributes(source, authnStatement);
        ReadElements(source.GetChildren(), authnStatement);

        return authnStatement;
    }

    /// <summary>
    /// Reads attributes of an AuthnStatement
    /// </summary>
    /// <param name="source"></param>
    /// <param name="authnStatement"></param>
    protected virtual void ReadAttributes(XmlTraverser source, AuthnStatement authnStatement)
    {
        authnStatement.AuthnInstant = source.GetRequiredDateTimeAttribute(Attributes.AuthnInstant);
        authnStatement.SessionIndex = source.GetAttribute(Attributes.SessionIndex);
        authnStatement.SessionNotOnOrAfter = source.GetDateTimeAttribute(Attributes.SessionNotOnOrAfter);
    }

    /// <summary>
    /// Reads elements of an AuthnStatement.
    /// </summary>
    /// <param name="source">Source Xml Reader</param>
    /// <param name="authnStatement">AuthnStatement to populate</param>
    protected virtual void ReadElements(XmlTraverser source, AuthnStatement authnStatement)
    {
        source.MoveNext(true);

        if (source.HasName(Elements.SubjectLocality, Namespaces.SamlUri))
        {
            // We're not supporting Subject Locality.
            source.MoveNext(true);
        }

        if (source.EnsureName(Elements.AuthnContext, Namespaces.SamlUri))
        {
            authnStatement.AuthnContext = ReadAuthnContext(source);
            source.MoveNext(true);
        }
    }
}
