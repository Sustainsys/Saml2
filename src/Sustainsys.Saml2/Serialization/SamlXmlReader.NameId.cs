using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Xml;

namespace Sustainsys.Saml2.Serialization;
public partial class SamlXmlReader
{
    /// <summary>
    /// Factory for NameId
    /// </summary>
    /// <returns></returns>
    protected virtual NameId CreateNameId() => new();

    /// <inheritdoc/>
    public virtual NameId ReadNameId(XmlTraverser source)
    {
        var result = CreateNameId();

        result.Value = source.GetTextContents();

        return result;
    }
}
