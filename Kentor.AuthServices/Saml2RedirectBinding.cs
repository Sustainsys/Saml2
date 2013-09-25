using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kentor.AuthServices
{
    class Saml2RedirectBinding : Saml2Binding
    {
        public override CommandResult Bind(Saml2AuthenticationRequest request)
        {
            var serializedReqeust = Serialize(request);

            var redirectUri = new Uri(request.DestinationUri.ToString() 
                + "?SAMLRequest=" + serializedReqeust);

            return new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.Found,
                Location = redirectUri
            };
        }

        public override bool CanUnbind(HttpRequestBase request)
        {
            return false;
        }

        // The MemoryStream is not disposed by the DeflateStream - we're using the keep-open flag.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private static string Serialize(Saml2AuthenticationRequest request)
        {
            using (var compressed = new MemoryStream())
            {
                using (var writer = new StreamWriter(new DeflateStream(compressed, CompressionLevel.Optimal, true)))
                {
                    writer.Write(request.ToXElement().ToString());
                }

                return HttpUtility.UrlEncode(Convert.ToBase64String(compressed.GetBuffer()));
            }
        }
    }
}
