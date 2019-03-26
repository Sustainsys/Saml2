#if NETCOREAPP2_1
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
#else
using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using HttpContext = Microsoft.Owin.IOwinContext;
#endif
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Sustainsys.Saml2.WebSso;
using System.IO;
using System.Xml.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using Sustainsys.Saml2.TestHelpers;

namespace Sustainsys.Saml2.Tests.Helpers
{
    public class StubServer
    {
#if NETCOREAPP2_1
		private static IWebHost host;
#else
        private static IDisposable host;
#endif
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
      <ArtifactResolutionService index=""4660""
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:SOAP""
        Location=""http://localhost:{1}/ars""/>
      <ArtifactResolutionService index=""117""
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:SOAP""
        Location=""http://localhost:{1}/ars2""/>
      <SingleLogoutService
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
        Location=""http://localhost:{1}/logout""
        ResponseLocation=""http://localhost:{1}/logoutResponse""/>
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
      <SingleLogoutService
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
        Location=""http://localhost:13428/logout""/>
    </IDPSSODescriptor>
  </EntityDescriptor>";

            content["/idpMetadataOtherEntityId"] = string.Format(
@"<EntityDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
    entityID=""http://other.entityid.example.com"">
    <IDPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol""
      WantAuthnRequestsSigned=""true"">
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

            var federationMetadataSigned = string.Format(
@"<EntitiesDescriptor ID=""federationMetadataSigned"" xmlns=""urn:oasis:names:tc:SAML:2.0:metadata"" validUntil=""2100-01-01T14:43:15Z"">
  <EntityDescriptor entityID=""http://idp.signedfederation.example.com/metadata"">
    <IDPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <KeyDescriptor use=""signing"">
        {0}
      </KeyDescriptor>
      <SingleSignOnService
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
        Location=""http://idp.signedfederation.example.com/ssoService"" />
    </IDPSSODescriptor>
  </EntityDescriptor>
</EntitiesDescriptor>
", SignedXmlHelper.KeyInfoXml);

            content["/federationMetadataSigned"] =
                SignedXmlHelper.SignXml(federationMetadataSigned);

            var federationMetadataSignedTampered = string.Format(
@"<EntitiesDescriptor ID=""federationMetadataSignedTampered"" xmlns=""urn:oasis:names:tc:SAML:2.0:metadata"" validUntil=""2100-01-01T14:43:15Z"">
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
</EntitiesDescriptor>
", SignedXmlHelper.KeyInfoXml);

            federationMetadataSignedTampered = SignedXmlHelper.SignXml(federationMetadataSignedTampered);

            content["/federationMetadataSignedTampered"] = 
                federationMetadataSignedTampered.Replace("ssoService", "tampered");

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
                      IdpAndFederationVeryShortCacheDurationPort);
            }

            if (FederationVeryShortCacheDurationSecondAlternativeEnabled)
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
                      IdpAndFederationVeryShortCacheDurationPort);
            }

            if (IdpAndFederationShortCacheDurationAvailable)
            {
                content["/federationMetadataShortCacheDuration"] = string.Format(
@"<EntitiesDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata"" cacheDuration=""PT0.500S"">
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
    <SingleLogoutService
      Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST""
      Location=""http://idp2Bindings.example.com/LogoutPost""
      ResponseLocation=""http://idp2Bindings.example.com/LogoutPostResponse""/>
    <SingleLogoutService
      Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
      Location=""http://idp2Bindings.example.com/LogoutRedirect""
      ResponseLocation=""http://idp2Bindings.example.com/LogoutRedirectResponse""/>
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
    <ArtifactResolutionService
      index=""0""
      Location=""http://localhost:{2}/ars""
      Binding=""urn:oasis:names:tc:SAML:2.0:bindings:SOAP"" />
      <SingleLogoutService
        Binding=""{1}""
        Location=""http://localhost:{2}/logout""
        ResponseLocation=""http://localhost:{2}/logoutResponse""/>
</IDPSSODescriptor>
</EntityDescriptor>",
                    keyElement, IdpVeryShortCacheDurationBinding, IdpAndFederationVeryShortCacheDurationPort);
            }

