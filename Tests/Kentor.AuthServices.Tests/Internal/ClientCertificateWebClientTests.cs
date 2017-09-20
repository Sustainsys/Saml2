using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kentor.AuthServices.Tests.Internal
{
    [TestClass]
    public class ClientCertificateWebClientTests
    {
        [TestMethod]
        public void Create_WithoutCertificate_ShouldAddNothingToRequest()
        {
            var client = new TestableClientCertificateWebClient(null);
            var payload = "Doesn't matter";
            var destination = new Uri("https://localhost/Endpoint");

            try
            {
                client.UploadString(destination, payload);
            }
            catch (Exception)
            {
                //Destination is not listening, but we should get an exception that shows it
                // at least tried to connect there.
            }

            client.HasCertificateInRequest.ShouldBeEquivalentTo(false);
        }

        [TestMethod]
        public void Create_WithCertificate_ShouldAddCertificateToRequest()
        {
            var config = KentorAuthServicesSection.Current;
            var options = new SPOptions(config);

            var client = new TestableClientCertificateWebClient(options.ArtifactResolutionTlsCertificate);
            var payload = "Doesn't matter";
            var destination = new Uri("https://localhost/Endpoint");

            try
            {
                client.UploadString(destination, payload);
            }
            catch (Exception)
            {
                //Destination is not listening, but we should get an exception that shows it
                // at least tried to connect there.
            }

            client.HasCertificateInRequest.ShouldBeEquivalentTo(true);
        }

        private class TestableClientCertificateWebClient : ClientCertificateWebClient
        {
            public bool HasCertificateInRequest;

            private readonly X509Certificate2 _expectedCertificate;

            public TestableClientCertificateWebClient(X509Certificate2 certificate) : base(certificate)
            {
                _expectedCertificate = certificate;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address);

                var httpWebRequest = (HttpWebRequest) request;
                if (httpWebRequest != null)
                {
                    HasCertificateInRequest = httpWebRequest.ClientCertificates.Contains(_expectedCertificate);
                }

                return request;
            }
        }
    }
}