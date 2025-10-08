// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Xml;
using System.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlWriterPlus
{
    /// <summary>
    /// Append a Subject element
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="subject"></param>
    protected virtual void Append(XmlNode parent, Subject subject)
    {
        if (subject != null)
        {
            var subjectElement = AppendElement(parent, Namespaces.Saml, Elements.Subject);
            if (subject.NameId != null)
            {
                Append(subjectElement, subject.NameId, "NameID");
            }

            if (subject.SubjectConfirmation != null)
            {
                Append(subjectElement, subject.SubjectConfirmation);
            }
        }
    }

    /// <summary>
    /// Append a SubjectConfirmation element
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="subjectConfirmation"></param>
    protected virtual void Append(XmlNode parent, SubjectConfirmation subjectConfirmation)
    {
        var subjectConfirmationElement = AppendElement(parent, Namespaces.Saml, Elements.SubjectConfirmation);
        subjectConfirmationElement.SetAttribute(Attributes.Method, subjectConfirmation.Method);

        if (subjectConfirmation.SubjectConfirmationData != null)
        {
            Append(subjectConfirmationElement, subjectConfirmation.SubjectConfirmationData);
        }
    }
    /// <summary>
    /// Append a SubjectConfirmationData element
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="subjectConfirmationData"></param>
    protected virtual void Append(XmlNode parent, SubjectConfirmationData subjectConfirmationData)
    {
        var subjectConfirmationDataElement = AppendElement(parent, Namespaces.Saml, Elements.SubjectConfirmationData);

        if (subjectConfirmationData.NotOnOrAfter.HasValue)
        {
            var dt = new DateTime(subjectConfirmationData.NotOnOrAfter.Value.Ticks, DateTimeKind.Utc);
            subjectConfirmationDataElement.SetAttribute(Attributes.NotOnOrAfter, dt.ToString("yyyy-MM-ddTHH:mm:ss\\Z"));
        }
        subjectConfirmationDataElement.SetAttributeIfValue(Attributes.Recipient, subjectConfirmationData.Recipient);
    }
}