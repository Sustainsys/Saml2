using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlReader
{
    /// <summary>
    /// Reads a Scoping.
    /// </summary>
    /// <param name="source">Source data</param>
    /// <returns>Scoping read</returns>


    protected Scoping ReadScoping(XmlTraverser source)
    {
        var result = Create<Scoping>();

        ReadElements(source.GetChildren(), result);
        ReadAttributes(source, result);

        return result;
    }


    /// <summary>
    /// Reads elements of a Scoping.
    /// </summary>
    /// <param name="source">Source Xml Reader</param>
    /// <param name="scoping">Subject to populate</param>
    protected virtual void ReadElements(XmlTraverser source, Scoping scoping)
    {
        // We require at least one element.
        source.MoveNext(false);

        do
        {
            if (source.HasName(Elements.IDPList, Namespaces.SamlpUri))
            {
                scoping.IDPList = ReadIdpList(source);
            }
            else
            {
                if (source.HasName(Elements.RequesterID, Namespaces.SamlpUri))
                {
                    scoping.RequesterID.Add(source.GetTextContents());
                }
                else
                {
                    // TODO: Add test for extra elements and report errors.
                }
            }
        } while (source.MoveNext(true));
    }
    /// <summary>
    /// Read RequestedAuthnContext attributes.
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="requestedAuthnContext">Endpoint</param>
    protected virtual void ReadAttributes(XmlTraverser source, Scoping scoping)
    {
        scoping.ProxyCount = source.GetIntAttribute(Attributes.ProxyCount) ?? 0;

    }


}