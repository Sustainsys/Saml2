using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// Saml2 Soap binding implementation.
    /// </summary>
    /// <remarks>
    /// This class does not follow the pattern of the other three bindings
    /// (Redirect, POST and Artifact) because it does not use the front channel
    /// with messages being passed over the user's browser.
    /// </remarks>
    public static class Saml2SoapBinding
    {
        const string soapFormatString = "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\"><SOAP-ENV:Body>{0}</SOAP-ENV:Body></SOAP-ENV:Envelope>";

        internal static string CreateSoapBody(string payLoad)
        {
            return string.Format(CultureInfo.InvariantCulture,
                soapFormatString, payLoad);
        }
    }
}
