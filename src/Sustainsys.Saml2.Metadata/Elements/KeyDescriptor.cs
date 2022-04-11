using Sustainsys.Saml2.Metadata.Attributes;
using System.Security.Cryptography.Xml;

namespace Sustainsys.Saml2.Metadata.Elements;

/// <summary>
/// Metadata keydscriptor
/// </summary>
public class KeyDescriptor
{
    /// <summary>
    /// Allowed use of the key. Default is Both as that's
    /// what an empty value means.
    /// </summary>
    public KeyUse Use {get;set;} = KeyUse.Both;

    /// <summary>
    /// Key info
    /// </summary>
    public KeyInfo? KeyInfo { get; set; }
}
