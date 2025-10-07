// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Sustainsys.Saml2.Samlp;
using System.Xml;

namespace Sustainsys.Saml2.Serialization;


/// <summary>
/// Write Saml entities to XML
/// </summary>
/// <remarks>
/// Commercial extensions to the base <see cref="ISamlXmlWriter"/>.
/// </remarks>
public interface ISamlXmlWriterPlus : ISamlXmlWriter
{
    /// <summary>
    /// Create an Xml document and write a SamlResponse to it.
    /// </summary>
    /// <param name="response">Saml Response</param>
    /// <returns>Created XmlDoc</returns>
    XmlDocument Write(Response response);
}