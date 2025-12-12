// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using System.Xml;

namespace Sustainsys.Saml2.Tests.Helpers;

public static class XmlExtensions
{
    public static XmlNamespaceManager GetNsMgr(this XmlNode xmlNode)
    {
        var xmlDoc = (xmlNode as XmlDocument) ?? xmlNode.OwnerDocument!;

        var nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);

        nsMgr.AddNamespace("saml", Constants.Namespaces.SamlUri);
        nsMgr.AddNamespace("samlp", Constants.Namespaces.SamlpUri);
        nsMgr.AddNamespace("md", Constants.Namespaces.MetadataUri);

        return nsMgr;
    }
}