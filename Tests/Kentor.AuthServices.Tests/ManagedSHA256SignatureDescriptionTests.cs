using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class ManagedSHA256SignatureDescriptionTests
    {
        [TestMethod]
        public void ManagedSha256SignatureDescription_CreateDeformatter_NullCheck()
        {
            new ManagedSHA256SignatureDescription()
                .Invoking(d => d.CreateDeformatter(null))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("key");
        }

        [TestMethod]
        public void ManagedSha256SignatureDescription_CreateFormatter_NullCheck()
        {
            new ManagedSHA256SignatureDescription()
                .Invoking(d => d.CreateFormatter(null))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("key");
        }
    }
}
