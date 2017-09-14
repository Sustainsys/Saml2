using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization;
using Kentor.AuthServices.Exceptions;

namespace Kentor.AuthServices.Tests.Exceptions
{
    [TestClass]
    public class AuthServicesExceptionTests
    {
        [Serializable]
        private class ConcreteAuthServicesException : AuthServicesException
        {
            public ConcreteAuthServicesException()
                : base()
            { }

            public ConcreteAuthServicesException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            { }
        }

        [TestMethod]
        public void AuthServicesException_DefaultCtor()
        {
            ExceptionTestHelpers.TestDefaultCtor<ConcreteAuthServicesException>();
        }

        [TestMethod]
        public void AuthServicesException_SerializationCtor()
        {
            ExceptionTestHelpers.TestSerializationCtor<ConcreteAuthServicesException>();
        }
    }
}
