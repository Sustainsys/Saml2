using Sustainsys.Saml2.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sustainsys.Saml2.Saml;

/// <summary>
/// Serializer for Saml assertion classes
/// </summary>
public interface ISamlSerializer
{
    /// <summary>
    /// Append a nameid to the parent with the given local name
    /// </summary>
    /// <param name="parent">Parent node</param>
    /// <param name="nameId">NameId to add</param>
    /// <param name="localName">Local name of new element</param>
    /// <returns>The created element</returns>
    XmlElement Append(XmlNode parent, NameId nameId, string localName);
}


/// <summary>
/// Serializer for Saml assertion classes
/// </summary>
public class SamlSerializer : SerializerBase, ISamlSerializer
{
    /// <summary>
    /// Ctor
    /// </summary>
    public SamlSerializer()
    {
        NamespaceUri = Constants.SamlNamespace;
        Prefix = "saml";
    }

    /// <inheritdoc/>
    public XmlElement Append(XmlNode parent, NameId nameId, string localName)
    {
        var element = Append(parent, localName);
        element.InnerText = nameId.Value;

        return element;
    }
}
