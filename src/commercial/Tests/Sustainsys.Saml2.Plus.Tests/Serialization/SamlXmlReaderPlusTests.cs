// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Sustainsys.Saml2.Tests.Helpers;
using Sustainsys.Saml2.Xml;
using System.Runtime.CompilerServices;

namespace Sustainsys.Saml2.Plus.Tests.Serialization;

public partial class SamlXmlReaderPlusTests
{
    private static XmlTraverser GetXmlTraverser([CallerMemberName] string? fileName = null)
        => TestData.GetXmlTraverser<SamlXmlReaderPlusTests>(fileName);
}