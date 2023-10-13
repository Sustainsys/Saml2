using Sustainsys.Saml2.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sustainsys.Saml2.Saml;

/// <summary>
/// Serializer for Saml assertion classes
/// </summary>
public interface ISamlSerializer : ISerializerBase
{
    /// <summary>
    /// Append a nameid to the parent with the given local name
    /// </summary>
    /// <param name="parent">Parent node</param>
    /// <param name="nameId">NameId to add, if null nothing is added</param>
    /// <param name="localName">Local name of new element</param>
    /// <returns>The created element, or null if none</returns>
    XmlElement? AppendIfValue(XmlNode parent, NameId? nameId, string localName);
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
        NamespaceUri = Constants.Namespaces.Saml;
        Prefix = "saml";
    }

    /// <inheritdoc/>
    public XmlElement? AppendIfValue(XmlNode parent, NameId? nameId, string localName)
    {
        if(nameId != null)
        {
            var element = Append(parent, localName);
            element.InnerText = nameId.Value;

            return element;
        }
        return null;
    }
}
