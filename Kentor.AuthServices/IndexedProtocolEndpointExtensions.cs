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
    /// Extension methods for IndexedProtocolEndpoint
    /// </summary>
    public static class IndexedProtocolEndpointExtensions
    {
        /// <summary>
        /// Writes out the AssertionConsumerService as an XElement
        /// </summary>
        /// <param name="indexedProtocolEndpoint"></param>
        /// <param name="elementName"></param>
        /// <returns>XElement</returns>
        public static XElement ToXElement(this IndexedProtocolEndpoint indexedProtocolEndpoint,
            XName elementName)
        {
            if (indexedProtocolEndpoint == null)
            {
                throw new ArgumentNullException("indexedProtocolEndpoint");
            }

            if (elementName == null)
            {
                throw new ArgumentNullException("elementName");
            }

            return new XElement(elementName,
                new XAttribute("isDefault", indexedProtocolEndpoint.IsDefault),
                new XAttribute("index", indexedProtocolEndpoint.Index),
                new XAttribute("Binding", Saml2Binding.HttpPostUri),
                new XAttribute("Location", indexedProtocolEndpoint.Location));
        }
    }
}
