using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Tests.Helpers;
using System.Security.Claims;

namespace Sustainsys.Saml2.Tests;

public class ClaimsFactoryTests
{
    [Fact]
    public void GetClaimsIdentity()
    {
        var subject = new ClaimsFactory();

        Assertion assertion = new()
        {
            Subject = new()
            {
                NameId = new()
                {
                    Value = "NameIdValue"
                }
            },
            Attributes =
            {
                // Single valued attribute
                { "email", "john.doe@example.com" },
                // Multi valued attribute
                { "role", "admin", "boss" }
                // TODO: Test for multiple attribute statements with same attribute name
            }
        };

        var actual = subject.GetClaimsIdentity(assertion.FakeValidate());

        Claim[] expectedClaims =
            [
                new(ClaimTypes.NameIdentifier, "NameIdValue"),
                new("email", "john.doe@example.com"),
                new("role", "admin"),
                new("role", "boss")
            ];

        ClaimsIdentity expected = new ClaimsIdentity(expectedClaims, "saml2");

        actual.Should().BeEquivalentTo(
            expected,
            opt => opt.For(ci => ci.Claims).Exclude(c => c.Subject));
    }
}