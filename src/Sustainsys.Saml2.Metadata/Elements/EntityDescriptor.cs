namespace Sustainsys.Saml2.Metadata.Elements;

public class EntityDescriptor
{
    public string EntityId { get; set; } = String.Empty;
    public string? Id { get; set; }
    public TimeSpan? CacheDuraton { get; set; }
    public DateTime? ValidUntil { get; set; }
}
