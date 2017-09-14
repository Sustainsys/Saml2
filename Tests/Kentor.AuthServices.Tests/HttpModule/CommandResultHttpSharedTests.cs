using FluentAssertions;
using Kentor.AuthServices.HttpModule;
using Kentor.AuthServices.WebSso;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Web;

namespace Kentor.AuthServices.Tests.HttpModule
{
    [TestClass]
    public partial class CommandResultHttpTests
    {
        [TestMethod]
        public void CommandResultHttp_ApplyCookies_NullCheck_CommandResult()
        {
            ((CommandResult)null)
                .Invoking(cr => cr.ApplyCookies(Substitute.For<HttpResponseBase>()))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("commandResult");
        }

        [TestMethod]
        public void CommandResultHttp_ApplyCookies_NullCheck_Response()
        {
            new CommandResult()
                .Invoking(cr => cr.ApplyCookies(null))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("response");
        }
    }
}
