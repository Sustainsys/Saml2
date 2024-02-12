using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
public partial class SamlXmlReader
{
    /// <summary>
    /// Reads an AudienceRestriction
    /// </summary>
    /// <param name="source">Source data</param>
    /// <returns>AudienceRestriction read</returns>
    protected AudienceRestriction ReadAudienceRestriction(XmlTraverser source)
    {
        var result = Create<AudienceRestriction>();

        ReadElements(source.GetChildren(), result);

        return result;
    }

    /// <summary>
    /// Read elements of AudienceRestriction
    /// </summary>
    /// <param name="source">Source data</param>
    /// <param name="result">AudienceRestriction to populate</param>
    protected virtual void ReadElements(XmlTraverser source, AudienceRestriction result)
    {
        source.MoveNext();

        while (source.EnsureName(Namespaces.SamlUri, Elements.Audience))
        {
            result.Audiences.Add(source.GetTextContents());
            source.MoveNext(true);
        }
    }
}
