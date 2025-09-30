// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

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
    public static X509Certificate2 Certificate { get; } = LoadCertificate("Sustainsys.Saml2.Tests.pfx");

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
        Certificate = LoadCertificate("Sustainsys.Saml2.Tests2.pfx"),
        TrustLevel = TrustLevel.TLS
    };

    private static X509Certificate2 LoadCertificate(string path)
    {
#if NET9_0_OR_GREATER
        return X509CertificateLoader.LoadPkcs12FromFile(path, "", X509KeyStorageFlags.EphemeralKeySet);
#else
        return new X509Certificate2(path);
#endif
    }

    public static SigningKey[] SingleSigningKey2 { get; } = new[]
    {
        SigningKey2
    };
}