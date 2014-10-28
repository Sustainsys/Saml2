using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace Kentor.AuthServices.WebSSO
{
    class Saml2PostBinding : Saml2Binding
    {
        protected internal override bool CanUnbind(HttpRequestData request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            return request.HttpMethod == "POST"
                && request.Form.Keys.Contains("SAMLResponse");
        }

        public override string Unbind(HttpRequestData request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var xml = Encoding.UTF8.GetString(
                Convert.FromBase64String(request.Form["SAMLResponse"]));

            return xml;
        }

        public override CommandResult Bind(string payload, Uri destinationUri, string messageName)
        {
            if (payload == null)
            {
                throw new ArgumentNullException("payload");
            }
            if (destinationUri == null)
            {
                throw new ArgumentNullException("destinationUri");
            }
            if (messageName == null)
            {
                throw new ArgumentNullException("messageName");
            }

            var encodedXml = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(payload));

            var cr = new CommandResult()
            {
                Content = String.Format(CultureInfo.InvariantCulture, PostHtmlFormatString,
               destinationUri, messageName, encodedXml)
            };

            return cr;
        }

        private const string PostHtmlFormatString = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN""
""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"">
<body onload=""document.forms[0].submit()"">
<noscript>
<p>
<strong>Note:</strong> Since your browser does not support JavaScript, 
you must press the Continue button once to proceed.
</p>
</noscript>
<form action=""{0}"" 
method=""post"">
<div>
<input type=""hidden"" name=""{1}"" 
value=""{2}""/>
</div>
<noscript>
<div>
<input type=""submit"" value=""Continue""/>
</div>
</noscript>
</form>
</body>
</html>";
    }
}
