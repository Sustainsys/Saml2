using System.Xml;

namespace Sustainsys.Saml2.Samlp;
partial class SamlpSerializer
{
    /// <summary>
    /// Append a type derived from RequestAbstractType, with the given name.
    /// </summary>
    /// <param name="parent">Parent node to append child element to</param>
    /// <param name="request">data</param>
    /// <param name="localName">Local name of the new element.</param>
    protected virtual XmlElement Append(XmlNode parent, RequestAbstractType request, string localName)
    {
        var element = Append(parent, localName);
        element.SetAttribute("ID", request.Id);
        element.SetAttribute("IssueInstant", XmlConvert.ToString(request.IssueInstant));
        element.SetAttribute("Version", request.Version);

        return element;
    }
}
