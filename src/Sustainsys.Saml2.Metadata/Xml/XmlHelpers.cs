using System.Security.Cryptography;
using System.Xml;

namespace Sustainsys.Saml2.Metadata.Xml;

/// <summary>
/// Xml utilities
/// </summary>
public static class XmlHelpers
{
    /// <summary>
    /// Create a valid xs:ID
    /// </summary>
    /// <returns>Id as string</returns>
    public static string CreateId()
    {
        var bytes = RandomNumberGenerator.GetBytes(20);
        return FormatId(bytes);
    }

    // Split to separate function to enable testing of formatting.
    internal static string FormatId(byte[] bytes)
    {
        // Ensure starting char will be a letter.
        bytes[0] = (byte)(bytes[0] & 0x7F);

        // Manually do Base 64 URL as we do not have a reference to Base64UrlTextEncoder here.
        return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }

    /// <summary>
    /// Get an Xml traverser for an XmlDocument
    /// </summary>
    /// <param name="xmlDoc">Source XmlDocument</param>
    /// <returns>XmlTraverser locatet at DocumentElement</returns>
    public static XmlTraverser GetXmlTraverser(this XmlDocument xmlDoc)
        => new(xmlDoc?.DocumentElement ?? throw new ArgumentException("DocumentElement cannot be null"));
}
