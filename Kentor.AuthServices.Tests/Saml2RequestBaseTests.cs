using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Text.RegularExpressions;

namespace Kentor.AuthServices.Tests
{
    class ConcreteSaml2Request : Saml2RequestBase { }

    [TestClass]
    public class Saml2RequestBaseTests
    {
        [TestMethod]
        public void Saml2RequestBase_Id_IsUnique()
        {
            var r1 = new ConcreteSaml2Request();
            var r2 = new ConcreteSaml2Request();

            r1.Id.Should().NotBe(r2.Id);
        }

        [TestMethod]
        public void Saml2RequestBase_Id_IsValidXsId()
        {
            var id = new ConcreteSaml2Request().Id;

            Regex.IsMatch(id, "[^:0-9][^:]*").Should().BeTrue();
        }

        [TestMethod]
        public void Saml2RequestBase_Version()
        {
            new ConcreteSaml2Request().Version.Should().Be("2.0");
        }
    }
}
