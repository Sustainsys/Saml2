using System.Xml;

namespace Sustainsys.Saml2.Bindings;

/// <summary>
/// Represents a Saml2 message as seen by the binding.
/// </summary>
public class Saml2Message
{
    /// <summary>
    /// Name of the message to be used in query strings, form fields etc.
    /// This is typically "SamlRequest" or "SamlResponse".
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// RelayState to include with message
    /// </summary>
    public string? RelayState { get; init; }

    /// <summary>
    /// The XML payload.
    /// </summary>
    public required XmlElement Xml { get; init; }

    /// <summary>
    /// Destination URL of the message. For outbound messages the URL
    /// to send the message to. For inbound, the URL the message was
    /// received at.
    /// </summary>
    public required string Destination { get; init; }
}