using System.Xml;

namespace Sustainsys.Saml2.Bindings;

/// <summary>
/// Represents a SAML2 message as seen by the binding.
/// </summary>
public class Saml2Message
{
    /// <summary>
    /// Name of the message to be used in query strings, form fields etc.
    /// This is typically "SamlRequest" or "SamlResponse".
    /// </summary>
    public string Name { get; init; } = default!;

    /// <summary>
    /// Destination URL of the message.
    /// </summary>
    public string Destination { get; init; } = default!;

    /// <summary>
    /// RelayState to include with message
    /// </summary>
    public string? RelayState { get; init; }

    /// <summary>
    /// The XML payload.
    /// </summary>
    public XmlDocument Xml { get; init; } = default!;
}
