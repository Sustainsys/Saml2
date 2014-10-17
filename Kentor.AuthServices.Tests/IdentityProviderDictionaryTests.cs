using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Configuration;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class IdentityProviderDictionaryTests
    {
        [TestMethod]
        public void IdentityProviderDictionary_Indexer_Nullcheck()
        {
            var subject = new IdentityProviderDictionary();

            Action a = () => { var i = subject[null]; };

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("entityId");
        }
    }
}
