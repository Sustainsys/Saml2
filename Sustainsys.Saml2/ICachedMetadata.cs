using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Sustainsys.Saml2
{
	interface ICachedMetadata
    {
		/// <summary>
		/// Permitted cache duration for the metadata.
		/// </summary>
		XsdDuration? CacheDuration { get; set; }

        /// <summary>
        /// Valid until
        /// </summary>
        DateTime? ValidUntil { get; set; }
    }
}
