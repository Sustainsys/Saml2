using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class InvalidSamlOperationExceptionTest
    {
        [TestMethod]
        public void InvalidSamlOperationException_DefaultCtor()
        {
            ExceptionTestHelpers.TestDefaultCtor<InvalidSamlOperationException>();
        }

        [TestMethod]
        public void InvalidSamlOperationException_SerializationCtor()
        {
            ExceptionTestHelpers.TestSerializationCtor<InvalidSamlOperationException>();
        }

        [TestMethod]
        public void InvalidSamlOperationException_StringCtor()
        {
            var msg = "Message!";
            var subject = new InvalidSamlOperationException(msg);

            subject.Message.Should().Be(msg);
        }
        [TestMethod]
        public void InvalidSamlOperationException_StringInnerExCtor()
        {
            var msg = "Message!";
            var inner = new Exception();
            var subject = new InvalidSamlOperationException(msg, inner);

            subject.Message.Should().Be(msg);
            subject.InnerException.Should().Be(inner);
        }

    }
}
