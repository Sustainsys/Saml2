using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlReader
{
    /// <summary>
    /// Reads a SubjectConfirmationData.
    /// </summary>
    /// <param name="source">Source data</param>
    /// <returns>SubjectConfirmationData read</returns>
    protected SubjectConfirmationData ReadSubjectConfirmationData(XmlTraverser source)
    {
        var result = Create<SubjectConfirmationData>();

        ReadAttributes(source, result);

        return result;
    }

    /// <summary>
    /// Extension point to add reading of attributes for Subject
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="subjectConfirmationData">SubjectConfirmationData</param>
    protected virtual void ReadAttributes(XmlTraverser source, SubjectConfirmationData subjectConfirmationData)
    {
        subjectConfirmationData.NotBefore = source.GetDateTimeAttribute(Attributes.NotBefore);
        subjectConfirmationData.NotOnOrAfter = source.GetDateTimeAttribute(Attributes.NotOnOrAfter);
        subjectConfirmationData.Recipient = source.GetAttribute(Attributes.Recipient);
        subjectConfirmationData.InResponseTo = source.GetAttribute(Attributes.InResponseTo);
        subjectConfirmationData.Address = source.GetAttribute(Attributes.Address);
    }
}
