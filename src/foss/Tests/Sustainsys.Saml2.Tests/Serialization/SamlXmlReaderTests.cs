// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Tests.Helpers;
using Sustainsys.Saml2.Xml;
using System.Runtime.CompilerServices;

namespace Sustainsys.Saml2.Tests.Serialization;

public partial class SamlXmlReaderTests
{
    private static XmlTraverser GetXmlTraverser([CallerMemberName] string? fileName = null)
        => TestData.GetXmlTraverser<SamlXmlReaderTests>(fileName);
}