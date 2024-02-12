namespace Sustainsys.Saml2.Saml;

/// <summary>
/// AttributeStatement, Core 2.7.3
/// </summary>
public class AttributeStatement : List<SamlAttribute>
{
    /// <summary>
    /// Convenience add method to add a single valued attribute.
    /// </summary>
    /// <param name="attributeName">Name of the attribute</param>
    /// <param name="attributeValue">Value of the attribute</param>
    public void Add(string attributeName, string attributeValue)
        => Add(new() { Name = attributeName, Values = { attributeValue } });
}