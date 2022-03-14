using System;

namespace Sustainsys.Saml2.Metadata
{
    // TODO : remove public once all related metadata codes moved to the project
    public interface ICachedMetadata
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