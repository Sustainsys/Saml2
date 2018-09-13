using Sustainsys.Saml2.Saml2P;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.Xml;

namespace Sustainsys.Saml2.Tests.WebSSO
{
    class Saml2MessageImplementation : ISaml2Message
    {
        public Uri DestinationUrl { get; set; }

        public string MessageName { get; set;  }

        public string RelayState { get; set;  }

        public string ToXml()
        {
            return XmlData;
        }

        public string XmlData { get; set; }

        public X509Certificate2 SigningCertificate { get; set; }

        public string SigningAlgorithm { get; set; } = SecurityAlgorithms.RsaSha256Signature;

        public EntityId Issuer { get; set; }
    }
}
