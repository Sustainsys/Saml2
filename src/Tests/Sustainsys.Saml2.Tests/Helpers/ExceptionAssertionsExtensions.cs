using FluentAssertions;
using FluentAssertions.Specialized;
using Sustainsys.Saml2.Xml;

namespace Sustainsys.Saml2.Tests.Helpers;
public static class ExceptionAssertionsExtensions
{
    public static ExceptionAssertions<SamlXmlException>
        WithErrors(this ExceptionAssertions<SamlXmlException> assertion, params ErrorReason[] expected)
    {
        assertion.Which.Errors.Select(e => e.Reason).Should().BeEquivalentTo(expected);

        return assertion;
    }
}