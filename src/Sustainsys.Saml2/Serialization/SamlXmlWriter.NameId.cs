using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Xml;
using System.Xml;

namespace Sustainsys.Saml2.Serialization;

partial class SamlXmlWriter
{
    /// <summary>
    /// Appends a node if the NameId has a value
    /// </summary>
    /// <param name="parent">Parent to append to</param>
    /// <param name="nameId">The NameId</param>
    /// <param name="localName">Local name of element</param>
    /// <param name="namespacePrefix">Namespace prefix of element.</param>
    /// <returns></returns>
    public XmlElement? AppendIfValue(XmlNode parent, NameId? nameId, string namespacePrefix, string localName)
    {
        if (nameId != null)
        {
            var element = Append(parent, namespacePrefix, localName);
            element.InnerText = nameId.Value;

            return element;
        }
        return null;
    }
}