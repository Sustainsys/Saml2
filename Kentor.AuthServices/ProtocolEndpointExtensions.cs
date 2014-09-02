using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Kentor.AuthServices
{
    static class ProtocolEndpointExtensions
    {
        public static void Load(this ProtocolEndpoint endpoint, XElement xmlData)
        {
            var bindingAttribute = xmlData.Attribute("Binding");

            if(bindingAttribute == null)
            {
                throw new InvalidMetadataException("Missing Binding attribute in endpoint.");
            }
            endpoint.Binding = new Uri(bindingAttribute.Value);

            var locationAttribute = xmlData.Attribute("Location");

            if (locationAttribute == null)
            {
                throw new InvalidMetadataException("Missing Location attribute in endpoint.");
            }

            endpoint.Location = new Uri(xmlData.Attribute("Location").Value);
        }
    }
}
