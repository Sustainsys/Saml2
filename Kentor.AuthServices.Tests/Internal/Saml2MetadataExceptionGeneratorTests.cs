using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Internal;
using FluentAssertions;

namespace Kentor.AuthServices.Tests.Internal
{
    [TestClass]
    public class Saml2MetadataExceptionGeneratorTests
    {
        [TestMethod]
        public void Saml2MetadataExceptionGenerator_SignatureMissingReturnException()
        {
            var result = (new Saml2MetadataExceptionGenerator()).SignatureMissing();
            result.Message.Should().Be("The SAML Metadata is not signed. Response cannot be trusted.");
        }

        [TestMethod]
        public void Saml2MetadataExceptionGenerator_NoReferencesReturnException()
        {
            var result = (new Saml2MetadataExceptionGenerator()).NoReferences();
            result.Message.Should().Be("No reference found in Xml signature, it doesn't validate the Xml data.");
        }

        [TestMethod]
        public void Saml2MetadataExceptionGenerator_MultipleReferencesReturnException()
        {
            var result = (new Saml2MetadataExceptionGenerator()).MultipleReferences();
            result.Message.Should().Be("Multiple references for Xml signatures are not allowed.");
        }

        [TestMethod]
        public void Saml2MetadataExceptionGenerator_IncorrectReferenceReturnException()
        {
            var result = (new Saml2MetadataExceptionGenerator()).IncorrectReference();
            result.Message.Should().Be("Incorrect reference on Xml signature. The reference must be to the root element of the element containing the signature.");
        }

        [TestMethod]
        public void Saml2MetadataExceptionGenerator_DisallowedTransformReturnException()
        {
            var result = (new Saml2MetadataExceptionGenerator()).DisallowedTransform("test");
            result.Message.Should().Be("Transform \"" + "test" + "\" found in Xml signature is not allowed in SAML.");
        }

        [TestMethod]
        public void Saml2MetadataExceptionGenerator_SignatureValidationFailReturnException()
        {
            var result = (new Saml2MetadataExceptionGenerator()).SignatureValidationFail();
            result.Message.Should().Be("Signature validation failed on SAML metadata.");
        }
    }
}
