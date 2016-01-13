using Kentor.AuthServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class RelayStateGeneratorTests
    {
        [TestMethod]
        public void RelayStateGenerator_GetSecureKey()
        {
            // Loop until we've seen the replacement work.
            var containedDash = false;
            var containedUnderscore = false;
            for(int i = 0; !containedDash || !containedUnderscore; i++)
            {
                i.Should().BeLessThan(1000, because: "if replacement works, we should have found the replacement characters sooner");
                var result = RelayStateGenerator.CreateSecureKey();

                // Can't really test a random algo any better than expecting a
                // specific length of the result and the right chars.
                result.Length.Should().Be(56);

                containedDash = containedDash || result.Contains("-");
                containedUnderscore = containedUnderscore || result.Contains("_");

                // Resulting string should not need to be URL encoded.
                result.Should().MatchRegex("^[A-Za-z0-9\\-_]*$");
            }
        }
    }
}
