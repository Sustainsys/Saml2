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
	public class AttributeConsumingService : IndexedEndpoint
	{
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
