using Sustainsys.Saml2.Metadata.XmlHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sustainsys.Saml2.Metadata
{
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
}
