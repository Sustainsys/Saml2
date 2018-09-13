using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Sustainsys.Saml2.Internal;
using System.Collections.Specialized;
using System.Web;
using System.Linq;

namespace Sustainsys.Saml2.Tests.Internal
{
    [TestClass]
    public class QueryStringHelperTests
    {
        [TestMethod]
        public void QueryStringHelper_ParseQueryString_ThrowsOnNull()
        {
            Action a = () => QueryStringHelper.ParseQueryString(null);
            a.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void QueryStringHelper_ParseQueryString_ShouldSplit()
        {
            var expected = new[]
            {
                new { key = "fname", value = "john" },
                new { key = "lname", value = "doe" }
            }.ToLookup(x => x.key, y => y.value);

            var collection = QueryStringHelper.ParseQueryString("fname=john&lname=doe");
            collection.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void QueryStringHelper_ParseQueryString_ShouldRemoveQuestionmark()
        {
            var expected = new[]
            {
                 new { key = "fname", value = "john" },
                new { key = "lname", value = "doe" }
            }.ToLookup(x => x.key, y => y.value);

            var collection = QueryStringHelper.ParseQueryString("?fname=john&lname=doe");
            collection.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void QueryStringHelper_ParseQueryString_ShouldCreateEmptyOnEmptyString()
        {
            var collection = QueryStringHelper.ParseQueryString("");
            collection.Should().BeEmpty();
        }

        [TestMethod]
        public void QueryStringHelper_ParseQueryString_HandlesKeyWithoutValue()
        {
            var subject = QueryStringHelper.ParseQueryString("?fname&lname=doe");

            var expected = new[]
            {
                new { key = "fname", value = (string)null },
                new { key = "lname", value="doe" }
            }.ToLookup(x => x.key, x => x.value);

            subject.Should().BeEquivalentTo(expected);
        }
    }
}
