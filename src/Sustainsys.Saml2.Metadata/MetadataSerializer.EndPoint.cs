using Sustainsys.Saml2.Metadata.Attributes;
using Sustainsys.Saml2.Metadata.Elements;
using Sustainsys.Saml2.Metadata.Xml;

namespace Sustainsys.Saml2.Metadata;
partial class MetadataSerializer
{
    /// <summary>
    /// Reads an endpoint.
    /// </summary>
    /// <param name="source">Source data</param>
    /// <returns>Endpoint read</returns>
    public virtual Endpoint ReadEndpoint(XmlTraverser source)
    {
        return new Endpoint
        {
            Binding = source.GetRequiredAbsoluteUriAttribute(AttributeNames.Binding).MapEnum<Binding>(),
            Location = source.GetRequiredAttribute(AttributeNames.Location) ?? ""
        };
    }
}
