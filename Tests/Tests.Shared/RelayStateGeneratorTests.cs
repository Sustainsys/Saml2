using Sustainsys.Saml2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Sustainsys.Saml2.Tests
{
    [TestClass]
    public class SecureKeyGeneratorTests
    {
        [TestMethod]
        public void SecureKeyGenerator_CreateRelayState()
        {
            // Loop until we've seen the replacement work.
            var containedDash = false;
            var containedUnderscore = false;
            for(int i = 0; !containedDash || !containedUnderscore; i++)
            {
                i.Should().BeLessThan(1000, because: "if replacement works, we should have found the replacement characters sooner");
                var result = SecureKeyGenerator.CreateRelayState();

                // Can't really test a random algo any better than expecting a
                // specific length of the result and the right chars.
                result.Length.Should().Be(24);

                containedDash = containedDash || result.Contains("-");
                containedUnderscore = containedUnderscore || result.Contains("_");

                // Resulting string should not need to be URL encoded.
                result.Should().MatchRegex("^[A-Za-z0-9\\-_]*$");
            }
        }
    }
}
