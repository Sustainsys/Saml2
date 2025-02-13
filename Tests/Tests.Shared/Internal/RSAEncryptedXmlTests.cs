using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography.Xml;
using System.Xml;
using FluentAssertions;
using Sustainsys.Saml2.Internal;
using System.Linq;

namespace Sustainsys.Saml2.Tests.Internal
{
  
    [TestClass]
    public class RSAEncryptedXmlTests
    {
        [TestMethod]
        public void AesGcmNonceIsSupported()
        {
            var aesGcm128NonceSizeInBytes = RSAEncryptedXml.AesGcm128NonceSizeInBits / 8;
            var dummyBytes = Enumerable.Range(42, aesGcm128NonceSizeInBytes + 1).Select(i => (byte)i).ToArray();
            var expectedIv = Enumerable.Range(42, aesGcm128NonceSizeInBytes);
            var dummyData = new EncryptedData();
            dummyData.CipherData = new CipherData(dummyBytes);
            var rex = new RSAEncryptedXml(new XmlDocument(), null );
            var iv = rex.GetDecryptionIV(dummyData, AesGcmAlgorithm.AesGcm128Identifier);
            iv.Should().NotBeNull();
            iv.Should().BeEquivalentTo(expectedIv);
        }
        
        [TestMethod]
        public void NonAesGcmAlgorithmsAreHandledByBaseClass()
        {
            var dummyBytes = Enumerable.Range(17, 20).Select(i => (byte)i).ToArray();
            var dummyData = new EncryptedData();
            dummyData.CipherData = new CipherData(dummyBytes);
            var rex = new RSAEncryptedXml(new XmlDocument(), null );
            var iv = rex.GetDecryptionIV(dummyData, EncryptedXml.XmlEncAES256Url);
            iv.Should().NotBeNull();
            iv.Should().BeEquivalentTo(Enumerable.Range(17, 16));
        }
    }
}