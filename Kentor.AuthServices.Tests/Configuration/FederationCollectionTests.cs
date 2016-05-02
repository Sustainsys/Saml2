using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Configuration;
using FluentAssertions;

namespace Kentor.AuthServices.Tests.Configuration
{
    [TestClass]
    public class FederationCollectionTests
    {
        [TestMethod]
        public void FederationCollection_RegisterFederations_NullCheck()
        {
            var subject = new FederationCollection();

            Action a = () => subject.RegisterFederations(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("options");
        }
    }
}
