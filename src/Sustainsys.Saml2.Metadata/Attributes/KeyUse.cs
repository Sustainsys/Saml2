namespace Sustainsys.Saml2.Metadata.Attributes;


/// <summary>
/// Usages allowed for a key
/// </summary>
[Flags]
public enum KeyUse
{
    /// <summary>
    /// Valid for signing
    /// </summary>
    Signing = 1,

    /// <summary>
    /// Valid for encryption
    /// </summary>
    Encryption = 2,

    /// <summary>
    /// Both encryption and signing.
    /// </summary>
    Both = 3,
}
