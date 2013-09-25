using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    static class StatusCodeHelper
    {
        static readonly IDictionary<string, Saml2StatusCode> fromString =
            new Dictionary<string, Saml2StatusCode>()
        {
            { "urn:oasis:names:tc:SAML:2.0:status:Success", Saml2StatusCode.Success },
            { "urn:oasis:names:tc:SAML:2.0:status:Requester", Saml2StatusCode.Requester },
            { "urn:oasis:names:tc:SAML:2.0:status:Responder", Saml2StatusCode.Responder },
            { "urn:oasis:names:tc:SAML:2.0:status:VersionMismatch", Saml2StatusCode.VersionMismatch }
        };

        public static Saml2StatusCode FromString(string statusString)
        {
            return fromString[statusString];
        }
    }
}
