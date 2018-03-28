namespace Sustainsys.Saml2.Metadata
{
    public class IndexedEndpoint : Endpoint, IIndexedEntryWithDefault
    {
		public int Index { get; set; }

		/// <summary>
		/// Is this the default endpoint?
		/// </summary>
		public bool? IsDefault { get; set; }
	}
}
