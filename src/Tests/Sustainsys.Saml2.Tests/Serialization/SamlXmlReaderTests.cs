using Sustainsys.Saml2.Tests.Helpers;
using System.Runtime.CompilerServices;
using Sustainsys.Saml2.Xml;
using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Serialization;
using System.Xml;

namespace Sustainsys.Saml2.Tests.Serialization;

public partial class SamlXmlReaderTests
{
    private static XmlTraverser GetXmlTraverser([CallerMemberName] string? fileName = null)
        => TestData.GetXmlTraverser<SamlXmlReaderTests>(fileName);

}
