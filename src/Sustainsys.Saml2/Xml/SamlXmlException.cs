namespace Sustainsys.Saml2.Xml;

/// <summary>
/// Exception type thrown for Xml-related errors from the Saml2 library.
/// </summary>
/// <remarks>
/// Ctor
/// </remarks>
/// <param name="errors">Error list</param>
public class SamlXmlException(IEnumerable<Error> errors) : 
    Exception(string.Join(" ", errors.Where(e => !e.Ignore).Select(e => e.Message)))
{
    /// <summary>
    /// Errors encountered.
    /// </summary>
    public IEnumerable<Error> Errors { get; } = errors;
}
