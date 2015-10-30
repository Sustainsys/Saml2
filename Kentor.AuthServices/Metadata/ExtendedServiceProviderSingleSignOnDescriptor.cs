using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Metadata
{
    /// <summary>
    /// Extended metadata class for SPSSODescriptor element.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "SignOn")]
    public class ExtendedServiceProviderSingleSignOnDescriptor : ServiceProviderSingleSignOnDescriptor
    {
        readonly ICollection<AttributeConsumingService> attributeConsumingServices =
            new List<AttributeConsumingService>();

        /// <summary>
        /// Attribute consuming services of the service provider.
        /// </summary>
        public ICollection<AttributeConsumingService> AttributeConsumingServices
        {
            get
            {
                return attributeConsumingServices;
            }
        }

        /// <summary>
        /// Extensions node in metadata.
        /// </summary>
        public ServiceProviderSingleSignOnDescriptorExtensions Extensions { get; }
            = new ServiceProviderSingleSignOnDescriptorExtensions();
    }
}
