namespace Sustainsys.Saml2.Metadata.Xml;

/// <summary>
/// Exception type thrown for Xml-related errors from the Saml2 library.
/// </summary>
public class Saml2XmlException : Exception
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="message">Message</param>
    public Saml2XmlException(string message)
        : base(message) { }
}
