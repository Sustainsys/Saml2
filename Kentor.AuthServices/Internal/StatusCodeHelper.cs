using Kentor.AuthServices.Saml2P;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Internal
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

        static readonly IDictionary<Saml2StatusCode, string> fromCode =
            fromString.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        public static Saml2StatusCode FromString(string statusString)
        {
            return fromString[statusString];
        }

        public static string FromCode(Saml2StatusCode statusCode)
        {
            return fromCode[statusCode];
        }
    }
}
