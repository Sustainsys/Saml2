using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Configuration;
using FluentAssertions;

namespace Kentor.AuthServices.Tests.Configuration
{
    [TestClass]
    public class IdentityProviderCollectionTests
    {
        [TestMethod]
        public void IdentityProviderCollection_RegisterIdentityProvider_NullCheck()
        {
            var subject = new IdentityProviderCollection();

            Action a = () => subject.RegisterIdentityProviders(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("options");
        }
    }
}
