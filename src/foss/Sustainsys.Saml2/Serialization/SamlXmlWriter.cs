// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using System.Xml;

namespace Sustainsys.Saml2.Serialization;

/// <summary>
/// Serializer for Saml assertion classes
/// </summary>
public partial class SamlXmlWriter : ISamlXmlWriter
{
    /// <summary>
    /// Map of namespace prefixes to full namespace Uris.
    /// </summary>
    protected virtual IDictionary<string, string> NamespacePrefixMap { get; set; } =
        new Dictionary<string, string>
        {
            { Constants.Namespaces.Metadata, Constants.Namespaces.MetadataUri },
            { Constants.Namespaces.Saml, Constants.Namespaces.SamlUri },
            { Constants.Namespaces.Samlp, Constants.Namespaces.SamlpUri }
        };

    /// <summary>
    /// Append an element with a specified namespace prefix, using the writer's
    /// NamespacePrefixMap
    /// </summary>
    /// <param name="node">Parent node</param>
    /// <param name="localName">local name of new element</param>
    /// <param name="namespacePrefix">Namespace prefix. The actual namespace URL is
    /// looked up in <see cref="NamespacePrefixMap"/></param>
    /// <returns>The new element</returns>
    protected virtual XmlElement Append(XmlNode node, string namespacePrefix, string localName)
    {
        var ownerDoc = node as XmlDocument ?? node.OwnerDocument ??
            throw new InvalidOperationException("Owning document cannot be resolved");

        if (!NamespacePrefixMap.TryGetValue(namespacePrefix, out var namespaceUri))
        {
            throw new ArgumentException($"Namespace prefix {namespacePrefix} is not mapped", nameof(namespacePrefix));
        }

        var element = ownerDoc.CreateElement(namespacePrefix, localName, namespaceUri);

        node.AppendChild(element);

        return element;
    }
}