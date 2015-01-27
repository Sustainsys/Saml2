using System;
using System.Text;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class MetadataFailedValidationExceptionTests
    {
        [TestMethod]
        public void MetadataFailedValidationException_DefaultCtor()
        {
            ExceptionTestHelpers.TestDefaultCtor<MetadataFailedValidationException>();
        }

        [TestMethod]
        public void MetadataFailedValidationException_SerializationCtor()
        {
            ExceptionTestHelpers.TestSerializationCtor<MetadataFailedValidationException>();
        }

        [TestMethod]
        public void MetadataFailedValidationException_StringCtor()
        {
            var msg = "Message!";
            var subject = new MetadataFailedValidationException(msg);

            subject.Message.Should().Be(msg);
        }

        [TestMethod]
        public void MetadataFailedValidationException_InnerExceptionCtor()
        {
            var msg = "Message!";
            var innerException = new InvalidOperationException("InnerMessage!");

            var subject = new MetadataFailedValidationException(msg, innerException);

            subject.Message.Should().Be("Message!");
            subject.InnerException.Message.Should().Be("InnerMessage!");
        }
    }
}
