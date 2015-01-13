using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Kentor.AuthServices.Internal;
using System.Collections.Specialized;
using System.Web;

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

        [TestMethod]
        public void QueryStringHelper_ParseQueryString_ShouldSplit()
        {
            NameValueCollection expected = new NameValueCollection() 
            {
                { "fname", "john" },
                { "lname", "doe" }
            };
            var collection = QueryStringHelper.ParseQueryString("fname=john&lname=doe");
            collection.Should().HaveCount(2, "The number of arguments that we have.").And.Equal(expected);
        }

        [TestMethod]
        public void QueryStringHelper_ParseQueryString_ShouldRemoveQuestionmark()
        {
            NameValueCollection expected = new NameValueCollection() 
            {
                { "fname", "john" },
                { "lname", "doe" }
            };
            var collection = QueryStringHelper.ParseQueryString("?fname=john&lname=doe");
            collection.Should().HaveCount(2, "The number of arguments that we have.").And.Equal(expected);
        }

        [TestMethod]
        public void QueryStringHelper_ParseQueryString_ShouldCreateEmptyOnEmptyString()
        {
            var collection = QueryStringHelper.ParseQueryString("");
            collection.Should().BeEmpty();
        }

        [TestMethod]
        public void QueryStringHelper_UrlEncode_ShouldEncode()
        {
            String test = "http://test# space 123/text?var=val&another=two";
            String expected = HttpUtility.UrlEncode(test);

            QueryStringHelper.UrlEncode(test).Should().Equals(expected);

        }
    }
}
