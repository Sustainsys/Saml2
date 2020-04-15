namespace Sustainsys.Saml2.Metadata.Services
{
    /// <summary>
    /// An indexed entry with an optional default
    /// </summary>
    public interface IIndexedEntryWithDefault
    {
        /// <summary>
        /// Index of the endpoint
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// Is this the default endpoint?
        /// </summary>
        bool? IsDefault { get; set; }
    }
}