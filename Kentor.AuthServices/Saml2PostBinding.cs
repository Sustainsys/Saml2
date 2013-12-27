using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace Kentor.AuthServices
{
    class Saml2PostBinding : Saml2Binding
    {
        protected override bool CanUnbind(System.Web.HttpRequestBase request)
        {
            return request.HttpMethod == "POST" 
                && request.Form.AllKeys.Contains("SAMLResponse");
        }

        public override Saml2Response Unbind(HttpRequestBase request)
        {
            var xml = Encoding.UTF8.GetString(
                Convert.FromBase64String(request.Form["SAMLResponse"]));

            return Saml2Response.Read(xml);            
        }

        public override CommandResult Bind(ISaml2Message request)
        {
            var encodedXml = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(request.ToXml()));

            var cr = new CommandResult()
            {
                Body = String.Format(CultureInfo.InvariantCulture, PostHtmlFormatStrign, 
                request.DestinationUri, encodedXml)
            };

            return cr;
        }

        private const string PostHtmlFormatStrign = @"<!DOCTYPE html>
<html>
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
<input type=""hidden"" name=""SAMLRequest"" 
value=""{1}""/>
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
