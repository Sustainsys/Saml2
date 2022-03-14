namespace Sustainsys.Saml2.Metadata.Services
{
    public class IndexedEndpoint : Endpoint, IIndexedEntryWithDefault
    {
        /// <summary>
        /// Index of the endpoint
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Is this the default endpoint?
        /// </summary>
        public bool? IsDefault { get; set; }
    }
}