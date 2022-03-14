using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sustainsys.Saml2.Metadata;

namespace Sustainsys.Saml2.Tests
{
    [TestClass]
    public class ManagedSHA256SignatureDescriptionTests
    {
        [TestMethod]
        public void ManagedSha256SignatureDescription_CreateDeformatter_NullCheck()
        {
            new ManagedRSASHA256SignatureDescription()
                .Invoking(d => d.CreateDeformatter(null))
                .Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("key");
        }

        [TestMethod]
        public void ManagedSha256SignatureDescription_CreateFormatter_NullCheck()
        {
            new ManagedRSASHA256SignatureDescription()
                .Invoking(d => d.CreateFormatter(null))
                .Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("key");
        }
    }
}
