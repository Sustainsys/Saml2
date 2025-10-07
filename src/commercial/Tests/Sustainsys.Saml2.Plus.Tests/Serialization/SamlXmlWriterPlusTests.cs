// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Sustainsys.Saml2.Tests.Helpers;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Sustainsys.Saml2.Plus.Tests.Serialization;
public partial class SamlXmlWriterPlusTests
{
    private static XmlDocument GetXmlDocument([CallerMemberName] string? fileName = null)
        => TestData.GetXmlDocument<SamlXmlWriterPlusTests>(fileName);
}