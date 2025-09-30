using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.IO.Compression;
using System.Xml;

namespace Sustainsys.Saml2.Bindings;

/// <summary>
/// Redirect binding implementation
/// </summary>
public interface IHttpRedirectBinding : IFrontChannelBinding
{
    /// <summary>
    /// Unbind from a URL.
    /// </summary>
    /// <param name="url">Url to unbind from</param>
    /// <param name="getSaml2Entity">Func that returns Identity provider from an entity id</param>
    /// <returns>Unbound message</returns>
    Task<Saml2Message> UnBindAsync(
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

    /// <inheritdoc/>
    public virtual Task<Saml2Message> UnBindAsync(
        string url,
        Func<string, Task<Saml2Entity>> getSaml2Entity)
    {
        var uri = new Uri(url);
        var query = new QueryStringEnumerable(uri.Query);

        string? messageName = null;
        string? message = null;
        string? relayState = null;

        foreach (var param in query)
        {
            // Simplicity is more important than performance here.
            var encodedName = param.EncodedName.ToString();

            // The standard is specific about the naming and they should never be
            // encoded. So let's find encoded only.
            if (encodedName == Constants.SamlResponse
                || encodedName == Constants.SamlRequest)
            {
                if (messageName != null)
                {
                    throw new InvalidOperationException($"Duplicate message parameters found: {messageName}, {encodedName}");
                }

                messageName = encodedName;
                message = param.DecodeValue().ToString();
            }

            if (encodedName == Constants.RelayState)
            {
                if (relayState != null)
                {
                    throw new InvalidOperationException("Duplicate RelayState parameters found");
                }
                relayState = param.DecodeValue().ToString();
            }
        }

        if (messageName == null || message == null)
        {
            throw new InvalidOperationException("SAMLResponse or SAMLRequest parameter not found");
        }

        var xd = new XmlDocument();
        xd.LoadXml(Inflate(message));

        return Task.FromResult(new Saml2Message()
        {
            // We're not supporting destinations containing a query string.
            Destination = uri.Scheme + "://" + uri.Host + uri.AbsolutePath,
            Name = messageName,
            RelayState = relayState,
            Xml = xd.DocumentElement!
        });
    }

    /// <inheritdoc/>    
    public override Task<Saml2Message> UnbindAsync(
        HttpRequest httpRequest,
        Func<string, Task<Saml2Entity>> getSaml2Entity) => throw new NotImplementedException();

    /// <inheritdoc/>
    protected override Task DoBindAsync(HttpResponse httpResponse, Saml2Message message)
    {
        var xmlString = message.Xml.OuterXml;

        var encoded = Deflate(xmlString);

        var location = $"{message.Destination}?{message.Name}={encoded}&RelayState={message.RelayState}";

        httpResponse.Redirect(location);

        return Task.CompletedTask;
    }

    private static string Deflate(string source)
    {
        using var compressed = new MemoryStream();
        using (var deflateStream = new DeflateStream(compressed, CompressionLevel.Optimal))
        {
            using var writer = new StreamWriter(deflateStream);
            writer.Write(source);
        }

        return Uri.EscapeDataString(Convert.ToBase64String(compressed.ToArray()));
    }

    private static string Inflate(string source)
    {
        var compressedBytes = Convert.FromBase64String(Uri.UnescapeDataString(source));

        using var compressed = new MemoryStream(compressedBytes);
        using var deflateStream = new DeflateStream(compressed, CompressionMode.Decompress);
        using var reader = new StreamReader(deflateStream);

        return reader.ReadToEnd();
    }
}