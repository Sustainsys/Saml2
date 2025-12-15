// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;

partial class SamlXmlReader
{
    /// <summary>
    /// Read the current node as an IDPSSODescriptor
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    protected virtual IDPSSODescriptor ReadIDPSSODescriptor(XmlTraverser source)
    {
        var result = Create<IDPSSODescriptor>();

        ReadAttributes(source, result);
        ReadElements(source.GetChildren(), result);

        return result;
    }

    /// <summary>
    /// Read attributes of IDPSSODescriptor.
    /// </summary>
    /// <param name="source">Xml traverser to read from</param>
    /// <param name="result">Result</param>
    protected virtual void ReadAttributes(XmlTraverser source, IDPSSODescriptor result)
    {
        ReadAttributes(source, (SSODescriptor)result);

        result.WantAuthnRequestsSigned = source.GetBoolAttribute(Attributes.WantAuthnRequestsSigned);
    }

    /// <summary>
    /// Read child elements of IDPSSODescriptor
    /// </summary>
    /// <param name="source">Xml traverser to read from</param>
    /// <param name="result"></param>
    protected virtual void ReadElements(XmlTraverser source, IDPSSODescriptor result)
    {
        ReadElements(source, (SSODescriptor)result);

        // We must have at least one SingleSignOnService in an IDPSSODescriptor and now we should be at it.
        if (!source.EnsureName(Elements.SingleSignOnService, Namespaces.MetadataUri))
        {
            return;
        }

        do
        {
            result.SingleSignOnServices.Add(ReadEndpoint(source));
        } while (source.MoveNext(true) && source.HasName(Elements.SingleSignOnService, Namespaces.MetadataUri));

        source.Skip();
    }
}