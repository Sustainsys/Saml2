using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class AuthServicesExceptionTests
    {
        private class ConcreteAuthServicesException : AuthServicesException
        {
            public ConcreteAuthServicesException()
                : base()
            { }
        }

        [TestMethod]
        public void TestBasicCtor()
        {
            ExceptionTestHelpers.TestDefaultCtor<ConcreteAuthServicesException>();
        }
    }
}
