using FluentAssertions;
using Sustainsys.Saml2.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Tests.Internal
{
    [TestClass]
    public class DelimitedStringTests
    {
        [TestMethod]
        public void DelimitedString_Join()
        {
            var actual = DelimitedString.Join("aaa", "b/b", "c,c", "d/,d", "/", "e");

            actual.Should().Be("aaa,b//b,c/,c,d///,d,//,e");
        }

        [TestMethod]
        public void DelimitedString_Split_SimpleWithoutEscaping()
        {
            var actual = DelimitedString.Split("aa,bb,cc");

            var expected = new[] { "aa", "bb", "cc" };

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void DelimitedString_Split_EmptyStrings()
        {
            var actual = DelimitedString.Split(",aa,,bb,");

            var expected = new[] { "", "aa", "", "bb", "" };

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void DelimitedString_Split_EscapeCharaters()
        {
            var actual = DelimitedString.Split("a/,b//c,/,,//,");
            var expected = new[] { "a,b/c", ",", "/", "" };

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void DelimitedString_Split_EndsWithEscapedDelimiter()
        {
            var actual = DelimitedString.Split("a/,");

            var expected = new[] { "a," };

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
