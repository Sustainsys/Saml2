using Sustainsys.Saml2.Saml.Elements;
using Sustainsys.Saml2.Xml;
using System.Xml;

namespace Sustainsys.Saml2.Saml;

partial class SamlSerializer
{
    /// <inheritdoc/>
    public XmlElement? AppendIfValue(XmlNode parent, NameId? nameId, string localName)
    {
        if (nameId != null)
        {
            var element = Append(parent, localName);
            element.InnerText = nameId.Value;

            return element;
        }
        return null;
    }

    /// <summary>
    /// Factory for NameId
    /// </summary>
    /// <returns></returns>
    protected virtual NameId CreateNameId() => new();

    /// <inheritdoc/>
    public virtual NameId ReadNameId(XmlTraverser source)
    {
        var result = CreateNameId();

        result.Value = source.GetTextContents();

        return result;
    }
}
