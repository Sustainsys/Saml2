using Sustainsys.Saml2.Samlp;
using System.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlWriter
{
    /// <summary>
    /// Append a type derived from RequestAbstractType, with the given name.
    /// </summary>
    /// <param name="parent">Parent node to append child element to</param>
    /// <param name="request">data</param>
    /// <param name="localName">Local name of the new element.</param>
    protected virtual XmlElement Append(XmlNode parent, RequestAbstractType request, string localName)
    {
        var element = Append(parent, Namespaces.Samlp, localName);
        element.SetAttribute(AttributeNames.ID, request.Id);
        element.SetAttribute(AttributeNames.IssueInstant, XmlConvert.ToString(request.IssueInstant, XmlDateTimeSerializationMode.RoundtripKind));
        element.SetAttribute(AttributeNames.Version, request.Version);

        return element;
    }
}
