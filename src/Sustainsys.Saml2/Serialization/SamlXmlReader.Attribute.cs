using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
public partial class SamlXmlReader
{
    /// <summary>
    /// Read an Attribute
    /// </summary>
    /// <param name="source">Xml Traverser to read from</param>
    /// <returns>Attribute</returns>
    protected SamlAttribute ReadAttribute(XmlTraverser source)
    {
        var attribute = Create<SamlAttribute>();

        ReadAttributes(source, attribute);
        ReadElements(source.GetChildren(), attribute);

        return attribute;
    }

    /// <summary>
    /// Read attributes of a SamlAttribute
    /// </summary>
    /// <param name="source">Source data</param>
    /// <param name="attribute">Attribute to populate</param>
    protected virtual void ReadAttributes(XmlTraverser source, SamlAttribute attribute)
    {
        attribute.Name = source.GetRequiredAttribute(Attributes.Name);
    }

    /// <summary>
    /// Read elements of a Saml attribute.
    /// </summary>
    /// <param name="source">Source data</param>
    /// <param name="attribute">Attribute to populate</param>
    protected virtual void ReadElements(XmlTraverser source, SamlAttribute attribute)
    {
        while (source.MoveNext(true)
            && source.EnsureName(Elements.AttributeValue, Namespaces.SamlUri))
        {
            var nilAttribute = source.GetAttribute("nil", Namespaces.XsiUri);

            if (nilAttribute == "true" || nilAttribute == "1")
            {
                attribute.Values.Add(null);
            }
            else
            {
                attribute.Values.Add(source.GetTextContents());
            }
        }
    }
}