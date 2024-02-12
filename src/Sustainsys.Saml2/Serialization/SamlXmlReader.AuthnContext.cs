using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Xml;
using System.Diagnostics;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlReader
{
    /// <summary>
    /// Reads an AuthnContext.
    /// </summary>
    /// <param name="source">Source data</param>
    /// <returns>authnContext read</returns>
    protected AuthnContext ReadAuthnContext(XmlTraverser source)
    {
        var authnContext = Create<AuthnContext>();

        ReadElements(source.GetChildren(), authnContext);

        return authnContext;
    }

    /// <summary>
    /// Reads elements of an AuthnContext.
    /// </summary>
    /// <param name="source">Source Xml Reader</param>
    /// <param name="authnContext">AuthnContext to populate</param>
    protected virtual void ReadElements(XmlTraverser source, AuthnContext authnContext)
    {
        source.MoveNext(true);

        if (source.HasName(Namespaces.SamlUri, Elements.AuthnContextClassRef))
        {
            authnContext.AuthnContextClassRef = source.GetTextContents();
            source.MoveNext(true);
        }

        // We only support AuthnContextClassRef so far
        source.Skip();
    }
}
