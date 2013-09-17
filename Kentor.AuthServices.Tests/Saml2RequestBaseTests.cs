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

        [TestMethod]
        public void Saml2RequestBase_IssueInstant_Format()
        {
            Regex.IsMatch(new ConcreteSaml2Request().IssueInstant,
                "20[0-9]{2}-((0[0-9])|(1[12]))-(([0-2][0-9])|(3[0-1]))T(([01][0-9])|(2[0-3]))(:[0-5][0-9]){2}Z")
                .Should().BeTrue();
        }

        [TestMethod]
        public void Saml2RequestBase_IssueInstant_IsNow()
        {
            var issueInstant = new ConcreteSaml2Request().IssueInstant;

            var parsed = DateTime.Parse(issueInstant).ToUniversalTime();

            parsed.Should().BeCloseTo(DateTime.UtcNow, 1200);
            parsed.Kind.Should().Be(DateTimeKind.Utc);
        }
    }
}
