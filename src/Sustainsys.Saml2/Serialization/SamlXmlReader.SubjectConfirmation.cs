using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Xml;
using System.Diagnostics;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlReader
{
    /// <summary>
    /// Reads a SubjectConfirmation.
    /// </summary>
    /// <param name="source">Source data</param>
    /// <returns>SubjectConfirmation read</returns>
    protected SubjectConfirmation ReadSubjectConfirmation(XmlTraverser source)
    {
        var result = Create<SubjectConfirmation>();

        ReadAttributes(source, result);
        ReadElements(source.GetChildren(), result);

        return result;
    }

    /// <summary>
    /// Reads attributes of SubjectConfirmation
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="subjectConfirmation">SubjectConfirmation</param>
    protected virtual void ReadAttributes(XmlTraverser source, SubjectConfirmation subjectConfirmation)
    {
        subjectConfirmation.Method = source.GetRequiredAbsoluteUriAttribute(Attributes.Method);
    }

    /// <summary>
    /// Reads elements of a SubjectConfirmation.
    /// </summary>
    /// <param name="source">Source Xml Reader</param>
    /// <param name="subjectConfirmation">Subject to populate</param>
    protected virtual void ReadElements(XmlTraverser source, SubjectConfirmation subjectConfirmation)
    {
        source.MoveNext(true);

        if (source.HasName(Elements.SubjectConfirmationData, Namespaces.SamlUri))
        {
            subjectConfirmation.SubjectConfirmationData = ReadSubjectConfirmationData(source);
            source.MoveNext(true);
        }
    }
}
