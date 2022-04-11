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
    protected virtual Endpoint ReadEndpoint(XmlTraverser source)
    {
        var result = new Endpoint();
        ReadAttributes(source, result);

        return result;
    }

    /// <summary>
    /// Read endpoint attributes.
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="endpoint">Endpoint</param>
    protected virtual void ReadAttributes(XmlTraverser source, Endpoint endpoint)
    {
        endpoint.Binding = source.GetRequiredAbsoluteUriAttribute(AttributeNames.Binding).MapEnum<Binding>();
        endpoint.Location = source.GetRequiredAttribute(AttributeNames.Location) ?? "";
    }

    /// <summary>
    /// Read indexed endpoint
    /// </summary>
    /// <param name="source">Source</param>
    /// <returns>IndexedEndpoint</returns>
    protected virtual IndexedEndpoint ReadIndexedEndpoint(XmlTraverser source)
    {
        var result = new IndexedEndpoint()
        {
            Index = source.GetRequiredIntAttribute(AttributeNames.index) ?? 0,
            IsDefault = source.GetBoolAttribute(AttributeNames.isDefault) ?? false
        };

        ReadAttributes(source, result);

        return result;
    }
}
