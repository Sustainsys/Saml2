// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Xml;
using System.Security.Cryptography.Xml;
using System.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;

partial class SamlXmlReader
{
    /// <summary>
    /// Read KeyDescriptor
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    protected KeyDescriptor ReadKeyDescriptor(XmlTraverser source)
    {
        var result = Create<KeyDescriptor>();

        // TODO: Extract ReadAttributes method.

        result.Use = source.GetEnumAttribute<KeyUse>(Attributes.use, true) ?? KeyUse.Both;

        // TODO: Extract a ReadElement method

        var children = source.GetChildren();

        if (children.MoveNext(false)
            && children.EnsureName(Elements.KeyInfo, SignedXml.XmlDsigNamespaceUrl))
        {
            children.IgnoreChildren();

            var keyInfo = new KeyInfo();
            keyInfo.LoadXml((XmlElement)children.CurrentNode!);

            result.KeyInfo = keyInfo;

            children.MoveNext(true);
        }

        return result;
    }
}