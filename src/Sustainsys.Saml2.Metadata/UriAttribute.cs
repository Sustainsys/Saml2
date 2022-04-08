using System;

namespace Sustainsys.Saml2.Metadata;

/// <summary>
/// C# attribute (not an XML attribute) containing the URI for enum values.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class UriAttribute : Attribute
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="uri">URI representation of the value used in Metadata.</param>
    public UriAttribute(string uri)
    {
        Uri = uri;
    }

    /// <summary>
    /// URI representation of the value used in Metadata.
    /// </summary>
    public string Uri { get; }
}