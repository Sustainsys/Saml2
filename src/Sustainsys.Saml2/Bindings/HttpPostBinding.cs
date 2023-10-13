using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public override bool CanUnbind(HttpRequest httpRequest)
        => httpRequest.Method == "POST"
        && httpRequest.Form.Keys.Any(
            k => k == Constants.SamlRequest || k == Constants.SamlResponse);

    /// <inheritdoc/>
    public override Task<TrustedData<Saml2Message>> UnbindAsync(
        HttpRequest httpRequest,
        Func<string, Task<Saml2Entity>> getSaml2Entity)
    {
        if (!CanUnbind(httpRequest))
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

        return Task.FromResult(new TrustedData<Saml2Message>(
            TrustLevel.None,
            new Saml2Message
            {
                Destination = httpRequest.PathBase + httpRequest.Path,
                Name = name,
                RelayState = httpRequest.Form[Constants.RelayState].SingleOrDefault(),
                Xml = xd.DocumentElement!
            }));
    }

    /// <inheritdoc/>
    protected override Task DoBindAsync(
        HttpResponse httpResponse,
        Saml2Message message)
        => throw new NotImplementedException();
}
