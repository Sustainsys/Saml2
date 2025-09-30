using Sustainsys.Saml2.Common;
using Sustainsys.Saml2.Xml;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlReader
{
    /// <summary>
    /// Read Extensions node.
    /// </summary>
    /// <param name="source">Soure to read from</param>
    /// <returns>Extensions</returns>
    protected virtual Extensions ReadExtensions(XmlTraverser source)
    {
        source.IgnoreChildren();

        return Create<Extensions>();
    }
}