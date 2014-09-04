using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace Kentor.AuthServices.StubIdp.Controllers
{
    public class MetadataController : Controller
    {
        private string CreateMetadata()
        {
            var metadata = new EntityDescriptor()
            {
                EntityId = new EntityId(UrlResolver.MetadataUrl.ToString())
            };

            var idpSsoDescriptor = new IdentityProviderSingleSignOnDescriptor();
            idpSsoDescriptor.ProtocolsSupported.Add(new Uri("urn:oasis:names:tc:SAML:2.0:protocol"));
            metadata.RoleDescriptors.Add(idpSsoDescriptor);

            idpSsoDescriptor.SingleSignOnServices.Add(new ProtocolEndpoint()
            {
                Binding = Saml2Binding.HttpRedirectUri,
                Location = UrlResolver.SsoServiceUrl
            });

            idpSsoDescriptor.Keys.Add(CertificateHelper.SigningKey);

            var serializer = new MetadataSerializer();

            using (var stream = new MemoryStream())
            {
                serializer.WriteMetadata(stream, metadata);
                
                using(var reader = new StreamReader(stream))
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    return reader.ReadToEnd();
                }
            }
        }
        
        // GET: Metadata
        public ActionResult Index()
        {
            return Content(CreateMetadata(), "application/samlmetadata+xml");
        }

        public ActionResult BrowserFriendly()
        {
            return Content(CreateMetadata(), "text/xml");
        }
    }
}