            return content;
        }

        public static int IdpMetadataSsoPort { get; set; } = 13428;
        public static int IdpAndFederationVeryShortCacheDurationPort { get; set; } = 80;
        public static Uri IdpVeryShortCacheDurationBinding { get; set; } = Saml2Binding.HttpRedirectUri;
        public static bool IdpVeryShortCacheDurationIncludeInvalidKey { get; set; }
        public static bool IdpVeryShortCacheDurationIncludeKey { get; set; } = true;
        public static bool IdpAndFederationShortCacheDurationAvailable { get; set; } = true;
        public static bool FederationVeryShortCacheDurationSecondAlternativeEnabled { get; set; } = false;

		static async Task HandleRequestAsync(HttpContext ctx, Func<Task> next)
		{
			string data;

			switch (ctx.Request.Path.ToString())
			{
				case "/ars":
					await ArtifactResolutionService(ctx);
					return;
				default:
					var content = GetContent();
					if (content.TryGetValue(ctx.Request.Path.ToString(), out data))
					{
						await ctx.Response.WriteAsync(data);
						return;
					}
					break;
			}
			await next.Invoke();
		}

#if NETCOREAPP2_1
		public static void Start(TestContext testContext)
        {
			host = new WebHostBuilder()
				.UseUrls("http://localhost:13428")
				.Configure(builder => builder.Use(HandleRequestAsync))
				.UseKestrel()
				.Build();
			host.Start();
        }
#else
		public static void Start(TestContext testContext)
		{
			host = WebApp.Start("http://localhost:13428", app =>
			{
				app.Use(HandleRequestAsync);
			});
		}
#endif

		private static async Task ArtifactResolutionService(HttpContext ctx)
        {
            LastArtifactResolutionSoapActionHeader = ctx.Request.Headers["SOAPAction"];

            using (var reader = new StreamReader(ctx.Request.Body))
            {
                var body = reader.ReadToEnd();

                var parsedRequest = XElement.Parse(body);

                var requestId = parsedRequest
                    .Element(Saml2Namespaces.SoapEnvelope + "Body")
                    .Element(Saml2Namespaces.Saml2P + "ArtifactResolve")
                    .Attribute("ID").Value;

                LastArtifactResolutionWasSigned = parsedRequest
                    .Element(Saml2Namespaces.SoapEnvelope + "Body")
                    .Element(Saml2Namespaces.Saml2P + "ArtifactResolve")
                    .Element(XNamespace.Get(SignedXml.XmlDsigNamespaceUrl) + "Signature")
                    != null;

                var response = string.Format(
    @"<SOAP-ENV:Envelope
    xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"">
    <SOAP-ENV:Body>
        <samlp:ArtifactResponse
            xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID=""_FQvGknDfws2Z"" Version=""2.0""
            InResponseTo = ""{0}""
            IssueInstant = ""{1}"">
            <Issuer>https://idp.example.com</Issuer>
            <samlp:Status>
                <samlp:StatusCode Value = ""urn:oasis:names:tc:SAML:2.0:status:Success"" />
            </samlp:Status>
            <message>   <child-node /> </message>
        </samlp:ArtifactResponse>
    </SOAP-ENV:Body>
</SOAP-ENV:Envelope>",
                requestId, DateTime.UtcNow.ToSaml2DateTimeString());

				await ctx.Response.WriteAsync(response);
            }
        }

        public static string LastArtifactResolutionSoapActionHeader { get; set; }

        public static bool LastArtifactResolutionWasSigned { get; set; }

        [AssemblyCleanup]
        public static void Stop()
        {
			if (host != null)
			{
				host.Dispose();
				host = null;
			}
        }
    }
}
