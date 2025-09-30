using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlReader
{
    /// <summary>
    /// Reads a Subject.
    /// </summary>
    /// <param name="source">Source data</param>
    /// <returns>Subject read</returns>
    protected Subject ReadSubject(XmlTraverser source)
    {
        var result = Create<Subject>();

        ReadElements(source.GetChildren(), result);

        return result;
    }

    /// <summary>
    /// Reads elements of a subject.
    /// </summary>
    /// <param name="source">Source Xml Reader</param>
    /// <param name="subject">Subject to populate</param>
    protected virtual void ReadElements(XmlTraverser source, Subject subject)
    {
        source.MoveNext(true);

        if (source.HasName(Elements.NameID, Namespaces.SamlUri))
        {
            subject.NameId = ReadNameId(source);
            source.MoveNext(true);
        }
        else
        {
            // TODO: Support BaseID and EncryptedID
        }

        if (source.HasName(Elements.SubjectConfirmation, Namespaces.SamlUri))
        {
            subject.SubjectConfirmation = ReadSubjectConfirmation(source);
            source.MoveNext(true);
        }
    }
}