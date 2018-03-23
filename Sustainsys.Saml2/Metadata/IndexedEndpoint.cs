namespace Sustainsys.Saml2.Metadata
{
    public class IndexedEndpoint : Endpoint, IIndexedEntryWithDefault
    {
		public int Index { get; set; }
		public bool? IsDefault { get; set; }
	}
}
