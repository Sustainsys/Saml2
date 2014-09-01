using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class InvalidMetadataExceptionTests
    {
        [TestMethod]
        public void InvalidMetadataException_DefaultCtor()
        {
            ExceptionTestHelpers.TestDefaultCtor<InvalidMetadataException>();
        }

        [TestMethod]
        public void InvalidMetadataException_SerializationCtor()
        {
            ExceptionTestHelpers.TestSerializationCtor<InvalidMetadataException>();
        }

        [TestMethod]
        public void InvalidMetadataException_StringCtor()
        {
            var msg = "Message!";
            var subject = new InvalidMetadataException(msg);

            subject.Message.Should().Be(msg);
        }

        [TestMethod]
        public void InvalidMetadataException_InnerExceptionCtor()
        {
            var inner = new Exception("inner");
            var msg = "Message!";

            var subject = new InvalidMetadataException(msg, inner);

            subject.Message.Should().Be(msg);
            subject.InnerException.Should().Be(inner);
        }
    }
}
