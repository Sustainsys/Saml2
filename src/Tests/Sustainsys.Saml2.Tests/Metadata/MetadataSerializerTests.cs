using Sustainsys.Saml2.Xml;
using Sustainsys.Saml2.Tests.Helpers;
using System.Runtime.CompilerServices;

namespace Sustainsys.Saml2.Tests.Metadata;

public partial class MetadataSerializerTests
{
    private static XmlTraverser GetXmlTraverser([CallerMemberName] string? fileName = null)
        => TestData.GetXmlTraverser<MetadataSerializerTests>(fileName);

}
