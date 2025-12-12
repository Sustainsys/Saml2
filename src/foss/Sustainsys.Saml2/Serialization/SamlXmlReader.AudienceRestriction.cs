// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;

public partial class SamlXmlReader
{
    /// <summary>
    /// Reads an AudienceRestriction
    /// </summary>
    /// <param name="source">Source data</param>
    /// <returns>AudienceRestriction read</returns>
    protected AudienceRestriction ReadAudienceRestriction(XmlTraverser source)
    {
        var result = Create<AudienceRestriction>();

        ReadElements(source.GetChildren(), result);

        return result;
    }

    /// <summary>
    /// Read elements of AudienceRestriction
    /// </summary>
    /// <param name="source">Source data</param>
    /// <param name="result">AudienceRestriction to populate</param>
    protected virtual void ReadElements(XmlTraverser source, AudienceRestriction result)
    {
        source.MoveNext();

        while (source.EnsureName(Elements.Audience, Namespaces.SamlUri))
        {
            result.Audiences.Add(source.GetTextContents());
            source.MoveNext(true);
        }
    }
}