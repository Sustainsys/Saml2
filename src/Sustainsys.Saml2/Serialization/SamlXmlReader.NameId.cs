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
    protected virtual NameId ReadNameId(XmlTraverser source)
    {
        var result = Create<NameId>();

        // Read the text value of the NameID element
        result.Value = source.GetTextContents();
        result.Format = source.GetAbsoluteUriAttribute("Format");

        return result;
    }
}