using Microsoft.AspNetCore.Http;
using Sustainsys.Saml2.Metadata.Attributes;
using System.IO.Compression;

namespace Sustainsys.Saml2.Bindings;

/// <summary>
/// Redirect binding implementation
/// </summary>
public interface IHttpRedirectBinding
{
    /// <summary>
    /// Unbind from a URL.
    /// </summary>
    /// <param name="url">Url to unbind from</param>
    /// <param name="getSaml2Entity">Func that returns Identity provider from an entity id</param>
    /// <returns>Unbound message</returns>
    Saml2Message UnBindAsync(
        string url, 
        Func<string, Task<Saml2Entity>> getSaml2Entity);
}

/// <summary>
/// Saml2 Http Redirect Binding
/// </summary>
public class HttpRedirectBinding : FrontChannelBinding, IHttpRedirectBinding
{
    /// <summary>
    /// Constructor
    /// </summary>
    public HttpRedirectBinding() : base(Constants.BindingUris.HttpRedirect) { }

    /// <inheritdoc/>
    public override bool CanUnbind(HttpRequest httpRequest)
        => false; // Because we haven't implemented redirect unbind yet.

    /// <summary>
    /// Unbinds a Saml2 mesasge from a URL
    /// </summary>
    /// <param name="url">URL with Saml message</param>
    /// <param name="getSaml2Entity">Func that returns Identity provider from an entity id</param>
    /// <returns>Unbount message</returns>
    public Saml2Message UnBindAsync(
        string url,
        Func<string, Task<Saml2Entity>> getSaml2Entity)
        => throw new NotImplementedException();
    
    /// <inheritdoc/>    
    public override Task<Saml2Message> UnbindAsync(
        HttpRequest httpRequest,
        Func<string, Task<Saml2Entity>> getSaml2Entity) => throw new NotImplementedException();

    /// <inheritdoc/>
    protected override async Task DoBindAsync(HttpResponse httpResponse, Saml2Message message)
    {
        var xmlString = message.Xml.OuterXml;

        using var compressed = new MemoryStream();
        using (var deflateStream = new DeflateStream(compressed, CompressionLevel.Optimal))
        {
            using var writer = new StreamWriter(deflateStream);
            await writer.WriteAsync(xmlString);
        }

        var encoded = Uri.EscapeDataString(Convert.ToBase64String(compressed.ToArray()));

        var location = $"{message.Destination}?{message.Name}={encoded}&RelayState={message.RelayState}";

        httpResponse.Redirect(location);
    }
}
