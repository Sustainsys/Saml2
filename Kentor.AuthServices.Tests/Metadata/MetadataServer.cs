using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;
using Kentor.AuthServices.TestHelpers;
using Kentor.AuthServices.WebSso;
using System.Threading;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Kentor.AuthServices.Tests.Metadata
{
    [TestClass]
    public class MetadataServer
    {
        private static IDisposable host;

        static IDictionary<string, string> GetContent()
        {
            var content = new Dictionary<string, string>();

            content["/idpMetadata"] = string.Format(
 @"<EntityDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
    entityID=""http://localhost:13428/idpMetadata"" validUntil=""2100-01-02T14:42:43Z"">
    <IDPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <KeyDescriptor>
        {0}
      </KeyDescriptor>
      <SingleSignOnService
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST""
        Location=""http://localhost:{1}/acs""/>
    </IDPSSODescriptor>
  </EntityDescriptor>
", SignedXmlHelper.KeyInfoXml, IdpMetadataSsoPort);

            content["/idpMetadataNoCertificate"] = 
@"<EntityDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
    entityID=""http://localhost:13428/idpMetadataNoCertificate"" cacheDuration=""PT15M"">
    <IDPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <SingleSignOnService
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
        Location=""http://localhost:13428/acs""/>
    </IDPSSODescriptor>
  </EntityDescriptor>";

            content["/idpMetadataOtherEntityId"] =
@"<EntityDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
    entityID=""http://other.entityid.example.com"">
    <IDPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <SingleSignOnService
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
        Location=""http://wrong.entityid.example.com/acs""/>
    </IDPSSODescriptor>
  </EntityDescriptor>";

            content["/federationMetadata"] = string.Format(
@"<EntitiesDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata"" Name=""Kentor.AuthServices.StubIdp Federation"" validUntil=""2100-01-01T14:43:15Z"" ID=""federationId"">
  <EntityDescriptor entityID=""http://idp.federation.example.com/metadata"">
    <IDPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <KeyDescriptor use=""signing"">
        {0}
      </KeyDescriptor>
      <SingleSignOnService
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
        Location=""http://idp.federation.example.com/ssoService"" />
    </IDPSSODescriptor>
  </EntityDescriptor>
  <EntityDescriptor entityID=""http://sp.federation.example.com/metadata"">
    <SPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <AssertionConsumerService index=""0""
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST""
        Location=""http://sp.federation.example.com/acs"" />
    </SPSSODescriptor>
  </EntityDescriptor>
</EntitiesDescriptor>
", SignedXmlHelper.KeyInfoXml);

            content["/federationMetadata"] = string.Format(
@"<EntitiesDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata"" Name=""Kentor.AuthServices.StubIdp Federation"" validUntil=""2100-01-01T14:43:15Z"" ID=""federationId"">
  <EntityDescriptor entityID=""http://idp.federation.example.com/metadata"">
    <IDPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <KeyDescriptor use=""signing"">
        {0}
      </KeyDescriptor>
      <SingleSignOnService
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
        Location=""http://idp.federation.example.com/ssoService"" />
    </IDPSSODescriptor>
  </EntityDescriptor>
  <EntityDescriptor entityID=""http://sp.federation.example.com/metadata"">
    <SPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <AssertionConsumerService index=""0""
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST""
        Location=""http://sp.federation.example.com/acs"" />
    </SPSSODescriptor>
  </EntityDescriptor>
</EntitiesDescriptor>
", SignedXmlHelper.KeyInfoXml, SignedXmlHelper.KeyInfoXml);

            content["/federationMetadataSigned"] = @"<?xml version=""1.0"" encoding=""utf-8""?><EntitiesDescriptor ID=""_88093024-575f-47f1-ab57-a66c74f6bef6"" Name=""Kentor.AuthServices.StubIdp Federation"" cacheDuration=""PT15M"" xmlns=""urn:oasis:names:tc:SAML:2.0:metadata"">
  <Signature xmlns=""http://www.w3.org/2000/09/xmldsig#"">
    <SignedInfo>
      <CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>
      <SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""/>
      <Reference URI=""#_88093024-575f-47f1-ab57-a66c74f6bef6"">
        <Transforms>
          <Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature""/>
          <Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>
        </Transforms>
        <DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""/>
        <DigestValue>rvfKbX7Nz2svHvo62uyOSn8EGhQ=</DigestValue>
      </Reference>
    </SignedInfo>
    <SignatureValue>c336dSeIEmf1OdFF6IkEr3ZWpFY28p0ZMWYCkR63r3BVyVNnkLnjv2EfiUuyaxtcc4H+5/4nfIi10z1TBYK2J1UjFCfzkpA3hUgGZ0I20+BHbFm2hoKTCU0M4ttc6kWJoyeenDVA7KnKQqWdIGKW1G4aMfB9vKCghey570QqD2JZynHDs69pYXC2lWRjWfgjsHngGJWsMCzBwKrCLOO6lG7XDNtx9LfLpZprv32ktsEJ/ggsAVoL/apB1gNXHPnY0/+RAIMRyXX5waiwnYGhxuRhZPKLtvLQkJRAF1hNeCo/g6wtoyCHDzfHEN3cWmLhI57wRMbqcBjteVmWFcVrAw==</SignatureValue>
    <KeyInfo>
      <X509Data>
        <X509Certificate>MIIDKTCCAhWgAwIBAgIQoXDqga0edKNDrLX+FDyO1TAJBgUrDgMCHQUAMCYxJDAiBgNVBAMTG0tlbnRvci5BdXRoU2VydmljZXMuU3R1YklkcDAeFw0xMzEyMjcyMDU0NDVaFw0zOTEyMzEyMzU5NTlaMCYxJDAiBgNVBAMTG0tlbnRvci5BdXRoU2VydmljZXMuU3R1YklkcDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBANAIi7po3bIVWeoEMIV60qt+MMXeCk4p58+ZqicnPUDyUuWxpft/fp0g4qARxfvJVTHCEJDGykaGuX4z3zEhB9tSz8MD7xbqHFrIIp0UJDKZFAl+zQH+eXnvG7h6P0BJR74fvGE6Y124PRMl/AE9SXwr2T2kr1wS/jO4pBRKo2H5tlhgTfsWSfIkGhvYXu5a1vP7iBqswlAzNYLUQI17okQEsli3mVzwfvDHwzOZtjnKIQA/Bs/UA21ZJZm1eGd3VEXw3vWv34hZXTWe7Hc8eBO8Yip81An+OFVEJ8kshUOtmmVtmZCAMfTq0TOwdfoDnsDHiVFp3nAS7gdUZ9rOxnECAwEAAaNbMFkwVwYDVR0BBFAwToAQOp8UfulPoU3Zor4hyctWkaEoMCYxJDAiBgNVBAMTG0tlbnRvci5BdXRoU2VydmljZXMuU3R1YklkcIIQoXDqga0edKNDrLX+FDyO1TAJBgUrDgMCHQUAA4IBAQBJN/vhEGjqQn1/lPEqezEiScCoRh2ZRBqDHJERAFLzH1DMrfp602NLYOUmbmIWoWjLoen+Pl7MEIF/lyC0WteMOEOk/pqvFMBrwbRwy1er8LbzMBbPVZaLpN858NOVdpGlilErHPkC9WtS3LIFuBz5/jnInC0JkTuf/LJP2g2OeRlJbLFJyxxK4ahTlbabzENe/jgplipDwBosbnLpMmL1B1/vj+RNHOxxaqhcsmdxhY/Zr34FyXguOLoKx9u/v8XDVB7gf/8ZH6tpMyESy2zeLjbXi9LRt7GLb5b+Fo4qDXBaTLWvuV9ltftrLkpL1rYToTGRfl+SQke7+kFM7l+N</X509Certificate>
      </X509Data>
    </KeyInfo>
  </Signature>
  <EntityDescriptor ID=""_d80d1a2e-003d-42ec-b642-12a36fbe5981"" entityID=""http://localhost:52071/Metadata"" xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion"">
    <IDPSSODescriptor protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <KeyDescriptor>
        <KeyInfo xmlns=""http://www.w3.org/2000/09/xmldsig#"">
          <X509Data>
            <X509Certificate>MIIDKTCCAhWgAwIBAgIQoXDqga0edKNDrLX+FDyO1TAJBgUrDgMCHQUAMCYxJDAiBgNVBAMTG0tlbnRvci5BdXRoU2VydmljZXMuU3R1YklkcDAeFw0xMzEyMjcyMDU0NDVaFw0zOTEyMzEyMzU5NTlaMCYxJDAiBgNVBAMTG0tlbnRvci5BdXRoU2VydmljZXMuU3R1YklkcDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBANAIi7po3bIVWeoEMIV60qt+MMXeCk4p58+ZqicnPUDyUuWxpft/fp0g4qARxfvJVTHCEJDGykaGuX4z3zEhB9tSz8MD7xbqHFrIIp0UJDKZFAl+zQH+eXnvG7h6P0BJR74fvGE6Y124PRMl/AE9SXwr2T2kr1wS/jO4pBRKo2H5tlhgTfsWSfIkGhvYXu5a1vP7iBqswlAzNYLUQI17okQEsli3mVzwfvDHwzOZtjnKIQA/Bs/UA21ZJZm1eGd3VEXw3vWv34hZXTWe7Hc8eBO8Yip81An+OFVEJ8kshUOtmmVtmZCAMfTq0TOwdfoDnsDHiVFp3nAS7gdUZ9rOxnECAwEAAaNbMFkwVwYDVR0BBFAwToAQOp8UfulPoU3Zor4hyctWkaEoMCYxJDAiBgNVBAMTG0tlbnRvci5BdXRoU2VydmljZXMuU3R1YklkcIIQoXDqga0edKNDrLX+FDyO1TAJBgUrDgMCHQUAA4IBAQBJN/vhEGjqQn1/lPEqezEiScCoRh2ZRBqDHJERAFLzH1DMrfp602NLYOUmbmIWoWjLoen+Pl7MEIF/lyC0WteMOEOk/pqvFMBrwbRwy1er8LbzMBbPVZaLpN858NOVdpGlilErHPkC9WtS3LIFuBz5/jnInC0JkTuf/LJP2g2OeRlJbLFJyxxK4ahTlbabzENe/jgplipDwBosbnLpMmL1B1/vj+RNHOxxaqhcsmdxhY/Zr34FyXguOLoKx9u/v8XDVB7gf/8ZH6tpMyESy2zeLjbXi9LRt7GLb5b+Fo4qDXBaTLWvuV9ltftrLkpL1rYToTGRfl+SQke7+kFM7l+N</X509Certificate>
          </X509Data>
        </KeyInfo>
      </KeyDescriptor>
      <SingleSignOnService Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect"" Location=""http://localhost:52071/""/>
    </IDPSSODescriptor>
  </EntityDescriptor>
</EntitiesDescriptor>";

            if (SignFederationMetadata)
            {
                content["/federationMetadata"] = SignedXmlHelper.SignXmlMetadata(content["/federationMetadata"]);
            }

            if (IdpAndFederationShortCacheDurationAvailable)
            {
                content["/federationMetadataVeryShortCacheDuration"] = string.Format(
@"<EntitiesDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata"" cacheDuration=""PT0.001S"">
  <EntityDescriptor entityID=""http://idp1.federation.example.com/metadata"">
    <IDPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <KeyDescriptor use=""signing"">
        {0}
      </KeyDescriptor>
      <SingleSignOnService
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
        Location=""http://idp1.federation.example.com:{1}/ssoService"" />
    </IDPSSODescriptor>
  </EntityDescriptor>
  <EntityDescriptor entityID=""http://idp2.federation.example.com/metadata"">
    <IDPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <KeyDescriptor use=""signing"">
        {0}
      </KeyDescriptor>
      <SingleSignOnService
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
        Location=""http://idp2.federation.example.com:{1}/ssoService"" />
    </IDPSSODescriptor>
  </EntityDescriptor>
  <EntityDescriptor entityID=""http://sp.federation.example.com/metadata"">
    <SPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <AssertionConsumerService index=""0""
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST""
        Location=""http://sp.federation.example.com/acs"" />
    </SPSSODescriptor>
  </EntityDescriptor>
</EntitiesDescriptor>", 
                      SignedXmlHelper.KeyInfoXml, 
                      IdpAndFederationVeryShortCacheDurationSsoPort);
            }

            if(FederationVeryShortCacheDurationSecondAlternativeEnabled)
            {
                content["/federationMetadataVeryShortCacheDuration"] = string.Format(
@"<EntitiesDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata"" cacheDuration=""PT0.001S"">
  <EntityDescriptor entityID=""http://idp1.federation.example.com/metadata"">
    <IDPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <KeyDescriptor use=""signing"">
        {0}
      </KeyDescriptor>
      <SingleSignOnService
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
        Location=""http://idp1.federation.example.com:{1}/ssoService"" />
    </IDPSSODescriptor>
  </EntityDescriptor>
  <EntityDescriptor entityID=""http://idp3.federation.example.com/metadata"">
    <IDPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <KeyDescriptor use=""signing"">
        {0}
      </KeyDescriptor>
      <SingleSignOnService
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
        Location=""http://idp3.federation.example.com:{1}/ssoService"" />
    </IDPSSODescriptor>
  </EntityDescriptor>
  <EntityDescriptor entityID=""http://sp.federation.example.com/metadata"">
    <SPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <AssertionConsumerService index=""0""
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST""
        Location=""http://sp.federation.example.com/acs"" />
    </SPSSODescriptor>
  </EntityDescriptor>
</EntitiesDescriptor>",
                      SignedXmlHelper.KeyInfoXml,
                      IdpAndFederationVeryShortCacheDurationSsoPort);
            }

            if (IdpAndFederationShortCacheDurationAvailable)
            {
                content["/federationMetadataShortCacheDuration"] = string.Format(
@"<EntitiesDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata"" cacheDuration=""PT0.200S"">
  <EntityDescriptor entityID=""http://idp1.federation.example.com/metadata"">
    <IDPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <KeyDescriptor use=""signing"">
        {0}
      </KeyDescriptor>
      <SingleSignOnService
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
        Location=""http://idp1.federation.example.com/ssoService"" />
    </IDPSSODescriptor>
  </EntityDescriptor>
  <EntityDescriptor entityID=""http://idp2.federation.example.com/metadata"">
    <IDPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <KeyDescriptor use=""signing"">
        {0}
      </KeyDescriptor>
      <SingleSignOnService
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
        Location=""http://idp2.federation.example.com/ssoService"" />
    </IDPSSODescriptor>
  </EntityDescriptor>
  <EntityDescriptor entityID=""http://sp.federation.example.com/metadata"">
    <SPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <AssertionConsumerService index=""0""
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST""
        Location=""http://sp.federation.example.com/acs"" />
    </SPSSODescriptor>
  </EntityDescriptor>
</EntitiesDescriptor>",
                      SignedXmlHelper.KeyInfoXml);
            }

            content["/idpMetadataWithMultipleBindings"] = string.Format(
@"<EntityDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
  entityID=""http://localhost:13428/idpMetadataWithMultipleBindings"">
  <IDPSSODescriptor
    protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
    <KeyDescriptor use=""signing"">
      {0}
    </KeyDescriptor>
    <SingleSignOnService
      Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST""
      Location=""http://idp2Bindings.example.com/POST"" />
    <SingleSignOnService
      Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
      Location=""http://idp2Bindings.example.com/Redirect"" />
  </IDPSSODescriptor>
</EntityDescriptor>", SignedXmlHelper.KeyInfoXml);

            content["/idpMetadataDifferentEntityId"] = string.Format(
@"<EntityDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
  entityID=""some-idp"">
  <IDPSSODescriptor
    protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
    <KeyDescriptor use=""signing"">
      {0}
    </KeyDescriptor>
    <SingleSignOnService
      Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
      Location=""http://idp.example.com/SsoService"" />
  </IDPSSODescriptor>
</EntityDescriptor>", SignedXmlHelper.KeyInfoXml);

            content["/idpMetadataWithArtifactBinding"] = string.Format(
@"<EntityDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
  entityID=""http://localhost:13428/idpMetadataWithArtifactBinding"">
  <IDPSSODescriptor
    protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
    <KeyDescriptor use=""signing"">
      {0}
    </KeyDescriptor>
    <SingleSignOnService
      Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Artifact""
      Location=""http://idpArtifact.example.com/Artifact"" />
    <SingleSignOnService
      Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST""
      Location=""http://idpArtifact.example.com/POST"" />
  </IDPSSODescriptor>
</EntityDescriptor>", SignedXmlHelper.KeyInfoXml);

            if (IdpAndFederationShortCacheDurationAvailable)
            {
                string keyElement = string.Empty;
                if (IdpVeryShortCacheDurationIncludeInvalidKey)
                {
                    keyElement = @"<KeyDescriptor use=""signing"">Gibberish</KeyDescriptor>";
                }

                content["/idpMetadataVeryShortCacheDuration"] = string.Format(
@"<EntityDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
entityID=""http://localhost:13428/idpMetadataVeryShortCacheDuration"" cacheDuration=""PT0.001S"">
<IDPSSODescriptor
    protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
    {0}
    <SingleSignOnService
    Binding=""{1}""
    Location=""http://localhost:{2}/acs""/>
</IDPSSODescriptor>
</EntityDescriptor>",
                    keyElement, IdpVeryShortCacheDurationBinding, IdpAndFederationVeryShortCacheDurationSsoPort);
            }

            var sambipath = "Metadata\\SambiMetadata.xml";
            var skolfederationPath = "Metadata\\SkolfederationMetadata.xml";

            if (File.Exists(sambipath))
            {
                content["/SambiMetadata"] = File.ReadAllText(sambipath);
            }
            if (File.Exists(skolfederationPath))
            {
                content["/SkolfederationMetadata"] = File.ReadAllText(skolfederationPath);
            }

            return content;
        }

        public static int IdpMetadataSsoPort { get; set; }
        public static int IdpAndFederationVeryShortCacheDurationSsoPort { get; set; }
        public static Uri IdpVeryShortCacheDurationBinding { get; set; }
        public static bool IdpVeryShortCacheDurationIncludeInvalidKey { get; set; }
        public static bool IdpAndFederationShortCacheDurationAvailable { get; set; }
        public static bool FederationVeryShortCacheDurationSecondAlternativeEnabled { get; set; }
        public static bool SignFederationMetadata { get; set; }

        static MetadataServer()
        {
            IdpMetadataSsoPort = 13428;
            IdpAndFederationVeryShortCacheDurationSsoPort = 80;
            IdpVeryShortCacheDurationBinding = Saml2Binding.HttpRedirectUri;
            IdpAndFederationShortCacheDurationAvailable = true;
            FederationVeryShortCacheDurationSecondAlternativeEnabled = false;
            SignFederationMetadata = false;
        }

        [AssemblyInitialize]
        public static void Start(TestContext testContext)
        {
            host = WebApp.Start("http://localhost:13428", app =>
            {
                app.Use(async (ctx, next) =>
                {
                    var content = GetContent();
                    string data;
                    if(content.TryGetValue(ctx.Request.Path.ToString(), out data))
                    {
                        await ctx.Response.WriteAsync(data);
                    }
                    else
                    {
                        await next.Invoke();
                    }
                });
            });
        }

        [AssemblyCleanup]
        public static void Stop()
        {
            host.Dispose();
        }
    }
}
