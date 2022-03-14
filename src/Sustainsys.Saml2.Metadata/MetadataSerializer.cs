using Sustainsys.Saml2.Metadata.Elements;
using Sustainsys.Saml2.Metadata.XmlHelpers;
using System.Xml;

namespace Sustainsys.Saml2.Metadata;

public class MetadataSerializer
{
    public EntityDescriptor ReadEntityDescriptor(XmlReader xmlReader)
    {
        xmlReader.EnsureName(Namespaces.Metadata, "EntityDescriptor");

        return new EntityDescriptor()
        {
            EntityId = xmlReader.GetRequiredAttribute("entityID")
        };
    }
}
