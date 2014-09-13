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
        /// <returns></returns>
        public static XElement ToXElement(
            this EntityDescriptor entityDescriptor)
        {
            if (entityDescriptor == null)
            {
                throw new ArgumentNullException("entityDescriptor");
            }

            return new XElement(Saml2Namespaces.Saml2Metadata + "EntityDescriptor",
                new XAttribute("entityID", entityDescriptor.EntityId.Id),
                entityDescriptor.RoleDescriptors.OfType<ServiceProviderSingleSignOnDescriptor>()
                    .Select(rd => rd.ToXElement())
                );
        }
    }
}
