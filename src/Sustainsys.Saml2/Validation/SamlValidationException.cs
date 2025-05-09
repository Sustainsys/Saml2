namespace Sustainsys.Saml2.Validation;

/// <summary>
/// Exception thrown when validation is not successful
/// </summary>
/// <param name="message">message</param>
public class SamlValidationException(string message) : Exception(message)
{ }