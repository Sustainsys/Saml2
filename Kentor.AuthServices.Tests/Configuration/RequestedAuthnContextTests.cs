using FluentAssertions;
using Kentor.AuthServices.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Kentor.AuthServices.Tests.Configuration
{
    [TestClass]
    public class RequestedAuthnContextTests
    {
        [TestMethod]
        public void RequestedAuthnContext_Ctor_Nullcheck()
        {
            Action a = () => new RequestedAuthnContext(null);

            a.ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("requestedAuthnContextElement");
        }

        [TestMethod]
        public void RequestedAuthnContext_Ctor_HandlesFullUri()
        {
            var config = new RequestedAuthnContextElement();
            config.AllowChange(true);
            var classRef = "http://id.sambi.se/loa2";
            config.AuthnContextClassRef = classRef;

            var subject = new RequestedAuthnContext(config);
            subject.ClassRef.Should().Be(classRef);
        }
    }
}
