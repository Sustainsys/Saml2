namespace Sustainsys.Saml2.Metadata;

/// <summary>
/// Metadata IndexedEndpoint
/// </summary>
public class IndexedEndpoint : Endpoint
{
    /// <summary>
    /// Inded of endpoint.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Is this the default enpdoint to use?
    /// </summary>
    public bool IsDefault { get; set; }
}
