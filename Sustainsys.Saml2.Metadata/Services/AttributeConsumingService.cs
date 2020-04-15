using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sustainsys.Saml2.Metadata.Services
{
    /// <summary>
    /// Metadata for an attribute consuming service.
    /// </summary>
    public class AttributeConsumingService : IIndexedEntryWithDefault
    {
        /// <summary>
        /// Index of the endpoint
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Is this the default endpoint?
        /// </summary>
        public bool? IsDefault { get; set; }

        /// <summary>
        /// Is the service required?
        /// </summary>
        public bool? IsRequired { get; set; }

        /// <summary>
        /// The name of the attribute consuming service.
        /// </summary>
        public ICollection<LocalizedName> ServiceNames { get; private set; }
            = new Collection<LocalizedName>();

        /// <summary>
        /// Description of the attribute consuming service
        /// </summary>
        public ICollection<LocalizedName> ServiceDescriptions { get; private set; }
            = new Collection<LocalizedName>();

        /// <summary>
        /// Requested attributes.
        /// </summary>
        public ICollection<RequestedAttribute> RequestedAttributes { get; private set; }
            = new Collection<RequestedAttribute>();
    }
}