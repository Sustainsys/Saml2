using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Sustainsys.Saml2.Tests.Internal
{
    namespace Sustainsys.Saml2.Tests.Internal
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
                client.HasCertificatesInRequest.Should().BeFalse();
            }

            [TestMethod]
            public void Create_WithCertificate_ShouldAddCertificateToRequest()
            {
                var config = SustainsysSaml2Section.Current;
                var options = new SPOptions(config);

                var client = new TestableClientCertificateWebClient(
                    options.ServiceCertificates
                    .Where(sc => sc.Use.HasFlag(CertificateUse.TlsClient))
                    .Select(sc => sc.Certificate));
                    
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
                client.HasCertificatesInRequest.Should().BeTrue();
            }

            private class TestableClientCertificateWebClient : ClientCertificateWebClient
            {
                public bool HasCertificatesInRequest;
                private readonly IList<X509Certificate2> expectedCertificates;
                public TestableClientCertificateWebClient(IEnumerable<X509Certificate2> certificates) 
                    : base(certificates)
                {
                    expectedCertificates = certificates?.ToList(); // Copy to not rely on shared ref that can be altered.
                }
                protected override WebRequest GetWebRequest(Uri address)
                {
                    var request = base.GetWebRequest(address);
                    var httpWebRequest = (HttpWebRequest)request;
                    if (httpWebRequest != null)
                    {
                        HasCertificatesInRequest = 
                            expectedCertificates != null
                            && expectedCertificates.Count == httpWebRequest.ClientCertificates.Count
                            && expectedCertificates.All(ec => httpWebRequest.ClientCertificates.Contains(ec));
                    }
                    return request;
                }
            }
        }
    }
}
