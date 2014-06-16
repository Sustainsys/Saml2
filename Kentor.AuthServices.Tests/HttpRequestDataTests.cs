using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class HttpRequestDataTests
    {
        [TestMethod]
        public void HttpRequestData_Ctor_Nullcheck()
        {
            Action a = () => new HttpRequestData(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("request");
        }
    }
}
