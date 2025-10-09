// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication.Cookies;
using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore;
using System.Security.Cryptography.X509Certificates;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

var certificateData = Convert.FromBase64String("MIICFTCCAYKgAwIBAgIQzfcJCkM1YahDtRGYsLphrDAJBgUrDgMCHQUAMCExHzAdBgNVBAMTFnN0dWJpZHAuc3VzdGFpbnN5cy5jb20wHhcNMTcxMjE0MTE1NDUwWhcNMzkxMjMxMjM1OTU5WjAhMR8wHQYDVQQDExZzdHViaWRwLnN1c3RhaW5zeXMuY29tMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDSSq8EX46J1yprfaBdh4pWII+/E7ypHM1NjG7mCwFwbkjq2tpSBuoASrQftbjIKqjVzxtxETw802VJu5CJR4d3Zdy5jD8NRTesfaQDazX7iiqisfnxmIdDhtJS0lXeBlj4MipoUW6l8Qsjx7ltZSwdfFLyh+bMqIrwOhMWGs82vQIDAQABo1YwVDBSBgNVHQEESzBJgBCBBNba7KNF5wnXqmYcejn6oSMwITEfMB0GA1UEAxMWc3R1YmlkcC5zdXN0YWluc3lzLmNvbYIQzfcJCkM1YahDtRGYsLphrDAJBgUrDgMCHQUAA4GBAHonBGahlldp7kcN5HGGnvogT8a0nNpM7GMdKhtzpLO3Uk3HyT3AAIKWiSoEv2n1BTalJ/CY/+te/JZPVGhWVzoi5JYytpj5gM0O7RH0a3/yUE8S8YLV2h0a2gsdoMvTRTnTm9CnXezCKqhjYjwsmOZtiCIYuFqX71d/pg5uoJfs");

builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = Saml2Defaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddSaml2(opt =>
    {
        opt.IdentityProvider = new()
        {
            EntityId = "https://localhost:5000/Saml2",
            SsoServiceUrl = "https://localhost:5000/Saml2/Sso",
            //EntityId = "https://stubidp.sustainsys.com/Metadata",
            //SsoServiceUrl = "https://stubidp.sustainsys.com",
            SsoServiceBinding = Constants.BindingUris.HttpRedirect,
            SigningKeys = [
            new()
            {
#if NET9_0_OR_GREATER
                Certificate = X509CertificateLoader.LoadCertificate(certificateData),
#else
                Certificate =  new X509Certificate2(certificateData),
#endif
                TrustLevel = TrustLevel.TLS
            },
            new()
            {
#if NET9_0_OR_GREATER
                Certificate = X509CertificateLoader.LoadPkcs12FromFile("Sustainsys.Saml2.Tests.pfx", "", X509KeyStorageFlags.EphemeralKeySet),
#else
                Certificate = new("Sustainsys.Saml2.Tests.pfx"),
#endif
                TrustLevel = TrustLevel.ConfiguredKey
            }
            ]
        };
        opt.EntityId = "https://localhost:5001/Saml2";
    });

builder.Services.AddRazorPages();

var app = builder.Build();

// Enable running the pipeline with path base. This somewhat confusingly
// allows the rest of the app to answer both on / and /subdir.
app.UsePathBase("/subdir");

app.UseAuthentication();

app.UseRouting();

app.MapRazorPages();

app.Run();