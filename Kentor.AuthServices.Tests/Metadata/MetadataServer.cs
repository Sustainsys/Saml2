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

            content["/idpMetadataOtherEntityId"] = string.Format(
@"<EntityDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
    entityID=""http://other.entityid.example.com"">
    <IDPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <KeyDescriptor use=""signing"">
        {0}
      </KeyDescriptor>
      <SingleSignOnService
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
        Location=""http://wrong.entityid.example.com/acs""/>
    </IDPSSODescriptor>
  </EntityDescriptor>", SignedXmlHelper.KeyInfoXml);

            content["/federationMetadata"] = string.Format(
@"<EntitiesDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata"" validUntil=""2100-01-01T14:43:15Z"">
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
                string keyElement = IdpVeryShortCacheDurationIncludeKey ?
                    string.Format(@"<KeyDescriptor use=""signing"">{0}</KeyDescriptor>",
                    IdpVeryShortCacheDurationIncludeInvalidKey ? "Gibberish" : SignedXmlHelper.KeyInfoXml2)
                    : "";

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
        public static bool IdpVeryShortCacheDurationIncludeKey { get; set; }
        public static bool IdpAndFederationShortCacheDurationAvailable { get; set; }
        public static bool FederationVeryShortCacheDurationSecondAlternativeEnabled { get; set; }

        static MetadataServer()
        {
            IdpMetadataSsoPort = 13428;
            IdpAndFederationVeryShortCacheDurationSsoPort = 80;
            IdpVeryShortCacheDurationBinding = Saml2Binding.HttpRedirectUri;
            IdpVeryShortCacheDurationIncludeKey = true;
            IdpAndFederationShortCacheDurationAvailable = true;
            FederationVeryShortCacheDurationSecondAlternativeEnabled = false;
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
