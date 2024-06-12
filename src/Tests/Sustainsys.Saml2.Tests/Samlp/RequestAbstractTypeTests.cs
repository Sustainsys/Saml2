using Sustainsys.Saml2.Samlp;
using System.Xml;

namespace Sustainsys.Saml2.Tests.Samlp;
public class RequestAbstractTypeTests
{
    private class ConcreteRequest : RequestAbstractType
    { }

    [Fact]
    public void DefaultValues()
    {
        var subject = new ConcreteRequest();

        // Core 3.2.1: Id is required
        XmlConvert.VerifyNCName(subject.Id);

        // Core 3.2.1: Version is required and must be "2.0"
        subject.Version.Should().Be("2.0");
    }
}
