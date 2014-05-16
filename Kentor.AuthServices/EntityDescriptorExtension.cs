using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Extension methods for EntityDescriptor
    /// </summary>
    public static class EntityDescriptorExtension
    {
     /// <summary>
     /// Converts EntityDescriptor to XElement
     /// </summary>
     /// <param name="entityDescriptor"></param>
     /// <param name="elementName"></param>
     /// <returns></returns>
        public static XElement ToXElement(this EntityDescriptor entityDescriptor,
            XName elementName)
        {
            if (entityDescriptor == null)
            {
                throw new ArgumentNullException("entityDescriptor");
            }

            if (elementName == null)
            {
                throw new ArgumentNullException("elementName");
            }

            var innerElementName = Saml2Namespaces.Saml2Metadata + "SPSSODescriptor";

            return new XElement(elementName,
                new XAttribute(XName.Get("entityId"), entityDescriptor.EntityId.Id),
                entityDescriptor.RoleDescriptors.OfType<ServiceProviderSingleSignOnDescriptor>().Select(rd => rd.ToXElement(innerElementName))
                );
        }
    }
}
