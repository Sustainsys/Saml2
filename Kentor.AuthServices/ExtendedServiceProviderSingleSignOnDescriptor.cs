using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Extended metadata class for SPSSODescriptor element.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "SignOn")]
    public class ExtendedServiceProviderSingleSignOnDescriptor : ServiceProviderSingleSignOnDescriptor
    {
        readonly ICollection<AttributeConsumingService> attributeConsumingServices =
            new List<AttributeConsumingService>();

        public ICollection<AttributeConsumingService> AttributeConsumingServices
        {
            get
            {
                return attributeConsumingServices;
            }
        }
    }
}
