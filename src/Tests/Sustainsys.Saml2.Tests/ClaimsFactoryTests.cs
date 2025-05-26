using Sustainsys.Saml2.Saml;
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
                new()
                {
                    Name = "email",
                    Values = { "john.doe@example.com" }
                },
                // Multi valued attribute
                new()
                {
                    Name = "role",
                    Values = { "admin", "boss" }
                }
                // TODO: Test for multiple attribute statements with same attribute name
            }
        };

        var actual = subject.GetClaimsIdentity(assertion);

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
