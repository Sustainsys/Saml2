using Sustainsys.Saml2.Metadata.Elements;
using Sustainsys.Saml2.Metadata.Xml;

namespace Sustainsys.Saml2.Metadata;
partial class MetadataSerializer
{
    /// <summary>
    /// Create extensions object.
    /// </summary>
    /// <returns>Extensions</returns>
    protected virtual Extensions CreateExtensions() => new();

    /// <summary>
    /// Read Extensions node.
    /// </summary>
    /// <param name="source">Soure to read from</param>
    /// <returns>Extensions</returns>
    protected Extensions ReadExtensions(XmlTraverser source) => CreateExtensions();
}