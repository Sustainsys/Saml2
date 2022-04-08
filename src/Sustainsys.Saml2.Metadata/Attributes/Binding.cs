namespace Sustainsys.Saml2.Metadata.Attributes;

/// <summary>
/// Saml2 bindings
/// </summary>
public enum Binding
{
    /// <summary>
    /// Http Redirect binding.
    /// </summary>
    [Uri("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect")]
    HttpRedirect = 1
}