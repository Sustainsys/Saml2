using System;
using System.Text;
using FluentAssertions;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Tokens;

namespace Kentor.AuthServices.Tests.Metadata
{
    /// <summary>
    /// Summary description for X509FromFilrCertificateValidatorTests
    /// </summary>
    [TestClass]
    public class X509FromFilrCertificateValidatorTests
    {
        public X509FromFilrCertificateValidatorTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void X509FromFileCertificateValidator_Ctor_ShouldThrowOnNull()
        {
            Action a = () => new X509FromFileCertificateValidator(null);
            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("onFileCertificate");
        }

        [TestMethod]
        public void X509FRomFileCertificateValidator_Validate_ShouldFailOnDifferentCert()
        {
            var originalCert = new X509Certificate2("Kentor.AuthServices.Tests.pfx");
            var wrongCert = new X509Certificate2("Kentor.AuthServices.Badcertificate.pfx", "password");
            var validator = new X509FromFileCertificateValidator(originalCert);

            Action a = () => validator.Validate(wrongCert);
            a.ShouldThrow<SecurityTokenValidationException>();
        }

        [TestMethod]
        public void X509FRomFileCertificateValidator_Validate_ShouldSucceedOnSameCert()
        {
            var originalCert = new X509Certificate2("Kentor.AuthServices.Tests.pfx");
            var rightCert = new X509Certificate2("Kentor.AuthServices.Tests.pfx");
            var validator = new X509FromFileCertificateValidator(originalCert);

            Action a = () => validator.Validate(rightCert);
            a.ShouldNotThrow<SecurityTokenValidationException>();
        }

        [TestMethod]
        public void X509FRomFileCertificateValidator_Validate_ShouldThrowOnNull()
        {
            var originalCert = new X509Certificate2("Kentor.AuthServices.Tests.pfx");
            var validator = new X509FromFileCertificateValidator(originalCert);

            Action a = () => validator.Validate(null);
            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("certificate");
        }
    }
}
