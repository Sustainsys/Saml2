using Sustainsys.Saml2.Metadata.Xml;
using System.Xml;

namespace Sustainsys.Saml2.Metadata.Elements;

/// <summary>
/// A Saml2 Metadata &lt;EntityDescriptor&gt; element.
/// </summary>
public class EntityDescriptor
{
    /// <summary>
    /// Id of the Entity. MUST be an absolute URI
    /// </summary>
    public string EntityId { get; set; } = "";
    
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

    /// <summary>
    /// Trust level of this data.
    /// </summary>
    public TrustLevel TrustLevel { get; set; }
    
    /// <summary>
    /// The extensions node of the metadata.
    /// </summary>
    public Extensions? Extensions { get; set; }

    /// <summary>
    /// Role Descriptors.
    /// </summary>
    public List<RoleDescriptor> RoleDescriptors { get; } = new();
}
