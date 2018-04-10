using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization;
using Sustainsys.Saml2.Exceptions;

namespace Sustainsys.Saml2.Tests.Exceptions
{
    [TestClass]
    public class Saml2ExceptionTests
    {
        [Serializable]
        private class ConcreteSaml2Exception : Saml2Exception
        {
            public ConcreteSaml2Exception()
                : base()
            { }

            public ConcreteSaml2Exception(SerializationInfo info, StreamingContext context)
                : base(info, context)
            { }
        }

        [TestMethod]
        public void Saml2Exception_DefaultCtor()
        {
            ExceptionTestHelpers.TestDefaultCtor<ConcreteSaml2Exception>();
        }

        [TestMethod]
        public void Saml2Exception_SerializationCtor()
        {
            ExceptionTestHelpers.TestSerializationCtor<ConcreteSaml2Exception>();
        }
    }
}
