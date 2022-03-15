namespace Sustainsys.Saml2.Metadata.Elements;

/// <summary>
/// A Saml2 Metadata &lt;EntityDescriptor&gt; element.
/// </summary>
public class EntityDescriptor
{
    /// <summary>
    /// Id of the Entity. MUST be an absolute URI
    /// </summary>
    public string EntityId { get; set; } = String.Empty;
    
    /// <summary>
    /// Id of the EntityDescriptor node.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Recommended interval for cache renewal.
    /// </summary>
    public TimeSpan? CacheDuraton { get; set; }

    /// <summary>
    /// Absolute expiry time of any cached data.
    /// </summary>
    public DateTime? ValidUntil { get; set; }
}
