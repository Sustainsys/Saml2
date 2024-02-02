using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Xml;
using System.Diagnostics;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlReader
{
    /// <summary>
    /// Factory for Subject
    /// </summary>
    protected virtual Subject CreateSubject() => new();

    /// <summary>
    /// Reads a Subject.
    /// </summary>
    /// <param name="source">Source data</param>
    /// <returns>Subject read</returns>
    protected virtual Subject ReadSubject(XmlTraverser source)
    {
        var result = CreateSubject();
        
        ReadAttributes(source, result);

        ReadElements(source.GetChildren(), result);

        return result;
    }

    /// <summary>
    /// Extension point to add reading of attributes for Subject
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="subject">Subject</param>
    protected virtual void ReadAttributes(XmlTraverser source, Subject subject)
    {
    }

    /// <summary>
    /// Reads elements of a subject.
    /// </summary>
    /// <param name="source">Source Xml Reader</param>
    /// <param name="subject">Subject to populate</param>
    protected virtual void ReadElements(XmlTraverser source, Subject subject)
    {
        source.MoveNext(true);

        if (source.HasName(Namespaces.SamlUri, Elements.NameID))
        {
            subject.NameId = ReadNameId(source);
            source.MoveNext(true);
        }
        else
        {
            // TODO: Support BaseID and EncryptedID
        }

        // TODO: Support SubjectConfirmation
    }
}
