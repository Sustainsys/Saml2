using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.AspNetCore2.Tests
{
    [TestClass]
    public class Saml2OptionsTests
    {
        [TestMethod]
        public void Saml2Options_Ctor_Defaults()
        {
            var subject = new Saml2Options();
            subject.CallbackPath.Value.Should().Be("/Saml2/Acs");
        }
    }
}
