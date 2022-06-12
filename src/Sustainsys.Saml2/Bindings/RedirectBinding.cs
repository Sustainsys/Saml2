using System.IO.Compression;

namespace Sustainsys.Saml2.Bindings;

/// <summary>
/// Saml2 Http Redirect Binding
/// </summary>
public class RedirectBinding : Binding
{
    /// <inheritdoc/>
    public override string Identification => throw new NotImplementedException();

    /// <inheritdoc/>
    protected override BoundMessage DoBind(Saml2Message message)
    {
        var xmlString = message.Xml.OuterXml;

        using var compressed = new MemoryStream();
        using (var deflateStream = new DeflateStream(compressed, CompressionLevel.Optimal))
        {
            using var writer = new StreamWriter(deflateStream);
            writer.Write(xmlString);
        }

        var encoded = Uri.EscapeDataString(Convert.ToBase64String(compressed.ToArray()));

        return new()
        {
            Method = HttpMethod.Get,
            Items = { new(message.Name, encoded) }
        };
    }
}
