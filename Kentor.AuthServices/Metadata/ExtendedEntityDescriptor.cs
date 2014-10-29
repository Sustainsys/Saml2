using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;

namespace Kentor.AuthServices.Metadata
{
    /// <summary>
    /// Subclass to EntityDescriptor that contains support for extensions.
    /// </summary>
    public class ExtendedEntityDescriptor : EntityDescriptor, ICachedMetadata
    {
        EntityDescriptorExtensions extensions = new EntityDescriptorExtensions();

        /// <summary>
        /// Extensions to the metadata.
        /// </summary>
        public EntityDescriptorExtensions Extensions
        {
            get
            {
                return extensions;
            }
        }

        /// <summary>
        /// Permitted cache duration for the metadata.
        /// </summary>
        public TimeSpan? CacheDuration { get; set; }

        /// <summary>
        /// Valid until
        /// </summary>
        public DateTime? ValidUntil { get; set; }
    }
}
