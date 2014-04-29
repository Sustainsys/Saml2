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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "spsso")]
        public static XElement ToXElement(this ServiceProviderSingleSignOnDescriptor spsso,
            XName elementName)
        {
            if (spsso == null)
            {
                throw new ArgumentNullException("spsso");
            }

            if (elementName == null)
            {
                throw new ArgumentNullException("elementName");
            }

            return null;
        }
    }
}
