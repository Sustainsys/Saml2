using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Kentor.AuthServices.Internal;
using System.Collections.Specialized;
using System.Web;
using System.Linq;

namespace Kentor.AuthServices.Tests.Internal
{
    [TestClass]
    public class QueryStringHelperTests
    {
        [TestMethod]
        public void QueryStringHelper_ParseQueryString_ThrowsOnNull()
        {
            Action a = () => QueryStringHelper.ParseQueryString(null);
            a.ShouldThrow<ArgumentNullException>();
        }

        private bool EqualityTester(IGrouping<String, String> lhs, IGrouping<String, String> rhs)
        {
            if (!lhs.Key.Equals(rhs.Key) || lhs.Count() != rhs.Count())
            {
                return false;
            }

            foreach (var val in lhs)
            {
                if (!rhs.Contains(val))
                {
                    return false;
                }
            }

            foreach (var val in rhs)
            {
                if (!lhs.Contains(val))
                {
                    return false;
                }
            }

            return true;
        }

        [TestMethod]
        public void QueryStringHelper_ParseQueryString_EmptyStringShouldCreateEmptyCollection()
        {
            var collection = QueryStringHelper.ParseQueryString("");
            collection.Should().BeEmpty();
        }

        [TestMethod]
        public void QueryStringHelper_ParseQueryString_ShouldSplit()
        {
            var expected = new [] 
            {
                new { key = "fname", value = "john" },
                new { key = "lname", value = "doe" }
            }.ToLookup(x => x.key, y => y.value);

            var collection = QueryStringHelper.ParseQueryString("fname=john&lname=doe");
            collection.Should().HaveCount(2, "The number of arguments that we have.").And.Equal(expected, EqualityTester);
        }

        [TestMethod]
        public void QueryStringHelper_ParseQueryString_ShouldRemoveQuestionmark()
        {
            var expected = new [] 
            {
                 new { key = "fname", value = "john" },
                new { key = "lname", value = "doe" }
            }.ToLookup(x => x.key, y => y.value);

            var collection = QueryStringHelper.ParseQueryString("?fname=john&lname=doe");
            collection.Should().HaveCount(2, "The number of arguments that we have.").And.Equal(expected, EqualityTester);
        }

        [TestMethod]
        public void QueryStringHelper_ParseQueryString_ShouldCreateEmptyOnEmptyString()
        {
            var collection = QueryStringHelper.ParseQueryString("");
            collection.Should().BeEmpty();
        }
    }
}
