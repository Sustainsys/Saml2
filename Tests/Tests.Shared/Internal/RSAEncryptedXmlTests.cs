using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography.Xml;
using System.Xml;
using FluentAssertions;
using Sustainsys.Saml2.Internal;

namespace Sustainsys.Saml2.Tests.Internal
{
  
    [TestClass]
    public class RSAEncryptedXmlTests
    {
        [TestMethod]
        public void AesGcmNonceIsSupported()
        {
            var aesGcm128NonceSizeInBytes = RSAEncryptedXml.AesGcm128NonceSizeInBits / 8;
            var dummyBytes = new byte[aesGcm128NonceSizeInBytes + 1];
            var expectedIv = new byte[aesGcm128NonceSizeInBytes];
            var dummyData = new EncryptedData();
            dummyData.CipherData = new CipherData(dummyBytes);
            var rex = new RSAEncryptedXml(new XmlDocument(), null );
            var iv = rex.GetDecryptionIV(dummyData, RSAEncryptedXml.AesGcm128Identifier);
            iv.Should().NotBeNull();
            iv.Should().BeEquivalentTo(expectedIv);
        }
        
        [TestMethod]
        public void NonAesGcmAlgorithmsAreHandledByBaseClass()
        {
            var dummyBytes = new byte[20];
            var dummyData = new EncryptedData();
            dummyData.CipherData = new CipherData(dummyBytes);
            var rex = new RSAEncryptedXml(new XmlDocument(), null );
            var iv = rex.GetDecryptionIV(dummyData, EncryptedXml.XmlEncAES256Url);
            iv.Should().NotBeNull();
        }
    }
}