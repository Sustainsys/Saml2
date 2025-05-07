using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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