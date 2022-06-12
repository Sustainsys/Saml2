namespace Sustainsys.Saml2.Bindings;

/// <summary>
/// A Saml2 message in transport ready format.
/// </summary>
public class BoundMessage
{
    /// <summary>
    /// The HTTP Method that should be used to deliver the message.
    /// </summary>
    public HttpMethod Method { get; init; } = default!;

    /// <summary>
    /// Parameters to supply, either as form values or as query string params.
    /// </summary>
    public List<KeyValuePair<string, string>> Items { get; } = new();
}
