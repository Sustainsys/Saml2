using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace Kentor.AuthServices
{
    class Saml2PostBinding : Saml2Binding
    {
        protected internal override bool CanUnbind(System.Web.HttpRequestBase request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            return request.HttpMethod == "POST" 
                && request.Form.AllKeys.Contains("SAMLResponse");
        }

        public override Saml2Response Unbind(HttpRequestBase request)
        {
            if(request == null)
            {
                throw new ArgumentNullException("request");
            }

            var xml = Encoding.UTF8.GetString(
                Convert.FromBase64String(request.Form["SAMLResponse"]));

            return Saml2Response.Read(xml);            
        }

        public override CommandResult Bind(ISaml2Message message)
        {
            if(message == null)
            {
                throw new ArgumentNullException("message");
            }

            var encodedXml = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(message.ToXml()));

            var cr = new CommandResult()
            {
                Content = String.Format(CultureInfo.InvariantCulture, PostHtmlFormatString,
                message.DestinationUri, message.MessageName, encodedXml)
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
