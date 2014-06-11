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
    /// Extension methods for ServiceProviderSingleSignOnDescriptor
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "SignOn")]
    public static class ServiceProviderSingleSignOnDescriptorExtensions
    {
        /// <summary>
        /// Creates an XML structure for this service provider.
        /// </summary>
        /// <param name="spsso">Source data</param>
        /// <returns>XML data according to the SAML2 metadata spec.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "spsso")]
        public static XElement ToXElement(this ServiceProviderSingleSignOnDescriptor spsso)
        {
            if (spsso == null)
            {
                throw new ArgumentNullException("spsso");
            }

            var innerElementName = Saml2Namespaces.Saml2Metadata + "AssertionConsumerService";

            return new XElement(Saml2Namespaces.Saml2Metadata + "SPSSODescriptor",
                spsso.AssertionConsumerServices.Select(acs =>
                    acs.Value.ToXElement(innerElementName)));
        }
    }
}
