using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Kentor.AuthServices.Exceptions;

namespace Kentor.AuthServices.Tests.Exceptions
{
    [TestClass]
    public class UnsuccessfulSamlOperationExceptionTest
    {
        [TestMethod]
        public void InvalidSamlOperationException_DefaultCtor()
        {
            ExceptionTestHelpers.TestDefaultCtor<UnsuccessfulSamlOperationException>();
        }

        [TestMethod]
        public void InvalidSamlOperationException_SerializationCtor()
        {
            ExceptionTestHelpers.TestSerializationCtor<UnsuccessfulSamlOperationException>();
        }

        [TestMethod]
        public void InvalidSamlOperationException_StringCtor()
        {
            var msg = "Message!";
            var subject = new UnsuccessfulSamlOperationException(msg);

            subject.Message.Should().Be(msg);
        }
        [TestMethod]
        public void InvalidSamlOperationException_StringInnerExCtor()
        {
            var msg = "Message!";
            var inner = new Exception();
            var subject = new UnsuccessfulSamlOperationException(msg, inner);

            subject.Message.Should().Be(msg);
            subject.InnerException.Should().Be(inner);
        }

    }
}
