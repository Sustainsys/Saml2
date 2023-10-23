namespace Sustainsys.Saml2.Xml;

/// <summary>
/// Exception type thrown for Xml-related errors from the Saml2 library.
/// </summary>
public class SamlXmlException : Exception
{
    /// <summary>
    /// Errors encountered.
    /// </summary>
    public IEnumerable<Error> Errors { get; }

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="errors">Error list</param>
    public SamlXmlException(IEnumerable<Error> errors)
        : base(string.Join(" ", 
            errors.Where(e => !e.Ignore).Select(e => e.Message)))
    {
        Errors = errors;
    }
}
