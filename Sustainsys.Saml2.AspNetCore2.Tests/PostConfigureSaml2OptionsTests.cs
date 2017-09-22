using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;

namespace Sustainsys.Saml2.AspNetCore2.Tests
{
    [TestClass]
    public class PostConfigureSaml2OptionsTests
    {
        [TestMethod]
        public void PostConfigureSaml2Options_PostConfigure_Nullcheck()
        {
            var subject = new PostConfigureSaml2Options();

            subject.Invoking(s => s.PostConfigure(null, null))
                .ShouldThrow<ArgumentNullException>().
                And.ParamName.Should().Be("options");
        }
    }
}
