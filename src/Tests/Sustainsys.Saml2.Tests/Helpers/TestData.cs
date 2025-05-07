using Sustainsys.Saml2.Xml;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace Sustainsys.Saml2.Tests.Helpers;

public static class TestData
{
    public static XmlTraverser GetXmlTraverser<TDirectory>([CallerMemberName] string? testName = null)
    {
        var document = GetXmlDocument<TDirectory>(testName);

        return new XmlTraverser(document.DocumentElement ?? throw new InvalidOperationException("XmlDoc contained no DocumentElement"));
    }

    public static XmlDocument GetXmlDocument<TDirectory>([CallerMemberName] string? testName = null)
    {
        ArgumentNullException.ThrowIfNull(testName);

        var assemblyName = typeof(TDirectory).Assembly.GetName().Name!;

        var fileName = "../../.."
            + typeof(TDirectory).FullName![assemblyName.Length..].Replace('.', '/')
            + "/" + testName + ".xml";

        var document = new XmlDocument();
        document.Load(fileName);
        return document;
    }

    public static X509Certificate2 Certificate { get; } = new X509Certificate2("Sustainsys.Saml2.Tests.pfx");

    public static SigningKey SigningKey { get; } = new()
    {
        Certificate = Certificate,
        TrustLevel = TrustLevel.ConfiguredKey
    };

    public static SigningKey[] SingleSigningKey { get; } =
    {
        SigningKey
    };

    public static SigningKey SigningKey2 { get; } = new()
    {
        Certificate = new X509Certificate2("Sustainsys.Saml2.Tests2.pfx"),
        TrustLevel = TrustLevel.TLS
    };

    public static SigningKey[] SingleSigningKey2 { get; } = new[]
    {
        SigningKey2
    };
}