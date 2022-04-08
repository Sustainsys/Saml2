using FluentAssertions;
using FluentAssertions.Specialized;
using Sustainsys.Saml2.Metadata.Xml;

namespace Sustainsys.Saml2.Tests.Helpers;
public static class ExceptionAssertionsExtensions
{
    public static ExceptionAssertions<Saml2XmlException>
        WithErrors(this ExceptionAssertions<Saml2XmlException> assertion, params ErrorReason[] expected)
    {
        assertion.Which.Errors.Select(e => e.Reason).Should().BeEquivalentTo(expected);

        return assertion;
    }
}
