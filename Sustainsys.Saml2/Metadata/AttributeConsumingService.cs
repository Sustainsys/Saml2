using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Metadata
{
    /// <summary>
    /// Metadata for an attribute consuming service.
    /// </summary>
    public class AttributeConsumingService
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">Name of the attribute consuming service.</param>
        public AttributeConsumingService(string name)
        {
            ServiceName = name;
        }

        /// <summary>
        /// The name of the attribute consuming service.
        /// </summary>
        public string ServiceName { get; set; }

        readonly ICollection<RequestedAttribute> requestedAttributes = new List<RequestedAttribute>();

        /// <summary>
        /// Requested attributes.
        /// </summary>
        public ICollection<RequestedAttribute> RequestedAttributes
        {
            get
            {
                return requestedAttributes;
            }
        }

        /// <summary>
        /// Is this the default AttributeConsumingService of the SP?
        /// </summary>
        public bool IsDefault { get; set; }
    }

	public class AttributeConsumingService2 : IIndexedEntryWithDefault
	{
		public int Index { get; set; }
		public bool? IsDefault { get; set; }
		public ICollection<LocalizedName> ServiceNames { get; private set; }
			= new Collection<LocalizedName>();
		public ICollection<LocalizedName> ServiceDescriptions { get; private set; }
			= new Collection<LocalizedName>();
		public ICollection<RequestedAttribute> RequestedAttributes { get; private set; }
			= new Collection<RequestedAttribute>();
	}
}
