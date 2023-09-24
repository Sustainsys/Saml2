namespace Sustainsys.Saml2.Metadata.Attributes;

/// <summary>
/// String constants for binding Uris.
/// </summary>
public static class BindingUris
{
    /// <summary>
    /// HTTP Redirect binding identifier.
    /// </summary>
    public const string HttpRedirect = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect";

    /// <summary>
    /// HTTP POST binding identifier.
    /// </summary>
    public const string HttpPOST = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST";

    /// <summary>
    /// SOAP binding identifier.
    /// </summary>
    public const string SOAP = "urn:oasis:names:tc:SAML:2.0:bindings:SOAP";
}


/// <summary>
/// Saml2 bindings
/// </summary>
public enum Binding
{
    /// <summary>
    /// Http Redirect binding.
    /// </summary>
    [Uri(BindingUris.HttpRedirect)]
    HttpRedirect = 1,

    /// <summary>
    /// HTTP POST binding.
    /// </summary>
    [Uri(BindingUris.HttpPOST)]
    HttpPOST = 2,

    /// <summary>
    /// SOAP binding.
    /// </summary>
    [Uri(BindingUris.SOAP)]
    SOAP = 3
}