using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Subclass to EntityDescriptor that contains support for extensions.
    /// </summary>
    public class ExtendedEntityDescriptor : EntityDescriptor
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
        public TimeSpan CacheDuration { get; set; }
    }
}
