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
        public void FedrationCollection_RegisterFedartions_NullCheck()
        {
            var subject = new FederationCollection();

            Action a = () => subject.RegisterFederations(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("options");
        }
    }
}
