namespace Sustainsys.Saml2.Metadata.Xml;

/// <summary>
/// Error reasons in the error reporting.
/// </summary>
public enum ErrorReason
{
    /// <summary>
    /// The local of the node name was not the expected.
    /// </summary>
    UnexpectedLocalName,

    /// <summary>
    /// The namesapce of the node was not the expected.
    /// </summary>
    UnexpectedNamespace,

    /// <summary>
    /// A required attribute was missing.
    /// </summary>
    MissingAttribute,
}
