using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Kentor.AuthServices.Internal
{
    /// <summary>
    /// A WebClient implementation that will add
    /// a client certificate to the requests it makes.
    /// </summary>
    internal class ClientCertificateWebClient : WebClient
    {
        private readonly X509Certificate2 certificate;

        /// <summary>
        /// Register the certificate to be used for this requets.
        /// </summary>
        /// <param name="certificate"></param>
        public ClientCertificateWebClient(X509Certificate2 certificate)
        {
            this.certificate = certificate;
        }

        /// <summary>
        /// Override the base class to add the certificate 
        /// to the reuqest before returning it.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);

            if (certificate != null)
            {
                request.ClientCertificates.Add(certificate);
            }

            return request;
        }
    }
}