namespace Sustainsys.Saml2.Saml;

/// <summary>
/// A Saml2 Subject, see core 2.4.1.
/// </summary>
public class Subject
{
    /// <summary>
    /// NameId
    /// </summary>
    public NameId? NameId { get; set; }

    /// <summary>
    /// SubjectConfirmation
    /// </summary>
    public SubjectConfirmation? SubjectConfirmation { get; set; }
}
