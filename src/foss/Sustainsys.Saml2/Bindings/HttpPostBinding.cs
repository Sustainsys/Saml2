// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Sustainsys.Saml2.Xml;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Sustainsys.Saml2.Bindings;

/// <summary>
/// Saml Http POST Binding
/// </summary>
public class HttpPostBinding : FrontChannelBinding
{
    /// <summary>
    /// Constructor
    /// </summary>
    public HttpPostBinding() : base(Constants.BindingUris.HttpPOST) { }

    /// <inheritdoc/>
    public override bool CanUnBind(HttpRequest httpRequest)
        => httpRequest.Method == "POST"
        && httpRequest.Form.Keys.Any(
            k => k == Constants.SamlRequest || k == Constants.SamlResponse);

    /// <inheritdoc/>
    public override Task<Saml2Message> UnBindAsync(
        HttpRequest httpRequest,
        Func<string, Task<Saml2Entity>> getSaml2Entity)
    {
        if (!CanUnBind(httpRequest))
        {
            throw new InvalidOperationException("Cannot unbind from this request. Always call CanUnbind before UnbindAsync to validate.");
        }

        string name;

        if (httpRequest.Form.ContainsKey(Constants.SamlRequest))
        {
            if (httpRequest.Form.ContainsKey(Constants.SamlResponse))
            {
                throw new ArgumentException("Either SamlResponse or SamlRequest should be defined, not both.");
            }
            name = Constants.SamlRequest;
        }
        else
        {
            if (httpRequest.Form.ContainsKey(Constants.SamlResponse))
            {
                name = Constants.SamlResponse;
            }
            else
            {
                // No need to handle case where none was present - CanUnbind should have taken care of that.
                throw new NotImplementedException();
            }
        }

        var xd = new XmlDocument();
        xd.LoadXml(Encoding.UTF8.GetString(Convert.FromBase64String(httpRequest.Form[name].Single()
            ?? throw new InvalidOperationException("No form content found"))));

        return Task.FromResult(new Saml2Message
        {
            Destination = httpRequest.PathBase + httpRequest.Path,
            Name = name,
            RelayState = httpRequest.Form[Constants.RelayState].SingleOrDefault(),
            Xml = xd.DocumentElement!,
            Binding = Identifier
        });
    }

    /// <inheritdoc/>
    protected override async Task DoBindAsync(
        HttpResponse httpResponse,
        Saml2Message message)

    {
        var xmlDoc = message.Xml;
        if (message.SigningCertificate != null)
        {
            var issuerElement = xmlDoc["Issuer", Constants.Namespaces.SamlUri]!;

            xmlDoc.Sign(message.SigningCertificate, issuerElement);
        }

        var relayStateHtml = string.IsNullOrEmpty(message.RelayState) ? null
            : string.Format(CultureInfo.InvariantCulture, PostHtmlRelayStateFormatString, message.RelayState);

        var encodedXml = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlDoc.OuterXml));

        var content = string.Format(
                    CultureInfo.InvariantCulture,
                    PostHtmlFormatString,
                    message.Destination,
                    relayStateHtml,
                    message.Name,
                    encodedXml);

        httpResponse.ContentType = "text/html";
        await httpResponse.WriteAsync(content);
    }

    private const string PostHtmlRelayStateFormatString = @"
<input type=""hidden"" name=""RelayState"" value=""{0}""/>";

    private const string PostHtmlFormatString = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN""
""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"">
<head>
<meta http-equiv=""Content-Security-Policy"" content=""script-src 'sha256-H3SVZBYrbqBt3ncrT/nNmOb6nwCjC12cPQzh5jnW4Y0='"">
</head>
<body>
<noscript>
<p>
<strong>Note:</strong> Since your browser does not support JavaScript, 
you must press the Continue button once to proceed.
</p>
</noscript>
<form action=""{0}"" method=""post"" name=""sustainsysSamlPostBindingSubmit"">
<div>{1}
<input type=""hidden"" name=""{2}""
value=""{3}""/>
</div>
<noscript>
<div>
<input type=""submit"" value=""Continue""/>
</div>
</noscript>
</form>
<script type=""text/javascript"">
document.forms.sustainsysSamlPostBindingSubmit.submit();
</script>
</body>
</html>";

}