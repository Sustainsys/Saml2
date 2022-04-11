using Sustainsys.Saml2.Metadata.Xml;
using System.Security.Cryptography.Xml;

namespace Sustainsys.Saml2.Metadata.Elements;

/// <summary>
/// Base class for role descriptors, implements RoleDescriptorType
/// </summary>
public class RoleDescriptor
{
    /// <summary>
    /// ProtocolSupportEnumeration attribute value.
    /// </summary>
    public string? ProtocolSupportEnumeration { get; set; }

    /// <summary>
    /// Cryptographif keys for signing and encryption.
    /// </summary>
    public List<KeyDescriptor> Keys { get; set; } = new();
}
