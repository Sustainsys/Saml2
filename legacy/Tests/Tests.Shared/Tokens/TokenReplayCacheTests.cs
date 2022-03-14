using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sustainsys.Saml2.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sustainsys.Saml2.Tests.Tokens
{
    [TestClass]
    public class TokenReplayCacheTests
    {
        [TestMethod]
        public void TokenReplayCache_TryAdd()
        {
            var subject = new TokenReplayCache();

            var key = "key";

            subject.TryAdd(key, DateTime.UtcNow.Add(TimeSpan.FromDays(1)))
                .Should().BeTrue();
        }

        [TestMethod]
        public void TokenReplayCache_TryFind_False()
        {
            var subject = new TokenReplayCache();

            var key = "key";

            subject.TryFind(key).Should().BeFalse();
        }

        [TestMethod]
        public void TokenReplayCache_TryFind_True()
        {
            var subject = new TokenReplayCache();

            var key = "key";

            subject.TryAdd(key, DateTime.UtcNow.Add(TimeSpan.FromDays(1)));

            subject.TryFind(key).Should().BeTrue();
        }
    }
}
