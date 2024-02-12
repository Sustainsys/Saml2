using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Xml;

namespace Sustainsys.Saml2.Serialization;
public partial class SamlXmlReader
{
    /// <summary>
    /// Read a NameId
    /// </summary>
    /// <param name="source">Xml Traverser to read from</param>
    /// <returns>NameId</returns>
    public virtual NameId ReadNameId(XmlTraverser source)
    {
        var result = Create<NameId>();

        result.Value = source.GetTextContents();

        return result;
    }
}
