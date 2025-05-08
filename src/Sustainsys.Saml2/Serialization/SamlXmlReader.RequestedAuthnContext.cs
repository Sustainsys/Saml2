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
    /// Reads a RequestedAuthnContext.
    /// </summary>
    /// <param name="source">Source data</param>
    /// <returns>RequestedAuthnContext read</returns>
    protected RequestedAuthnContext ReadRequestedAuthnContext(XmlTraverser source)
    {
        var result = Create<RequestedAuthnContext>();

        ReadElements(source.GetChildren(), result);
        ReadAttributes(source, result);

        return result;
    }

    /// <summary>
    /// Reads elements of a requestedAuthnContext.
    /// </summary>
    /// <param name="source">Source Xml Reader</param>
    /// <param name="requestedAuthnContext">Subject to populate</param>
    protected virtual void ReadElements(XmlTraverser source, RequestedAuthnContext requestedAuthnContext)
    {
        // We require at least one element.
        source.MoveNext(false);

        do
        {
            if (source.HasName(Elements.AuthnContextClassRef, Namespaces.SamlUri))
            {
                requestedAuthnContext.AuthnContextClassRef.Add(source.GetTextContents());
            }
            else
            {
                if (source.HasName(Elements.AuthnContextDeclRef, Namespaces.SamlUri))
                {
                    requestedAuthnContext.AuthnContextDeclRef.Add(source.GetTextContents());
                }
                else
                {
                    // TODO: Add test for extra elements and report errors.
                }
            }
        } while (source.MoveNext(true)); // Read all elements found.

        // TODO: Add Test case if we have both ClassRef and DeclRef and add an error that it
        // is not allowed.
        //source.Errors.Add(new(ErrorReason.ExtraElements))
    }

    /// <summary>
    /// Read RequestedAuthnContext attributes.
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="requestedAuthnContext">Endpoint</param>
    protected virtual void ReadAttributes(XmlTraverser source, RequestedAuthnContext requestedAuthnContext)
    {
        requestedAuthnContext.Comparison = source.GetAttribute(Attributes.Comparison) ?? "";

    }
}