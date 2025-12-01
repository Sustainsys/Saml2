// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;

namespace Sustainsys.Saml2.Serialization;

/// <summary>
/// Reader for SAML classes from XML
/// </summary>
/// <remarks>
/// Commercial extensions to the base <see cref="ISamlXmlReader"/>.
/// </remarks>
public interface ISamlXmlReaderPlus : ISamlXmlReader
{
    /// <summary>
    /// Read an <see cref="AuthnRequest"/>
    /// </summary>
    /// <param name="source">Xml Traverser to read from</param>
    /// <param name="errorInspector">Callback that can inspect and alter errors before throwing</param>
    /// <returns><see cref="AuthnRequest"/></returns>
    AuthnRequest ReadAuthnRequest(
        XmlTraverser source,
        Action<ReadErrorInspectorContext<AuthnRequest>>? errorInspector = null);

}