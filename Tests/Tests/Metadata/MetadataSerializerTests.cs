using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Selectors;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace Sustainsys.Saml2.Tests.Metadata
{
	class TestMetadataSerializer : MetadataSerializer
	{
		public AdditionalMetadataLocation TestCreateAdditionalMetadataLocationInstance() =>
			base.CreateAdditionalMetadataLocationInstance();

		public AffiliationDescriptor TestCreateAffiliationDescriptorInstance() =>
			base.CreateAffiliationDescriptorInstance();

		public ApplicationServiceEndpoint TestCreateApplicationServiceEndpointInstance() =>
			base.CreateApplicationServiceEndpointInstance();

		public ApplicationServiceDescriptor TestCreateApplicationServiceInstance() =>
			base.CreateApplicationServiceInstance();

		public ArtifactResolutionService TestCreateArtifactResolutionServiceInstance() =>
			base.CreateArtifactResolutionServiceInstance();

		public AssertionConsumerService TestCreateAssertionConsumerServiceInstance() =>
			base.CreateAssertionConsumerServiceInstance();

		public AssertionIdRequestService TestCreateAssertionIdRequestServiceInstance() =>
			base.CreateAssertionIdRequestServiceInstance();

		public AttributeAuthorityDescriptor TestCreateAttributeAuthorityDescriptorInstance() =>
			base.CreateAttributeAuthorityDescriptorInstance();

		public AttributeConsumingService TestCreateAttributeConsumingServiceInstance() =>
			base.CreateAttributeConsumingServiceInstance();

		public AttributeService TestCreateAttributeServiceInstance() =>
			base.CreateAttributeServiceInstance();

		public AttributeProfile TestCreateAttributeProfileInstance() =>
			base.CreateAttributeProfileInstance();

		public AuthnAuthorityDescriptor TestCreateAuthnAuthorityDescriptorInstance() =>
			base.CreateAuthnAuthorityDescriptorInstance();

		public AuthnQueryService TestCreateAuthnQueryServiceInstance() =>
			base.CreateAuthnQueryServiceInstance();

		public AuthzService TestCreateAuthzServiceInstance() =>
			base.CreateAuthzServiceInstance();

		public CipherData TestCreateCipherDataInstance() =>
			base.CreateCipherDataInstance();

		public ClaimValue TestCreateClaimValueInstance() =>
			base.CreateClaimValueInstance();

		public CipherReference TestCreateCipherReferenceInstance() =>
			base.CreateCipherReferenceInstance();

		public ConstrainedValue TestCreateConstrainedValueInstance() =>
			base.CreateConstrainedValueInstance();

		public ContactPerson TestCreateContactPersonInstance() =>
			base.CreateContactPersonInstance();

		public DiscoveryResponse TestCreateDiscoveryResponseInstance() =>
			base.CreateDiscoveryResponseInstance();

		public DSigKeyInfo TestCreateDSigKeyInfoInstance() =>
			base.CreateDSigKeyInfoInstance();

		public PDPDescriptor TestCreatePDPDescriptorInstance() =>
			base.CreatePDPDescriptorInstance();

		public EncryptedData TestCreateEncryptedDataInstance() =>
			base.CreateEncryptedDataInstance();

		public EncryptionProperty TestCreateEncryptionPropertyInstance() =>
			base.CreateEncryptionPropertyInstance();

		public EncryptionProperties TestCreateEncryptionPropertiesInstance() =>
			base.CreateEncryptionPropertiesInstance();

		public EncryptedValue TestCreateEncryptedValueInstance() =>
			base.CreateEncryptedValueInstance();

		public EncryptionMethod TestCreateEncryptionMethodInstance() =>
			base.CreateEncryptionMethodInstance();

		public Endpoint TestCreateEndpointInstance() =>
			base.CreateEndpointInstance();

		public EntitiesDescriptor TestCreateEntitiesDescriptorInstance() =>
			base.CreateEntitiesDescriptorInstance();

		public EntityDescriptor TestCreateEntityDescriptorInstance() =>
			base.CreateEntityDescriptorInstance();

		public IdpSsoDescriptor TestCreateIdpSsoDescriptorInstance() =>
			base.CreateIdpSsoDescriptorInstance();

		public KeyDescriptor TestCreateKeyDescriptorInstance() =>
			base.CreateKeyDescriptorInstance();

		public LocalizedName TestCreateLocalizedNameInstance() =>
			base.CreateLocalizedNameInstance();

		public LocalizedUri TestCreateLocalizedUriInstance() =>
			base.CreateLocalizedUriInstance();

		public ManageNameIDService TestCreateManageNameIDServiceInstance() =>
			base.CreateManageNameIDServiceInstance();

		public NameIDFormat TestCreateNameIDFormatInstance() =>
			base.CreateNameIDFormatInstance();

		public NameIDMappingService TestCreateNameIDMappingServiceInstance() =>
			base.CreateNameIDMappingServiceInstance();

		public Organization TestCreateOrganizationInstance() =>
			base.CreateOrganizationInstance();

		public PassiveRequestorEndpoint TestCreatePassiveRequestorEndpointInstance() =>
			base.CreatePassiveRequestorEndpointInstance();

		public RequestedAttribute TestCreateRequestedAttributeInstance(string name) =>
			base.CreateRequestedAttributeInstance(name);

		public Saml2Attribute TestCreateSaml2AttributeInstance(string name) =>
			base.CreateSaml2AttributeInstance(name);

		public SingleLogoutService TestCreateSingleLogoutServiceInstance() =>
			base.CreateSingleLogoutServiceInstance();

		public SingleSignOutNotificationEndpoint TestCreateSingleSignOutNotificationEndpointInstance() =>
			base.CreateSingleSignOutNotificationEndpointInstance();

		public SecurityTokenServiceDescriptor TestCreateSecurityTokenServiceDescriptorInstance() =>
			base.CreateSecurityTokenServiceDescriptorInstance();

		public SingleSignOnService TestCreateSingleSignOnServiceInstance() =>
			base.CreateSingleSignOnServiceInstance();

		public SpSsoDescriptor TestCreateSpSsoDescriptorInstance() =>
			base.CreateSpSsoDescriptorInstance();

		public EndpointReference TestReadEndpointReference(XmlReader reader) =>
			base.ReadEndpointReference(reader);

		public ApplicationServiceDescriptor TestReadApplicationServiceDescriptor(XmlReader reader) =>
			base.ReadApplicationServiceDescriptor(reader);

		public ContactPerson TestReadContactPerson(XmlReader reader) =>
			base.ReadContactPerson(reader);

		public byte[] TestReadBase64(XmlReader reader) =>
			base.ReadBase64(reader);

		public IdpSsoDescriptor TestReadIdpSsoDescriptor(XmlReader reader) =>
			base.ReadIdpSsoDescriptor(reader);

		public DsaKeyValue TestReadDsaKeyValue(XmlReader reader) =>
			base.ReadDSAKeyValue(reader);

		public RsaKeyValue TestReadRsaKeyValue(XmlReader reader) =>
			base.ReadRSAKeyValue(reader);

		public EcKeyValue TestReadEcKeyValue(XmlReader reader) =>
			base.ReadECKeyValue(reader);

		public EcKeyValue TestReadEcDsaKeyValue(XmlReader reader) =>
			base.ReadECDSAKeyValue(reader);

		public KeyValue TestReadKeyValue(XmlReader reader) =>
			base.ReadKeyValue(reader);

		public KeyDescriptor TestReadKeyDescriptor(XmlReader reader) =>
			base.ReadKeyDescriptor(reader);

		public RetrievalMethod TestReadRetrievalMethod(XmlReader reader) =>
			base.ReadRetrievalMethod(reader);

		public X509IssuerSerial TestReadX509IssuerSerial(XmlReader reader) =>
			base.ReadX509IssuerSerial(reader);

		public X509Digest TestReadX509Digest(XmlReader reader) =>
			base.ReadX509Digest(reader);

		public X509Data TestReadX509Data(XmlReader reader) =>
			base.ReadX509Data(reader);

		public DSigKeyInfo TestReadDSigKeyInfo(XmlReader reader) =>
			base.ReadDSigKeyInfo(reader);

		public EncryptionMethod TestReadEncryptionMethod(XmlReader reader) =>
			base.ReadEncryptionMethod(reader);

		public CipherReference TestReadCipherReference(XmlReader reader) =>
			base.ReadCipherReference(reader);

		public CipherData TestReadCipherData(XmlReader reader) =>
			base.ReadCipherData(reader);

		public EncryptionProperty TestReadEncryptionProperty(XmlReader reader) =>
			base.ReadEncryptionProperty(reader);

		public EncryptionProperties TestReadEncryptionProperties(XmlReader reader) =>
			base.ReadEncryptionProperties(reader);

		public EncryptedData TestReadEncryptedData(XmlReader reader) =>
			base.ReadEncryptedData(reader);

		public EncryptedValue TestReadEncryptedValue(XmlReader reader) =>
			base.ReadEncryptedValue(reader);

		public ClaimValue TestReadClaimValue(XmlReader reader) =>
			base.ReadClaimValue(reader);

		public ConstrainedValue TestReadConstrainedValue(XmlReader reader) =>
			base.ReadConstrainedValue(reader);

		public DisplayClaim TestReadDisplayClaim(XmlReader reader) =>
			base.ReadDisplayClaim(reader);

		public AssertionConsumerService TestReadAssertionConsumerService(XmlReader reader) =>
			base.ReadAssertionConsumerService(reader);

		public EntitiesDescriptor TestReadEntitiesDescriptor(XmlReader reader) =>
			base.ReadEntitiesDescriptor(reader, null);

		public EntityDescriptor TestReadEntityDescriptor(XmlReader reader) =>
			base.ReadEntityDescriptor(reader, null);

		public Saml2Attribute TestReadSaml2Attribute(XmlReader reader) =>
			base.ReadSaml2Attribute(reader);

		public AffiliationDescriptor TestReadAffiliationDescriptor(XmlReader reader) =>
			base.ReadAffiliationDescriptor(reader);

		public AdditionalMetadataLocation TestReadAdditionalMetadataLocation(XmlReader reader) =>
			base.ReadAdditionalMetadataLocation(reader);

		public NameIDFormat TestReadNameIDFormat(XmlReader reader) =>
			base.ReadNameIDFormat(reader);

		public PDPDescriptor TestReadPDPDescriptor(XmlReader reader) =>
			base.ReadPDPDescriptor(reader);

		public AuthnAuthorityDescriptor TestReadAuthnAuthorityDescriptor(XmlReader reader) =>
			base.ReadAuthnAuthorityDescriptor(reader);

		public AttributeAuthorityDescriptor TestReadAttributeAuthorityDescriptor(XmlReader reader) =>
			base.ReadAttributeAuthorityDescriptor(reader);

		public AttributeProfile TestReadAttributeProfile(XmlReader reader) =>
			base.ReadAttributeProfile(reader);

		public LocalizedName TestReadLocalizedName(XmlReader reader) =>
			base.ReadLocalizedName(reader);

		public LocalizedUri TestReadLocalizedUri(XmlReader reader) =>
			base.ReadLocalizedUri(reader);

		public Organization TestReadOrganization(XmlReader reader) =>
			base.ReadOrganization(reader);

		public SecurityTokenServiceDescriptor TestReadSecurityTokenServiceDescriptor(XmlReader reader) =>
			base.ReadSecurityTokenServiceDescriptor(reader);

		public AttributeConsumingService TestReadAttributeConsumingService(XmlReader reader) =>
			base.ReadAttributeConsumingService(reader);

		public RequestedAttribute TestReadRequestedAttribute(XmlReader reader) =>
			base.ReadRequestedAttribute(reader);

		public SpSsoDescriptor TestReadSpSsoDescriptor(XmlReader reader) =>
			base.ReadSpSsoDescriptor(reader);
	}

	[TestClass]
	public class MetadataSerializerTests
	{
		static readonly Regex whitespaceRe = new Regex("[ \t\r\n]");
		static readonly string certData = whitespaceRe.Replace(@"
				MIIC2jCCAkMCAg38MA0GCSqGSIb3DQEBBQUAMIGbMQswCQYDVQQGEwJKUDEOMAwG
				A1UECBMFVG9reW8xEDAOBgNVBAcTB0NodW8ta3UxETAPBgNVBAoTCEZyYW5rNERE
				MRgwFgYDVQQLEw9XZWJDZXJ0IFN1cHBvcnQxGDAWBgNVBAMTD0ZyYW5rNEREIFdl
				YiBDQTEjMCEGCSqGSIb3DQEJARYUc3VwcG9ydEBmcmFuazRkZC5jb20wHhcNMTIw
				ODIyMDUyNzQxWhcNMTcwODIxMDUyNzQxWjBKMQswCQYDVQQGEwJKUDEOMAwGA1UE
				CAwFVG9reW8xETAPBgNVBAoMCEZyYW5rNEREMRgwFgYDVQQDDA93d3cuZXhhbXBs
				ZS5jb20wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQC0z9FeMynsC8+u
				dvX+LciZxnh5uRj4C9S6tNeeAlIGCfQYk0zUcNFCoCkTknNQd/YEiawDLNbxBqut
				bMDZ1aarys1a0lYmUeVLCIqvzBkPJTSQsCopQQ9V8WuT252zzNzs68dVGNdCJd5J
				NRQykpwexmnjPPv0mvj7i8XgG379TyW6P+WWV5okeUkXJ9eJS2ouDYdR2SM9BoVW
				+FgxDu6BmXhozW5EfsnajFp7HL8kQClI0QOc79yuKl3492rH6bzFsFn2lfwWy9ic
				7cP8EpCTeFp1tFaD+vxBhPZkeTQ1HKx6hQ5zeHIB5ySJJZ7af2W8r4eTGYzbdRW2
				4DDHCPhZAgMBAAEwDQYJKoZIhvcNAQEFBQADgYEAQMv+BFvGdMVzkQaQ3/+2noVz
				/uAKbzpEL8xTcxYyP3lkOeh4FoxiSWqy5pGFALdPONoDuYFpLhjJSZaEwuvjI/Tr
				rGhLV1pRG9frwDFshqD2Vaj4ENBCBh6UpeBop5+285zQ4SI7q4U9oSebUDJiuOx6
				+tZ9KynmrbJpTSi0+BMK", "");

		XmlNamespaceManager CreateNamespaceManager(XmlDocument doc)
		{
			var nsmgr = new XmlNamespaceManager(doc.NameTable);
			// const string XmlNs = "http://www.w3.org/XML/1998/namespace";
			nsmgr.AddNamespace("auth", "http://docs.oasis-open.org/wsfed/authorization/200706");
			nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
			nsmgr.AddNamespace("ds11", "http://www.w3.org/2009/xmldsig11#");
			nsmgr.AddNamespace("md", "urn:oasis:names:tc:SAML:2.0:metadata");
			nsmgr.AddNamespace("fed", "http://docs.oasis-open.org/wsfed/federation/200706");
			nsmgr.AddNamespace("idp", "urn:oasis:names:tc:SAML:profiles:SSO:idp-discovery-protocol");
			nsmgr.AddNamespace("saml2", "urn:oasis:names:tc:SAML:2.0:assertion");
			nsmgr.AddNamespace("wsa", "http://www.w3.org/2005/08/addressing");
			nsmgr.AddNamespace("wsp", "http://schemas.xmlsoap.org/ws/2002/12/policy");
			nsmgr.AddNamespace("xenc", "http://www.w3.org/2001/04/xmlenc#");
			nsmgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
			return nsmgr;
		}

		(XmlDocument doc, XmlNamespaceManager nsmgr) LoadXml(string xml)
		{
			var doc = new XmlDocument();
			doc.LoadXml(xml);
			var nsmgr = CreateNamespaceManager(doc);
			return (doc, nsmgr);
		}

		void ReadTest<T>(string xml, T expected, Func<TestMetadataSerializer, XmlReader, T> readFn,
			Func<FluentAssertions.Equivalency.EquivalencyAssertionOptions<T>,
				 FluentAssertions.Equivalency.EquivalencyAssertionOptions<T>> config = null)
		{
			using (var stringReader = new StringReader(xml))
			using (var xmlReader = XmlReader.Create(stringReader))
			{
				xmlReader.MoveToContent();

				var serializer = new TestMetadataSerializer();
				var result = readFn(serializer, xmlReader);
				if (config != null)
				{
					result.Should().BeEquivalentTo(expected, config);
				}
				else
				{
					result.Should().BeEquivalentTo(expected);
				}
			}
		}

		[TestMethod]
		public void MetadataSerializerTests_TestCreateInstance()
		{
			var serializer = new TestMetadataSerializer();
			serializer.TestCreateAdditionalMetadataLocationInstance()
				.Should().BeOfType<AdditionalMetadataLocation>();
			serializer.TestCreateAffiliationDescriptorInstance()
				.Should().BeOfType<AffiliationDescriptor>();
			serializer.TestCreateApplicationServiceEndpointInstance()
				.Should().BeOfType<ApplicationServiceEndpoint>();
			serializer.TestCreateApplicationServiceInstance()
				.Should().BeOfType<ApplicationServiceDescriptor>();
			serializer.TestCreateArtifactResolutionServiceInstance()
				.Should().BeOfType<ArtifactResolutionService>();
			serializer.TestCreateAssertionConsumerServiceInstance()
				.Should().BeOfType<AssertionConsumerService>();
			serializer.TestCreateAssertionIdRequestServiceInstance()
				.Should().BeOfType<AssertionIdRequestService>();
			serializer.TestCreateAttributeAuthorityDescriptorInstance()
				.Should().BeOfType<AttributeAuthorityDescriptor>();
			serializer.TestCreateAttributeConsumingServiceInstance()
				.Should().BeOfType<AttributeConsumingService>();
			serializer.TestCreateAttributeServiceInstance()
				.Should().BeOfType<AttributeService>();
			serializer.TestCreateAttributeProfileInstance()
				.Should().BeOfType<AttributeProfile>();
			serializer.TestCreateAuthnAuthorityDescriptorInstance()
				.Should().BeOfType<AuthnAuthorityDescriptor>();
			serializer.TestCreateAuthnQueryServiceInstance()
				.Should().BeOfType<AuthnQueryService>();
			serializer.TestCreateAuthzServiceInstance()
				.Should().BeOfType<AuthzService>();
			serializer.TestCreateCipherDataInstance()
				.Should().BeOfType<CipherData>();
			serializer.TestCreateClaimValueInstance()
				.Should().BeOfType<ClaimValue>();
			serializer.TestCreateCipherReferenceInstance()
				.Should().BeOfType<CipherReference>();
			serializer.TestCreateConstrainedValueInstance()
				.Should().BeOfType<ConstrainedValue>();
			serializer.TestCreateContactPersonInstance()
				.Should().BeOfType<ContactPerson>();
			serializer.TestCreateDiscoveryResponseInstance()
				.Should().BeOfType<DiscoveryResponse>();
			serializer.TestCreateDSigKeyInfoInstance()
				.Should().BeOfType<DSigKeyInfo>();
			serializer.TestCreatePDPDescriptorInstance()
				.Should().BeOfType<PDPDescriptor>();
			serializer.TestCreateEncryptedDataInstance()
				.Should().BeOfType<EncryptedData>();
			serializer.TestCreateEncryptionPropertyInstance()
				.Should().BeOfType<EncryptionProperty>();
			serializer.TestCreateEncryptionPropertiesInstance()
				.Should().BeOfType<EncryptionProperties>();
			serializer.TestCreateEncryptedValueInstance()
				.Should().BeOfType<EncryptedValue>();
			serializer.TestCreateEncryptionMethodInstance()
				.Should().BeOfType<EncryptionMethod>();
			serializer.TestCreateEndpointInstance()
				.Should().BeOfType<Endpoint>();
			serializer.TestCreateEntitiesDescriptorInstance()
				.Should().BeOfType<EntitiesDescriptor>();
			serializer.TestCreateEntityDescriptorInstance()
				.Should().BeOfType<EntityDescriptor>();
			serializer.TestCreateIdpSsoDescriptorInstance()
				.Should().BeOfType<IdpSsoDescriptor>();
			serializer.TestCreateKeyDescriptorInstance()
				.Should().BeOfType<KeyDescriptor>();
			serializer.TestCreateLocalizedNameInstance()
				.Should().BeOfType<LocalizedName>();
			serializer.TestCreateLocalizedUriInstance()
				.Should().BeOfType<LocalizedUri>();
			serializer.TestCreateManageNameIDServiceInstance()
				.Should().BeOfType<ManageNameIDService>();
			serializer.TestCreateNameIDFormatInstance()
				.Should().BeOfType<NameIDFormat>();
			serializer.TestCreateNameIDMappingServiceInstance()
				.Should().BeOfType<NameIDMappingService>();
			serializer.TestCreateOrganizationInstance()
				.Should().BeOfType<Organization>();
			serializer.TestCreatePassiveRequestorEndpointInstance()
				.Should().BeOfType<PassiveRequestorEndpoint>();
			serializer.TestCreateSingleLogoutServiceInstance()
				.Should().BeOfType<SingleLogoutService>();
			serializer.TestCreateSingleSignOutNotificationEndpointInstance()
				.Should().BeOfType<SingleSignOutNotificationEndpoint>();
			serializer.TestCreateSecurityTokenServiceDescriptorInstance()
				.Should().BeOfType<SecurityTokenServiceDescriptor>();
			serializer.TestCreateSingleSignOnServiceInstance()
				.Should().BeOfType<SingleSignOnService>();
			serializer.TestCreateSpSsoDescriptorInstance()
				.Should().BeOfType<SpSsoDescriptor>();

			var reqAtt = serializer.TestCreateRequestedAttributeInstance("testAttribute");
			reqAtt.Should().BeOfType<RequestedAttribute>();
			reqAtt.Name.Should().Be("testAttribute");

			var saml2Att = serializer.TestCreateSaml2AttributeInstance("saml2Attribute");
			saml2Att.Should().BeOfType<Saml2Attribute>();
			saml2Att.Name.Should().Be("saml2Attribute");
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEndpointReference()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<wsa:EndpointReference
				xmlns:wsa='http://www.w3.org/2005/08/addressing'
				xmlns:wsp='http://schemas.xmlsoap.org/ws/2002/12/policy'>
				<wsa:Address>http://dummy.idp.com/EndpointReference/</wsa:Address>
				<wsa:ReferenceProperties><ref>Some reference props</ref></wsa:ReferenceProperties>
				<wsa:ReferenceParameters><ref>Some reference params</ref></wsa:ReferenceParameters>
				<wsa:Metadata><any>Anything at all</any></wsa:Metadata>
				<wsa:PortType>test:porttype</wsa:PortType>
				<wsp:Policy><some-policy-element/></wsp:Policy>
			</wsa:EndpointReference>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new EndpointReference()
			{
				Uri = new Uri("http://dummy.idp.com/EndpointReference/"),
				ReferenceProperties = {
					doc.SelectSingleNode("/wsa:EndpointReference/wsa:ReferenceProperties/*",
						nsmgr).As<XmlElement>() },
				ReferenceParameters = {
					doc.SelectSingleNode("/wsa:EndpointReference/wsa:ReferenceParameters/*",
						nsmgr).As<XmlElement>() },
				Metadata = {
					doc.SelectSingleNode("/wsa:EndpointReference/wsa:Metadata/*",
						nsmgr).As<XmlElement>() },
				PortType = "test:porttype",
				Policies = {
					doc.SelectSingleNode("/wsa:EndpointReference/wsp:Policy/*",
						nsmgr).As<XmlElement>() }
			};
			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadEndpointReference(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadApplicationServiceDescriptor()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<md:RoleDescriptor
				protocolSupportEnumeration='http://docs.oasis-open.org/wsfed/federation/200706'
				xsi:type='fed:ApplicationServiceType'
				xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
				xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'
				xmlns:fed='http://docs.oasis-open.org/wsfed/federation/200706'
				xmlns:wsa='http://www.w3.org/2005/08/addressing'>
				<fed:ApplicationServiceEndpoint>
					<wsa:EndpointReference>
						<wsa:Address>http://dummy.idp.com/ApplicationServiceEndpoint/</wsa:Address>
						<wsa:ReferenceParameters><ref>Some reference</ref></wsa:ReferenceParameters>
						<wsa:Metadata><any>Anything at all</any></wsa:Metadata>
					</wsa:EndpointReference>
				</fed:ApplicationServiceEndpoint>
				<fed:SingleSignOutNotificationEndpoint>
					<wsa:EndpointReference>
						<wsa:Address>http://dummy.idp.com/SingleSignOutNotificationEndpoint/</wsa:Address>
						<wsa:ReferenceParameters><ref>Some reference 2</ref></wsa:ReferenceParameters>
						<wsa:Metadata><any>Anything at all 2</any></wsa:Metadata>
					</wsa:EndpointReference>
				</fed:SingleSignOutNotificationEndpoint>
				<fed:PassiveRequestorEndpoint>
					<wsa:EndpointReference>
						<wsa:Address>http://dummy.idp.com/PassiveRequestorEndpoint/</wsa:Address>
						<wsa:ReferenceParameters><ref>Some reference 3</ref></wsa:ReferenceParameters>
						<wsa:Metadata><any>Anything at all 3</any></wsa:Metadata>
					</wsa:EndpointReference>
				</fed:PassiveRequestorEndpoint>
			</md:RoleDescriptor>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new ApplicationServiceDescriptor()
			{
				ProtocolsSupported = { new Uri("http://docs.oasis-open.org/wsfed/federation/200706") },
				Endpoints = {
					new EndpointReference()
					{
						Uri = new Uri("http://dummy.idp.com/ApplicationServiceEndpoint/"),
						ReferenceParameters = { doc.SelectSingleNode(
							"md:RoleDescriptor/fed:ApplicationServiceEndpoint/wsa:EndpointReference/wsa:ReferenceParameters/*",
							nsmgr).As<XmlElement>() },
						Metadata = { doc.SelectSingleNode(
							"md:RoleDescriptor/fed:ApplicationServiceEndpoint/wsa:EndpointReference/wsa:Metadata/*",
							nsmgr).As<XmlElement>() }
					}
				},
				SingleSignOutEndpoints = {
					new EndpointReference()
					{
						Uri = new Uri("http://dummy.idp.com/SingleSignOutNotificationEndpoint/"),
						ReferenceParameters = { doc.SelectSingleNode(
							"md:RoleDescriptor/fed:SingleSignOutNotificationEndpoint/wsa:EndpointReference/wsa:ReferenceParameters/*",
							nsmgr).As<XmlElement>() },
						Metadata = { doc.SelectSingleNode(
							"md:RoleDescriptor/fed:SingleSignOutNotificationEndpoint/wsa:EndpointReference/wsa:Metadata/*",
							nsmgr).As<XmlElement>() }
					}
				},
				PassiveRequestorEndpoints = {
					new EndpointReference()
					{
						Uri = new Uri("http://dummy.idp.com/PassiveRequestorEndpoint/"),
						ReferenceParameters = { doc.SelectSingleNode(
							"md:RoleDescriptor/fed:PassiveRequestorEndpoint/wsa:EndpointReference/wsa:ReferenceParameters/*",
							nsmgr).As<XmlElement>() },
						Metadata = { doc.SelectSingleNode(
							"md:RoleDescriptor/fed:PassiveRequestorEndpoint/wsa:EndpointReference/wsa:Metadata/*",
							nsmgr).As<XmlElement>() }
					}
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadApplicationServiceDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadContactPerson()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<md:ContactPerson contactType='technical' xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'>
				<md:Company>Test Company</md:Company>
				<md:GivenName>David</md:GivenName>
				<md:SurName>Test</md:SurName>
				<md:EmailAddress>david.test@test.company</md:EmailAddress>
				<md:EmailAddress>david.test2@test.company</md:EmailAddress>
				<md:TelephoneNumber>0123456789</md:TelephoneNumber>
				<md:TelephoneNumber>9876543210</md:TelephoneNumber>
				<md:Extensions>
					<any-extension-element/>
					<any-other-element/>
				</md:Extensions>
			</md:ContactPerson>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new ContactPerson()
			{
				Type = ContactType.Technical,
				Company = "Test Company",
				GivenName = "David",
				Surname = "Test",
				EmailAddresses = { "david.test@test.company", "david.test2@test.company" },
				TelephoneNumbers = { "0123456789", "9876543210" },
				Extensions = {
					doc.SelectSingleNode("/md:ContactPerson/md:Extensions/*[1]", nsmgr).As<XmlElement>(),
					doc.SelectSingleNode("/md:ContactPerson/md:Extensions/*[2]", nsmgr).As<XmlElement>()
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadContactPerson(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadBase64()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?><b64>dGhpcyBpcyBzb21lIHRlc3QgdGV4dA==</b64>";
			byte[] expected = System.Text.Encoding.UTF8.GetBytes("this is some test text");
			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadBase64(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadIdpSsoDescriptor()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			  <md:IDPSSODescriptor
				xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
				xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion'
				WantAuthnRequestsSigned='true'
				protocolSupportEnumeration='urn:oasis:names:tc:SAML:2.0:protocol'
				cacheDuration='P2Y6M5DT12H35M30S'
				validUntil='2020-01-01T14:32:31'
				errorURL='http://idp.example.com/something/went/wrong'
				ID='yourGUIDhere'>
			    <md:KeyDescriptor use='signing'>
			      <ds:KeyInfo xmlns:ds='http://www.w3.org/2000/09/xmldsig#'>
			        <MyCustomElement xmlns='urn:MyNamespace' />
			      </ds:KeyInfo>
			    </md:KeyDescriptor>
				<md:Extensions>
					<extra-idp-sso-stuff/>
				</md:Extensions>
				<md:Organization>
					<md:Extensions>
						<ext-elt/>
					</md:Extensions>
					<md:OrganizationName xml:lang='en'>Acme Ltd</md:OrganizationName>
					<md:OrganizationDisplayName xml:lang='en'>Acme Ltd (display)</md:OrganizationDisplayName>
					<md:OrganizationURL xml:lang='en'>http://acme.co/</md:OrganizationURL>
				</md:Organization>
				<md:ContactPerson contactType='administrative' xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'>
					<md:Company>Acme Ltd</md:Company>
					<md:GivenName>Wile E</md:GivenName>
					<md:SurName>Coyote</md:SurName>
					<md:EmailAddress>wile.e.coyto@acme.co</md:EmailAddress>
					<md:TelephoneNumber>11223344</md:TelephoneNumber>
					<md:Extensions>
						<time-for-tea/>
						<and-biscuits/>
					</md:Extensions>
				</md:ContactPerson>
				<md:ArtifactResolutionService
					index='1'
					isDefault='false'
					Binding='http://idp.example.com/ars1'
					Location='http://idp.example.com/arsloc1'
					ResponseLocation='http://idp.example.com/arsresp1' />
				<md:ArtifactResolutionService
					index='2'
					isDefault='false'
					Binding='http://idp.example.com/ars2'
					Location='http://idp.example.com/arsloc2'
					ResponseLocation='http://idp.example.com/arsresp2' />
				<md:SingleLogoutService
					Binding='http://idp.example.com/slsbinding'
					Location='http://idp.example.com/slslocation'
					ResponseLocation='http://idp.example.com/slsresponselocation' />
				<md:ManageNameIDService
					Binding='http://idp.example.com/mnibinding'
					Location='http://idp.example.com/mnilocation'
					ResponseLocation='http://idp.example.com/mniresponselocation' />
				<md:NameIDFormat>http://idp.example.com/nameidformat</md:NameIDFormat>
				<md:SingleSignOnService
					Binding='http://idp.example.com/ssobinding'
					Location='http://idp.example.com/ssolocation'
					ResponseLocation='http://idp.example.com/ssoresponselocation' />
				<md:NameIDMappingService
					Binding='http://idp.example.com/ssobinding'
					Location='http://idp.example.com/ssolocation'
					ResponseLocation='http://idp.example.com/ssoresponselocation' />
				<md:AssertionIDRequestService
					Binding='http://idp.example.com/ssobinding'
					Location='http://idp.example.com/ssolocation'
					ResponseLocation='http://idp.example.com/ssoresponselocation' />
				<md:AttributeProfile>http://idp.example.com/attributeprofile</md:AttributeProfile>
				<saml:Attribute Name='testAtt' NameFormat='http://idp.example.com/nameformat' FriendlyName='friendlyAtt'>
					<saml:AttributeValue>attValue</saml:AttributeValue>
				</saml:Attribute>
			  </md:IDPSSODescriptor>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new IdpSsoDescriptor()
			{
				WantAuthnRequestsSigned = true,
				ProtocolsSupported = { new Uri("urn:oasis:names:tc:SAML:2.0:protocol") },
				CacheDuration = new TimeSpan(365 * 2 + 30 * 6 + 5, 12, 35, 30),
				ValidUntil = new DateTime(2020, 01, 01, 14, 32, 31),
				ErrorUrl = new Uri("http://idp.example.com/something/went/wrong"),
				Id = "yourGUIDhere",
				Extensions = {
					doc.SelectSingleNode("/md:IDPSSODescriptor/md:Extensions/*[1]",
						nsmgr).As<XmlElement>()
				},
				Organization = new Organization() {
					Extensions = {
						doc.SelectSingleNode("/md:IDPSSODescriptor/md:Organization/md:Extensions/*[1]",
							nsmgr).As<XmlElement>()
					},
					Names = {
						new LocalizedName("Acme Ltd", "en")
					},
					DisplayNames = {
						new LocalizedName("Acme Ltd (display)", "en")
					},
					Urls = {
						new LocalizedUri(new Uri("http://acme.co/"), "en")
					}
				},
				Contacts = {
					new ContactPerson() {
						Type = ContactType.Administrative,
						Company = "Acme Ltd",
						GivenName = "Wile E",
						Surname = "Coyote",
						EmailAddresses = { "wile.e.coyto@acme.co" },
						TelephoneNumbers = { "11223344" },
						Extensions = {
							doc.SelectSingleNode("/md:IDPSSODescriptor/md:ContactPerson/md:Extensions/*[1]",
								nsmgr).As<XmlElement>(),
							doc.SelectSingleNode("/md:IDPSSODescriptor/md:ContactPerson/md:Extensions/*[2]",
								nsmgr).As<XmlElement>()
						}
					}
				},
				Keys = {
					new KeyDescriptor() {
						Use = KeyType.Signing,
						KeyInfo = new DSigKeyInfo()
					}
				},
				ArtifactResolutionServices = {
					{ 1,  new ArtifactResolutionService() {
						Index = 1,
						IsDefault = false,
						Binding = new Uri("http://idp.example.com/ars1"),
						Location = new Uri("http://idp.example.com/arsloc1"),
						ResponseLocation = new Uri("http://idp.example.com/arsresp1")
					} },
					{ 2, new ArtifactResolutionService() {
						Index = 2,
						IsDefault = false,
						Binding = new Uri("http://idp.example.com/ars2"),
						Location = new Uri("http://idp.example.com/arsloc2"),
						ResponseLocation = new Uri("http://idp.example.com/arsresp2")
					} }
				},
				SingleLogoutServices = {
					new SingleLogoutService() {
						Binding = new Uri("http://idp.example.com/slsbinding"),
						Location = new Uri("http://idp.example.com/slslocation"),
						ResponseLocation = new Uri("http://idp.example.com/slsresponselocation")
					},
				},
				ManageNameIDServices = {
					new ManageNameIDService() {
						Binding = new Uri("http://idp.example.com/mnibinding"),
						Location = new Uri("http://idp.example.com/mnilocation"),
						ResponseLocation = new Uri("http://idp.example.com/mniresponselocation")
					}
				},
				NameIdentifierFormats = {
					new NameIDFormat() {
						Uri = new Uri("http://idp.example.com/nameidformat")
					}
				},
				SingleSignOnServices = {
					new SingleSignOnService() {
						Binding = new Uri("http://idp.example.com/ssobinding"),
						Location = new Uri("http://idp.example.com/ssolocation"),
						ResponseLocation = new Uri("http://idp.example.com/ssoresponselocation")
					}
				},
				NameIDMappingServices = {
					new NameIDMappingService() {
						Binding = new Uri("http://idp.example.com/ssobinding"),
						Location = new Uri("http://idp.example.com/ssolocation"),
						ResponseLocation = new Uri("http://idp.example.com/ssoresponselocation")
					}
				},
				AssertionIDRequestServices = {
					new AssertionIdRequestService() {
						Binding = new Uri("http://idp.example.com/ssobinding"),
						Location = new Uri("http://idp.example.com/ssolocation"),
						ResponseLocation = new Uri("http://idp.example.com/ssoresponselocation")
					}
				},
				AttributeProfiles = {
					new AttributeProfile() {
						Uri = new Uri("http://idp.example.com/attributeprofile")
					}
				},
				SupportedAttributes = {
					new Saml2Attribute("testAtt", "attValue") {
						NameFormat = new Uri("http://idp.example.com/nameformat"),
						FriendlyName = "friendlyAtt"
					}
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadIdpSsoDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAssertionConsumerService()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<md:AssertionConsumerService xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					index='150'
					isDefault='false'
					Binding='http://idp.example.com/acs1'
					Location='http://idp.example.com/acsloc1'
					ResponseLocation='http://idp.example.com/acsresp1'>
				</md:AssertionConsumerService>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new AssertionConsumerService()
			{
				Index = 150,
				IsDefault = false,
				Binding = new Uri("http://idp.example.com/acs1"),
				Location = new Uri("http://idp.example.com/acsloc1"),
				ResponseLocation = new Uri("http://idp.example.com/acsresp1")
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadAssertionConsumerService(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadDSAKeyValue()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<DSAKeyValue xmlns='http://www.w3.org/2000/09/xmldsig#'>
				<P>/KaCzo4Syrom78z3EQ5SbbB4sF7ey80etKII864WF64B81uRpH5t9jQTxeEu0ImbzRMqzVDZkVG9xD7nN1kuFw==</P>
				<Q>li7dzDacuo67Jg7mtqEm2TRuOMU=</Q>
				<G>Z4Rxsnqc9E7pGknFFH2xqaryRPBaQ01khpMdLRQnG541Awtx/XPaF5Bpsy4pNWMOHCBiNU0NogpsQW5QvnlMpA==</G>
				<Y>qV38IqrWJG0V/mZQvRVi1OHw9Zj84nDC4jO8P0axi1gb6d+475yhMjSc/BrIVC58W3ydbkK+Ri4OKbaRZlYeRA==</Y>
				<J>qV38IqrWJG0V/mZQvRVi1OHw9Zj84nDC4jO8P0axi1gb6d+475yhMjSc/BrIVC58W3ydbkK+Ri4OKbaRZlYeRA==</J>
				<Seed>qV38IqrWJG0V/mZQvRVi1OHw9Zj84nDC4jO8P0axi1gb6d+475yhMjSc/BrIVC58W3ydbkK+Ri4OKbaRZlYeRA==</Seed>
				<PgenCounter>pxXTng==</PgenCounter>
			</DSAKeyValue>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new DsaKeyValue(
				new DSAParameters {
					P = Convert.FromBase64String("/KaCzo4Syrom78z3EQ5SbbB4sF7ey80etKII864WF64B81uRpH5t9jQTxeEu0ImbzRMqzVDZkVG9xD7nN1kuFw=="),
					Q = Convert.FromBase64String("li7dzDacuo67Jg7mtqEm2TRuOMU="),
					G = Convert.FromBase64String("Z4Rxsnqc9E7pGknFFH2xqaryRPBaQ01khpMdLRQnG541Awtx/XPaF5Bpsy4pNWMOHCBiNU0NogpsQW5QvnlMpA=="),
					Y = Convert.FromBase64String("qV38IqrWJG0V/mZQvRVi1OHw9Zj84nDC4jO8P0axi1gb6d+475yhMjSc/BrIVC58W3ydbkK+Ri4OKbaRZlYeRA=="),
					// (rubbish, but will do for a test)
					J = Convert.FromBase64String("qV38IqrWJG0V/mZQvRVi1OHw9Zj84nDC4jO8P0axi1gb6d+475yhMjSc/BrIVC58W3ydbkK+Ri4OKbaRZlYeRA=="),
					Seed = Convert.FromBase64String("qV38IqrWJG0V/mZQvRVi1OHw9Zj84nDC4jO8P0axi1gb6d+475yhMjSc/BrIVC58W3ydbkK+Ri4OKbaRZlYeRA=="),
					Counter = -1491741794
				}
			);

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadDsaKeyValue(reader),
				opts => opts.ComparingByMembers<DSAParameters>());
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadRSAKeyValue()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<RSAKeyValue xmlns='http://www.w3.org/2000/09/xmldsig#'>
				  <Modulus>xA7SEU+e0yQH5rm9kbCDN9o3aPIo7HbP7tX6WOocLZAtNfyxSZDU16ksL6WjubafOqNEpcwR3RdFsT7bCqnXPBe5ELh5u4VEy19MzxkXRgrMvavzyBpVRgBUwUlV5foK5hhmbktQhyNdy/6LpQRhDUDsTvK+g9Ucj47es9AQJ3U=</Modulus>
				  <Exponent>AQAB</Exponent>
				</RSAKeyValue>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new RsaKeyValue(
				new RSAParameters
				{
					Modulus = Convert.FromBase64String("xA7SEU+e0yQH5rm9kbCDN9o3aPIo7HbP7tX6WOocLZAtNfyxSZDU16ksL6WjubafOqNEpcwR3RdFsT7bCqnXPBe5ELh5u4VEy19MzxkXRgrMvavzyBpVRgBUwUlV5foK5hhmbktQhyNdy/6LpQRhDUDsTvK+g9Ucj47es9AQJ3U="),
					Exponent = Convert.FromBase64String("AQAB")
				}
			);

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadRsaKeyValue(reader),
				opts => opts.ComparingByMembers<RSAParameters>());
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadECKeyValue()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<ECKeyValue xmlns='http://www.w3.org/2009/xmldsig11#'>
				  <NamedCurve URI='urn:oid:1.2.840.10045.3.1.7' />
				  <PublicKey>BOVKaiLPKEDChhkA64UEBOXTv/VFHnhrUPN+bXqCvEl7rroAYpH5tKzbiGTtMSlp4JO9Pxg44zeX7EoWDvOrpD0=</PublicKey>
				</ECKeyValue>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new EcKeyValue(
				new ECParameters {
					Curve = ECCurve.CreateFromOid(new Oid("1.2.840.10045.3.1.7")),
					Q = new ECPoint {
						X = new byte[] {
							0xe5, 0x4a, 0x6a, 0x22, 0xcf, 0x28, 0x40, 0xc2, 0x86, 0x19, 0x00,
							0xeb, 0x85, 0x04, 0x04, 0xe5, 0xd3, 0xbf, 0xf5, 0x45, 0x1e, 0x78,
							0x6b, 0x50, 0xf3, 0x7e, 0x6d, 0x7a, 0x82, 0xbc, 0x49, 0x7b },
						Y = new byte[] {
								0xae, 0xba, 0x00, 0x62, 0x91, 0xf9, 0xb4, 0xac, 0xdb, 0x88, 0x64,
								0xed, 0x31, 0x29, 0x69, 0xe0, 0x93, 0xbd, 0x3f, 0x18, 0x38, 0xe3,
								0x37, 0x97, 0xec, 0x4a, 0x16, 0x0e, 0xf3, 0xab, 0xa4, 0x3d }
					}
				}
			);

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadEcKeyValue(reader),
				opts => opts.ComparingByMembers<ECParameters>()
							.ComparingByMembers<ECCurve>()
							.ComparingByMembers<ECPoint>()
							.ComparingByMembers<Oid>()
							.WithTracing());
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadECDSAKeyValue()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<ECDSAKeyValue xmlns='http://www.w3.org/2001/04/xmldsig-more#'>
				  <DomainParameters>
					<NamedCurve URN='urn:oid:1.2.840.10045.3.1.7' />
				  </DomainParameters>
				  <PublicKey>
					<X Value='58511060653801744393249179046482833320204931884267326155134056258624064349885' />
					<Y Value='102403352136827775240910267217779508359028642524881540878079119895764161434936' />
				  </PublicKey>
				</ECDSAKeyValue>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new EcKeyValue(
				new ECParameters {
					Curve = ECCurve.CreateFromOid(new Oid("1.2.840.10045.3.1.7")),
					Q = new ECPoint
					{
						X = BigInteger.Parse("58511060653801744393249179046482833320204931884267326155134056258624064349885").ToByteArray(),
						Y = BigInteger.Parse("102403352136827775240910267217779508359028642524881540878079119895764161434936").ToByteArray()
					}
				}
			);

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadEcDsaKeyValue(reader),
				opts => opts.ComparingByMembers<ECParameters>()
							.ComparingByMembers<ECCurve>()
							.ComparingByMembers<ECPoint>()
							.ComparingByMembers<Oid>()
							.WithTracing());
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadKeyValue()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<KeyValue xmlns='http://www.w3.org/2000/09/xmldsig#'>
					<RSAKeyValue>
					  <Modulus>xA7SEU+e0yQH5rm9kbCDN9o3aPIo7HbP7tX6WOocLZAtNfyxSZDU16ksL6WjubafOqNEpcwR3RdFsT7bCqnXPBe5ELh5u4VEy19MzxkXRgrMvavzyBpVRgBUwUlV5foK5hhmbktQhyNdy/6LpQRhDUDsTvK+g9Ucj47es9AQJ3U=</Modulus>
					  <Exponent>AQAB</Exponent>
					</RSAKeyValue>
				</KeyValue>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new RsaKeyValue(
				new RSAParameters
				{
					Modulus = Convert.FromBase64String("xA7SEU+e0yQH5rm9kbCDN9o3aPIo7HbP7tX6WOocLZAtNfyxSZDU16ksL6WjubafOqNEpcwR3RdFsT7bCqnXPBe5ELh5u4VEy19MzxkXRgrMvavzyBpVRgBUwUlV5foK5hhmbktQhyNdy/6LpQRhDUDsTvK+g9Ucj47es9AQJ3U="),
					Exponent = Convert.FromBase64String("AQAB")
				}
			);

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadKeyValue(reader),
				opts => opts.ComparingByMembers<RSAParameters>().RespectingRuntimeTypes());
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadRetrievalMethod()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<RetrievalMethod xmlns='http://www.w3.org/2000/09/xmldsig#'
					URI='http://idp.example.com/signingCert.cer'
					Type='http://idp.example.com/x509certtype'>
					<Transforms>
						<unparsed-extra-elements/>
						<of-any-kind/>
					</Transforms>
				</RetrievalMethod>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new RetrievalMethod {
				Uri = new Uri("http://idp.example.com/signingCert.cer"),
				Type = new Uri("http://idp.example.com/x509certtype"),
				Transforms = {
					doc.SelectSingleNode("/ds:RetrievalMethod/ds:Transforms/*[1]",
						nsmgr).As<XmlElement>(),
					doc.SelectSingleNode("/ds:RetrievalMethod/ds:Transforms/*[2]",
						nsmgr).As<XmlElement>()
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadRetrievalMethod(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadX509IssuerSerial()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<X509IssuerSerial xmlns='http://www.w3.org/2000/09/xmldsig#'>
					<X509IssuerName>Test Issuer</X509IssuerName>
					<X509SerialNumber>128976</X509SerialNumber>
				</X509IssuerSerial>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new X509IssuerSerial("Test Issuer", "128976");

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadX509IssuerSerial(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadX509Digest()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<X509Digest xmlns='http://www.w3.org/2009/xmldsig11#' Algorithm='http://w3c.org/madeUpAlgorithm'>BOVKaiLPKEDChhkA64UEBOXTv/VFHnhrUPN+bXqCvEl7rroAYpH5tKzbiGTtMSlp4JO9Pxg44zeX7EoWDvOrpD0=</X509Digest>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new X509Digest
			{
				Algorithm = new Uri("http://w3c.org/madeUpAlgorithm"),
				Value = Convert.FromBase64String("BOVKaiLPKEDChhkA64UEBOXTv/VFHnhrUPN+bXqCvEl7rroAYpH5tKzbiGTtMSlp4JO9Pxg44zeX7EoWDvOrpD0=")
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadX509Digest(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadX509Data()
		{
			string crlData = whitespaceRe.Replace(@"
				MIIBYDCBygIBATANBgkqhkiG9w0BAQUFADBDMRMwEQYKCZImiZPyLGQBGRYDY29t
				MRcwFQYKCZImiZPyLGQBGRYHZXhhbXBsZTETMBEGA1UEAxMKRXhhbXBsZSBDQRcN
				MDUwMjA1MTIwMDAwWhcNMDUwMjA2MTIwMDAwWjAiMCACARIXDTA0MTExOTE1NTcw
				M1owDDAKBgNVHRUEAwoBAaAvMC0wHwYDVR0jBBgwFoAUCGivhTPIOUp6+IKTjnBq
				SiCELDIwCgYDVR0UBAMCAQwwDQYJKoZIhvcNAQEFBQADgYEAItwYffcIzsx10NBq
				m60Q9HYjtIFutW2+DvsVFGzIF20f7pAXom9g5L2qjFXejoRvkvifEBInr0rUL4Xi
				NkR9qqNMJTgV/wD9Pn7uPSYS69jnK2LiK8NGgO94gtEVxtCccmrLznrtZ5mLbnCB
				fUNCdMGmr8FVF6IzTNYGmCuk/C4=", "");

			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			  <X509Data xmlns='http://www.w3.org/2000/09/xmldsig#'>
				<X509IssuerSerial> 
				  <X509IssuerName>
					C=JP, ST=Tokyo, L=Chuo-ku, O=Frank4DD, OU=WebCert Support, CN=Frank4DD Web CA/emailAddress=support@frank4dd.com
				  </X509IssuerName>
				  <X509SerialNumber>3580</X509SerialNumber>
				</X509IssuerSerial>
				<X509SKI>31d97bd7</X509SKI> 
				<X509SubjectName>C=JP, ST=Tokyo, O=Frank4DD, CN=www.example.com</X509SubjectName>
				<X509Certificate>" + certData + @"</X509Certificate>
			    <X509CRL>" + crlData + @"</X509CRL>
			  </X509Data>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new X509Data {
				IssuerSerial = new X509IssuerSerial(
					"C=JP, ST=Tokyo, L=Chuo-ku, O=Frank4DD, OU=WebCert Support, CN=Frank4DD Web CA/emailAddress=support@frank4dd.com",
					"3580"
				),
				SKI = Convert.FromBase64String("31d97bd7"),
				SubjectName = "C=JP, ST=Tokyo, O=Frank4DD, CN=www.example.com",
				Certificates = {
					new X509Certificate2(Convert.FromBase64String(certData))
				},
				CRL = Convert.FromBase64String(crlData)
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadX509Data(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadDSigKeyInfo()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<KeyInfo xmlns='http://www.w3.org/2000/09/xmldsig#' Id='testId'>
				<X509Data>
					<X509IssuerSerial> 
					  <X509IssuerName>
						C=JP, ST=Tokyo, L=Chuo-ku, O=Frank4DD, OU=WebCert Support, CN=Frank4DD Web CA/emailAddress=support@frank4dd.com
					  </X509IssuerName>
					  <X509SerialNumber>3580</X509SerialNumber>
					</X509IssuerSerial>
				</X509Data>
				<KeyName>NameOfKey</KeyName>
				<KeyValue>
					<RSAKeyValue>
					  <Modulus>xA7SEU+e0yQH5rm9kbCDN9o3aPIo7HbP7tX6WOocLZAtNfyxSZDU16ksL6WjubafOqNEpcwR3RdFsT7bCqnXPBe5ELh5u4VEy19MzxkXRgrMvavzyBpVRgBUwUlV5foK5hhmbktQhyNdy/6LpQRhDUDsTvK+g9Ucj47es9AQJ3U=</Modulus>
					  <Exponent>AQAB</Exponent>
					</RSAKeyValue>
				</KeyValue>
				<RetrievalMethod
					URI='http://idp.example.com/signingCert.cer'
					Type='http://idp.example.com/x509certtype'>
				</RetrievalMethod>
			</KeyInfo>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new DSigKeyInfo {
				Id = "testId",
				RetrievalMethods = {
					new RetrievalMethod {
						Uri = new Uri("http://idp.example.com/signingCert.cer"),
						Type = new Uri("http://idp.example.com/x509certtype")
					}
				},
				Data = {
					new X509Data {
						IssuerSerial = new X509IssuerSerial(
							"C=JP, ST=Tokyo, L=Chuo-ku, O=Frank4DD, OU=WebCert Support, CN=Frank4DD Web CA/emailAddress=support@frank4dd.com",
							"3580"
						)
					}
				},
				KeyNames = {
					"NameOfKey"
				},
				KeyValues = {
					new RsaKeyValue(
						new RSAParameters
						{
							Modulus = Convert.FromBase64String("xA7SEU+e0yQH5rm9kbCDN9o3aPIo7HbP7tX6WOocLZAtNfyxSZDU16ksL6WjubafOqNEpcwR3RdFsT7bCqnXPBe5ELh5u4VEy19MzxkXRgrMvavzyBpVRgBUwUlV5foK5hhmbktQhyNdy/6LpQRhDUDsTvK+g9Ucj47es9AQJ3U="),
							Exponent = Convert.FromBase64String("AQAB")
						}
					)
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadDSigKeyInfo(reader),
				opts => opts.ComparingByMembers<RSAParameters>());
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEncryptionMethod()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<EncryptionMethod xmlns='http://www.w3.org/2001/04/xmlenc#'
				xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
				Algorithm='http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p'>
				<KeySize> 2048</KeySize>
				<OAEPparams> 9lWu3Q== </OAEPparams>
				<ds:DigestMethod Algorithm='http://www.w3.org/2000/09/xmldsig#sha1'/>
			</EncryptionMethod>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new EncryptionMethod
			{
				OAEPparams = Convert.FromBase64String("9lWu3Q=="),
				Algorithm = new Uri("http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p"),
				KeySize = 2048
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadEncryptionMethod(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadCipherReference()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			  <CipherReference xmlns='http://www.w3.org/2001/04/xmlenc#'
				xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
				URI='http://www.example.com/CipherValues.xml'>
				<Transforms>
				  <ds:Transform 
				   Algorithm='http://www.w3.org/TR/1999/REC-xpath-19991116'>
					  <ds:XPath xmlns:rep='http://www.example.org/repository'>
						self::text()[parent::rep:CipherValue[@Id='example1']]
					  </ds:XPath>
				  </ds:Transform>
				  <ds:Transform Algorithm='http://www.w3.org/2000/09/xmldsig#base64'/>
				</Transforms>
			  </CipherReference>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new CipherReference
			{
				Uri = new Uri("http://www.example.com/CipherValues.xml"),
				Transforms = {
					doc.SelectSingleNode("/xenc:CipherReference/xenc:Transforms/*[1]",
						nsmgr).As<XmlElement>(),
					doc.SelectSingleNode("/xenc:CipherReference/xenc:Transforms/*[2]",
						nsmgr).As<XmlElement>()
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadCipherReference(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadCipherData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			  <CipherData xmlns='http://www.w3.org/2001/04/xmlenc#'>
				<CipherValue>DEADBEEF</CipherValue>
			    <CipherReference URI='http://www.example.com/CipherValues.xml'/>
			  </CipherData>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new CipherData
			{
				CipherValue = "DEADBEEF",
				CipherReference = new CipherReference() {
					Uri = new Uri("http://www.example.com/CipherValues.xml"),
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadCipherData(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEncryptionProperty()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			  <EncryptionProperty xmlns='http://www.w3.org/2001/04/xmlenc#'
				Target='http://enc.org/target'
				Id='someId'>
				<AnythingYouLike/>
			  </EncryptionProperty>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new EncryptionProperty
			{
				Target = new Uri("http://enc.org/target"),
				Id = "someId"
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadEncryptionProperty(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEncryptionProperties()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<EncryptionProperties Id='anId'>
				  <EncryptionProperty xmlns='http://www.w3.org/2001/04/xmlenc#'
					Target='http://enc.org/target'
					Id='someId'>
					<AnythingYouLike/>
				  </EncryptionProperty>
			</EncryptionProperties>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new EncryptionProperties
			{
				Properties = {
					new EncryptionProperty
					{
						Target = new Uri("http://enc.org/target"),
						Id = "someId"
					}
				},
				Id = "anId"
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadEncryptionProperties(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEncryptedData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<EncryptedData
				xmlns='http://www.w3.org/2001/04/xmlenc#'
				xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
				Id='ENCDATAID'
				MimeType='text/plain'
				Encoding='https://encoding.org'
				Type='http://www.w3.org/2001/04/xmlenc#Element'>
				<EncryptionMethod
					Algorithm='http://www.w3.org/2001/04/xmlenc#tripledes-cbc'/>
				<ds:KeyInfo xmlns:ds='http://www.w3.org/2000/09/xmldsig#'>
					<ds:KeyName>John Smith</ds:KeyName>
				</ds:KeyInfo>
				<CipherData><CipherValue>DEADBEEF</CipherValue></CipherData>
				<EncryptionProperties Id='anId2'>
					  <EncryptionProperty xmlns='http://www.w3.org/2001/04/xmlenc#'
						Target='http://enc.org/target2'
						Id='someId2'>
						<AnythingYouLike/>
					  </EncryptionProperty>
				</EncryptionProperties>
			</EncryptedData>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new EncryptedData
			{
				CipherData = new CipherData
				{
					CipherValue = "DEADBEEF"
				},
				KeyInfo = new DSigKeyInfo
				{
					KeyNames = {
						"John Smith"
					}
				},
				EncryptionMethod = new EncryptionMethod
				{
					Algorithm = new Uri("http://www.w3.org/2001/04/xmlenc#tripledes-cbc")
				},
				EncryptionProperties = new EncryptionProperties
				{
					Properties = {
						new EncryptionProperty
						{
							Target = new Uri("http://enc.org/target2"),
							Id = "someId2"
						}
					},
					Id = "anId2"
				},
				Id = "ENCDATAID",
				Type = new Uri("http://www.w3.org/2001/04/xmlenc#Element"),
				MimeType = "text/plain",
				Encoding = new Uri("https://encoding.org")
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadEncryptedData(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEncryptedValue()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<auth:EncryptedValue
				xmlns:xenc='http://www.w3.org/2001/04/xmlenc#'
				xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
				xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
				<xenc:EncryptedData>
					<xenc:EncryptionMethod
						Algorithm='http://www.w3.org/2001/04/xmlenc#tripledes-cbc'/>
					<ds:KeyInfo xmlns:ds='http://www.w3.org/2000/09/xmldsig#'>
						<ds:KeyName>John Smith</ds:KeyName>
					</ds:KeyInfo>
					<xenc:CipherData><xenc:CipherValue>DEADBEEF</xenc:CipherValue></xenc:CipherData>
				</xenc:EncryptedData>
			</auth:EncryptedValue>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new EncryptedValue
			{
				EncryptedData = new EncryptedData()
				{
					CipherData = new CipherData
					{
						CipherValue = "DEADBEEF"
					},
					KeyInfo = new DSigKeyInfo
					{
						KeyNames = {
							"John Smith"
						}
					},
					EncryptionMethod = new EncryptionMethod
					{
						Algorithm = new Uri("http://www.w3.org/2001/04/xmlenc#tripledes-cbc")
					}
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadEncryptedValue(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadClaimValue_Value()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<auth:ValueLessThan
				xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
				<auth:Value>Some Value</auth:Value>
			</auth:ValueLessThan>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new ClaimValue
			{
				Value = "Some Value"
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadClaimValue(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadClaimValue_StructuredValue()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<auth:ValueLessThan
				xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
				<auth:StructuredValue>
					<xml-element-1/>
				</auth:StructuredValue>
			</auth:ValueLessThan>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new ClaimValue
			{
				StructuredValue = new Collection<XmlElement> {
					doc.SelectSingleNode("/auth:ValueLessThan/auth:StructuredValue/*[1]",
						nsmgr).As<XmlElement>()
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadClaimValue(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadConstainedValue()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<auth:ConstrainedValue
				xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'
				AssertConstraint='true'>
				<auth:ValueLessThan><auth:Value>10</auth:Value></auth:ValueLessThan>
				<auth:ValueLessThanOrEqual><auth:Value>11</auth:Value></auth:ValueLessThanOrEqual>
				<auth:ValueGreaterThan><auth:Value>-5</auth:Value></auth:ValueGreaterThan>
				<auth:ValueGreaterThanOrEqual><auth:Value>-4</auth:Value></auth:ValueGreaterThanOrEqual>
				<auth:ValueInRange>
					<auth:ValueUpperBound><auth:Value>10000</auth:Value></auth:ValueUpperBound>
					<auth:ValueLowerBound><auth:Value>0</auth:Value></auth:ValueLowerBound>
				</auth:ValueInRange>
				<auth:ValueOneOf>
					<auth:Value>10</auth:Value>
					<auth:StructuredValue>
						<xml-element-1/>
					</auth:StructuredValue>
				</auth:ValueOneOf>
			</auth:ConstrainedValue>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new ConstrainedValue
			{
				AssertConstraint = true,
				Constraints = {
					new ConstrainedValue.CompareConstraint(ConstrainedValue.CompareConstraint.CompareOperator.Lt)
					{
						Value = new ClaimValue { Value = "10" }
					},
					new ConstrainedValue.CompareConstraint(ConstrainedValue.CompareConstraint.CompareOperator.Lte)
					{
						Value = new ClaimValue { Value = "11" }
					},
					new ConstrainedValue.CompareConstraint(ConstrainedValue.CompareConstraint.CompareOperator.Gt)
					{
						Value = new ClaimValue { Value = "-5" }
					},
					new ConstrainedValue.CompareConstraint(ConstrainedValue.CompareConstraint.CompareOperator.Gte)
					{
						Value = new ClaimValue { Value = "-4" }
					},
					new ConstrainedValue.RangeConstraint
					{
						LowerBound = new ClaimValue { Value = "0" },
						UpperBound = new ClaimValue { Value = "10000" },
					},
					new ConstrainedValue.ListConstraint
					{
						Values = {
							new ClaimValue { Value = "10" },
							new ClaimValue
							{
								StructuredValue = new Collection<XmlElement> {
									doc.SelectSingleNode(
										"/auth:ConstrainedValue/auth:ValueOneOf/auth:StructuredValue[1]/*[1]",
										nsmgr).As<XmlElement>()
								}
							}
						}
					}
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadConstrainedValue(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadDisplayClaim()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<auth:ClaimType Uri='https://saml.claim.type/' Optional='false'
				xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'
				xmlns:xenc='http://www.w3.org/2001/04/xmlenc#'
				xmlns:ds='http://www.w3.org/2000/09/xmldsig#'>
				<auth:DisplayName>The claim name</auth:DisplayName>
				<auth:Description>The claim description</auth:Description>
				<auth:DisplayValue>I claim the moon</auth:DisplayValue>
				<auth:Value>moon</auth:Value>
				<auth:StructuredValue><moon/></auth:StructuredValue>
				<auth:EncryptedValue>
					<xenc:EncryptedData>
						<xenc:EncryptionMethod
							Algorithm='http://www.w3.org/2001/04/xmlenc#tripledes-cbc'/>
						<ds:KeyInfo xmlns:ds='http://www.w3.org/2000/09/xmldsig#'>
							<ds:KeyName>John Smith</ds:KeyName>
						</ds:KeyInfo>
						<xenc:CipherData><xenc:CipherValue>DEADBEEF</xenc:CipherValue></xenc:CipherData>
					</xenc:EncryptedData>
				</auth:EncryptedValue>
			</auth:ClaimType>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new DisplayClaim("https://saml.claim.type/")
			{
				Optional = false,
				DisplayName = "The claim name",
				Description = "The claim description",
				DisplayValue = "I claim the moon",
				Value = "moon",
				StructuredValue = new Collection<XmlElement>
				{
					doc.SelectSingleNode("/auth:ClaimType/auth:StructuredValue/*[1]",
						nsmgr).As<XmlElement>()
				},
				EncryptedValue = new EncryptedValue
				{
					EncryptedData = new EncryptedData()
					{
						CipherData = new CipherData
						{
							CipherValue = "DEADBEEF"
						},
						KeyInfo = new DSigKeyInfo
						{
							KeyNames = {
								"John Smith"
							}
						},
						EncryptionMethod = new EncryptionMethod
						{
							Algorithm = new Uri("http://www.w3.org/2001/04/xmlenc#tripledes-cbc")
						}
					}
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadDisplayClaim(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEntitiesDescriptor()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<EntitiesDescriptor Name='https://your-federation.org/metadata/federation-name.xml'
				xmlns='urn:oasis:names:tc:SAML:2.0:metadata'
				xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
				xmlns:shibmd='urn:mace:shibboleth:metadata:1.0'
				xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
 
				<!-- Actual providers go here.  -->
				<!-- An identity provider. -->
				<EntityDescriptor entityID='https://idp.example.org/idp/shibboleth'>
				   <IDPSSODescriptor protocolSupportEnumeration='urn:mace:shibboleth:1.0 urn:oasis:names:tc:SAML:2.0:protocol'>
					  <Extensions>
						 <shibmd:Scope regexp='false'>example.org</shibmd:Scope>
					  </Extensions>
					  <KeyDescriptor>
						 <ds:KeyInfo>
							<ds:X509Data>
							   <ds:X509Certificate>" + certData + @"</ds:X509Certificate>
							 </ds:X509Data>
						  </ds:KeyInfo>
					   </KeyDescriptor>
 
					   <NameIDFormat>urn:mace:shibboleth:1.0:nameIdentifier</NameIDFormat>
					   <NameIDFormat>urn:oasis:names:tc:SAML:2.0:nameid-format:transient</NameIDFormat>
 
					   <SingleSignOnService Binding='urn:mace:shibboleth:1.0:profiles:AuthnRequest'
								Location='https://idp.example.org/idp/profile/Shibboleth/SSO' />
         
					   <SingleSignOnService Binding='urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST'
								Location='https://idp.example.org/idp/profile/SAML2/POST/SSO' />
 
					   <SingleSignOnService Binding='urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect'
								Location='https://idp.example.org/idp/profile/SAML2/Redirect/SSO' />
				   </IDPSSODescriptor>
     
				   <AttributeAuthorityDescriptor protocolSupportEnumeration='urn:oasis:names:tc:SAML:1.1:protocol urn:oasis:names:tc:SAML:2.0:protocol'>
					   <KeyDescriptor>
						   <ds:KeyInfo>
							   <ds:X509Data>
								   <ds:X509Certificate>" + certData + @"
								   </ds:X509Certificate>
							   </ds:X509Data>
						   </ds:KeyInfo>
					   </KeyDescriptor>
 
					   <AttributeService Binding='urn:oasis:names:tc:SAML:1.0:bindings:SOAP-binding'
									  Location='https://idp.example.org:8443/idp/profile/SAML1/SOAP/AttributeQuery' />
         
					   <AttributeService Binding='urn:oasis:names:tc:SAML:2.0:bindings:SOAP'
									  Location='https://idp.example.org:8443/idp/profile/SAML2/SOAP/AttributeQuery' />
         
					   <NameIDFormat>urn:mace:shibboleth:1.0:nameIdentifier</NameIDFormat>
					   <NameIDFormat>urn:oasis:names:tc:SAML:2.0:nameid-format:transient</NameIDFormat>
 
				   </AttributeAuthorityDescriptor>

				   <Organization>
							<OrganizationName xml:lang='en'>Your Identities</OrganizationName>
							<OrganizationDisplayName xml:lang='en'>Your Identities</OrganizationDisplayName>
							<OrganizationURL xml:lang='en'>http://www.example.org/</OrganizationURL>
					</Organization>
					<ContactPerson contactType='technical'>
							<GivenName>Your</GivenName>
							<SurName>Contact</SurName>
							<EmailAddress>admin@example.org</EmailAddress>
					</ContactPerson>
				</EntityDescriptor>
  
				<!-- A service provider. -->
				<EntityDescriptor entityID='https://sp.example.org/shibboleth-sp'>
					<SPSSODescriptor protocolSupportEnumeration='urn:oasis:names:tc:SAML:2.0:protocol urn:oasis:names:tc:SAML:1.1:protocol'>
						<Extensions>
							<idpdisc:DiscoveryResponse xmlns:idpdisc='urn:oasis:names:tc:SAML:profiles:SSO:idp-discovery-protocol'
									index='1' Binding='urn:oasis:names:tc:SAML:profiles:SSO:idp-discovery-protocol'
									Location='http://sp.example.org/Shibboleth.sso/DS'/>
							<idpdisc:DiscoveryResponse xmlns:idpdisc='urn:oasis:names:tc:SAML:profiles:SSO:idp-discovery-protocol'
									index='2' Binding='urn:oasis:names:tc:SAML:profiles:SSO:idp-discovery-protocol'
									Location='https://sp.example.org/Shibboleth.sso/DS'/>
						</Extensions>
 
					<KeyDescriptor>
							<ds:KeyInfo>
								<ds:X509Data>
									<ds:X509Certificate>" + certData + @"
									</ds:X509Certificate>
								</ds:X509Data>
							</ds:KeyInfo>
						</KeyDescriptor>
 
						<NameIDFormat>urn:oasis:names:tc:SAML:2.0:nameid-format:transient</NameIDFormat>
						<NameIDFormat>urn:mace:shibboleth:1.0:nameIdentifier</NameIDFormat>
 
						<AssertionConsumerService index='1' isDefault='true'
								Binding='urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST'
								Location='https://sp.example.org/Shibboleth.sso/SAML2/POST'/>
						<AssertionConsumerService index='2'
								Binding='urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST-SimpleSign'
								Location='https://sp.example.org/Shibboleth.sso/SAML2/POST-SimpleSign'/>
						<AssertionConsumerService index='3'
								Binding='urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Artifact'
								Location='https://sp.example.org/Shibboleth.sso/SAML2/Artifact'/>
						<AssertionConsumerService index='4'
								Binding='urn:oasis:names:tc:SAML:1.0:profiles:browser-post'
								Location='https://sp.example.org/Shibboleth.sso/SAML/POST'/>
						<AssertionConsumerService index='5'
								Binding='urn:oasis:names:tc:SAML:1.0:profiles:artifact-01'
								Location='https://sp.example.org/Shibboleth.sso/SAML/Artifact'/>
 
					</SPSSODescriptor>
 
				<Organization>
					<OrganizationName xml:lang='en'>Your Service</OrganizationName>
					<OrganizationDisplayName xml:lang='en'>Your Service</OrganizationDisplayName>
					<OrganizationURL xml:lang='en'>http://sp.example.org/</OrganizationURL>
				</Organization>
				<ContactPerson contactType='technical'>
					<GivenName>Your</GivenName>
					<SurName>Admin</SurName>
					<EmailAddress>admin@example.org</EmailAddress>
				</ContactPerson>
				</EntityDescriptor>
			</EntitiesDescriptor>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new EntitiesDescriptor {
				Name = "https://your-federation.org/metadata/federation-name.xml",
				ChildEntities = {
					new EntityDescriptor(new EntityId("https://idp.example.org/idp/shibboleth")) {
						RoleDescriptors = {
							new IdpSsoDescriptor
							{
								ProtocolsSupported = {
									new Uri("urn:mace:shibboleth:1.0"),
									new Uri("urn:oasis:names:tc:SAML:2.0:protocol")
								},
								Extensions = {
									doc.SelectSingleNode(
										"/md:EntitiesDescriptor/md:EntityDescriptor[1]/md:IDPSSODescriptor/md:Extensions/*[1]",
										nsmgr).As<XmlElement>()
								},
								Keys = {
									new KeyDescriptor {
										KeyInfo = new DSigKeyInfo {
											Data = {
												new X509Data {
													Certificates = {
														new X509Certificate2(Convert.FromBase64String(certData))
													}
												}
											}
										}
									}
								},
								NameIdentifierFormats = {
									new NameIDFormat { Uri = new Uri("urn:mace:shibboleth:1.0:nameIdentifier") },
									new NameIDFormat { Uri = new Uri("urn:oasis:names:tc:SAML:2.0:nameid-format:transient") },
								},
								SingleSignOnServices = {
									new SingleSignOnService {
										Binding = new Uri("urn:mace:shibboleth:1.0:profiles:AuthnRequest"),
										Location = new Uri("https://idp.example.org/idp/profile/Shibboleth/SSO")
									},
									new SingleSignOnService {
										Binding = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST"),
										Location = new Uri("https://idp.example.org/idp/profile/SAML2/POST/SSO")
									},
									new SingleSignOnService {
										Binding = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect"),
										Location = new Uri("https://idp.example.org/idp/profile/SAML2/Redirect/SSO")
									}
								}
							},
							new AttributeAuthorityDescriptor() { 
								ProtocolsSupported = {
									new Uri("urn:oasis:names:tc:SAML:1.1:protocol"),
									new Uri("urn:oasis:names:tc:SAML:2.0:protocol")
								},
								Keys = {
									new KeyDescriptor {
										KeyInfo = new DSigKeyInfo {
											Data = {
												new X509Data {
													Certificates = {
														new X509Certificate2(Convert.FromBase64String(certData))
													}
												}
											}
										}
									}
								},
								AttributeServices = {
									new AttributeService {
										Binding = new Uri("urn:oasis:names:tc:SAML:1.0:bindings:SOAP-binding"),
										Location = new Uri("https://idp.example.org:8443/idp/profile/SAML1/SOAP/AttributeQuery")
									},
									new AttributeService {
										Binding = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:SOAP"),
										Location = new Uri("https://idp.example.org:8443/idp/profile/SAML2/SOAP/AttributeQuery")
									}
								},
								NameIDFormats = {
									new NameIDFormat {
										Uri = new Uri("urn:mace:shibboleth:1.0:nameIdentifier")
									},
									new NameIDFormat {
										Uri = new Uri("urn:oasis:names:tc:SAML:2.0:nameid-format:transient")
									}
								}
							}
						},
						Organization = new Organization {
							Names = {
								new LocalizedName("Your Identities", "en")
							},
							DisplayNames = {
								new LocalizedName("Your Identities", "en")
							},
							Urls = {
								new LocalizedUri(new Uri("http://www.example.org/"), "en")
							}
						},
						Contacts = {
							new ContactPerson {
								Type = ContactType.Technical,
								GivenName = "Your",
								Surname = "Contact",
								EmailAddresses = { "admin@example.org" }
							}
						}
					},
					new EntityDescriptor(new EntityId("https://sp.example.org/shibboleth-sp"))
					{
						RoleDescriptors = {
							new SpSsoDescriptor
							{
								ProtocolsSupported = {
									new Uri("urn:oasis:names:tc:SAML:2.0:protocol"),
									new Uri("urn:oasis:names:tc:SAML:1.1:protocol")
								},
								DiscoveryResponses = {
									{ 1,
										new DiscoveryResponse {
											Index = 1,
											Binding = new Uri("urn:oasis:names:tc:SAML:profiles:SSO:idp-discovery-protocol"),
											Location = new Uri("http://sp.example.org/Shibboleth.sso/DS"),
										}
									},
									{ 2,
										new DiscoveryResponse {
											Index = 2,
											Binding = new Uri("urn:oasis:names:tc:SAML:profiles:SSO:idp-discovery-protocol"),
											Location = new Uri("https://sp.example.org/Shibboleth.sso/DS")
										}
									}
								},
								Keys = {
									new KeyDescriptor {
										KeyInfo = new DSigKeyInfo {
											Data = {
												new X509Data {
													Certificates = {
														new X509Certificate2(Convert.FromBase64String(certData))
													}
												}
											}
										}
									}
								},
								NameIdentifierFormats = {
									new NameIDFormat {
										Uri = new Uri("urn:oasis:names:tc:SAML:2.0:nameid-format:transient")
									},
									new NameIDFormat {
										Uri = new Uri("urn:mace:shibboleth:1.0:nameIdentifier")
									}
								},
								AssertionConsumerServices = {
									{ 1,
										new AssertionConsumerService {
											Index = 1,
											IsDefault = true,
											Binding = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST"),
											Location = new Uri("https://sp.example.org/Shibboleth.sso/SAML2/POST")
										}
									}, { 2,
										new AssertionConsumerService {
											Index = 2,
											Binding = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST-SimpleSign"),
											Location = new Uri("https://sp.example.org/Shibboleth.sso/SAML2/POST-SimpleSign")
										}
									}, { 3,
										new AssertionConsumerService {
											Index = 3,
											Binding = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Artifact"),
											Location = new Uri("https://sp.example.org/Shibboleth.sso/SAML2/Artifact")
										}
									}, { 4,
										new AssertionConsumerService {
											Index = 4,
											Binding = new Uri("urn:oasis:names:tc:SAML:1.0:profiles:browser-post"),
											Location = new Uri("https://sp.example.org/Shibboleth.sso/SAML/POST")
										}
									}, { 5,
										new AssertionConsumerService {
											Index = 5,
											Binding = new Uri("urn:oasis:names:tc:SAML:1.0:profiles:artifact-01"),
											Location = new Uri("https://sp.example.org/Shibboleth.sso/SAML/Artifact")
										}
									}
								}
							}
						},
						Organization = new Organization {
							Names = {
								new LocalizedName("Your Service", "en")
							},
							DisplayNames = {
								new LocalizedName("Your Service", "en")
							},
							Urls = {
								new LocalizedUri(new Uri("http://sp.example.org"), "en")
							}
						},
						Contacts = {
							new ContactPerson {
								Type = ContactType.Technical,
								GivenName = "Your",
								Surname = "Admin",
								EmailAddresses = { "admin@example.org" }
							}
						}
					}
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadEntitiesDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadSaml2Attribute()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<saml:Attribute 
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion'
					Name='testAtt'
					NameFormat='http://idp.example.com/nameformat'
					FriendlyName='friendlyAtt'>
					<saml:AttributeValue>attValue</saml:AttributeValue>
				</saml:Attribute>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new Saml2Attribute("testAtt", "attValue")
			{
				NameFormat = new Uri("http://idp.example.com/nameformat"),
				FriendlyName = "friendlyAtt"
			};
				
			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadSaml2Attribute(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadKeyDescriptor()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<md:KeyDescriptor
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
					xmlns:xenc='http://www.w3.org/2001/04/xmlenc#'
					use='encryption'>
					<ds:KeyInfo>Any text, intermingled with:
						<ds:KeyName>string</ds:KeyName>
					</ds:KeyInfo>
					<md:EncryptionMethod Algorithm='http://www.example.com/'>Any text, intermingled with:
						<xenc:KeySize>1</xenc:KeySize>
						<xenc:OAEPparams>GpM7</xenc:OAEPparams>
						<!--any element-->
					</md:EncryptionMethod>
				</md:KeyDescriptor>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new KeyDescriptor
			{
				Use = KeyType.Encryption,
				KeyInfo = new DSigKeyInfo
				{
					KeyNames = { "string" }
				},
				EncryptionMethods = {
					new EncryptionMethod {
						Algorithm = new Uri("http://www.example.com/"),
						KeySize = 1,
						OAEPparams = Convert.FromBase64String("GpM7")
					}
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadKeyDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAffiliationDescriptor()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<md:AffiliationDescriptor 
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
					xmlns:xenc='http://www.w3.org/2001/04/xmlenc#'
					cacheDuration='P0Y3M2DT9H12M00S'
					validUntil='2019-02-02T15:16:17'
					affiliationOwnerID='mr owner'
					ID='yourGUIDhere'>
					<md:Extensions>
						<ext-elt/>
					</md:Extensions>
					<md:AffiliateMember>http://idp.example.org</md:AffiliateMember>
					<md:KeyDescriptor>
						<ds:KeyInfo>Any text, intermingled with:
							<ds:KeyName>string</ds:KeyName>
						</ds:KeyInfo>
						<md:EncryptionMethod Algorithm='http://www.example.com/'>Any text, intermingled with:
							<xenc:KeySize>1</xenc:KeySize>
							<xenc:OAEPparams>GpM7</xenc:OAEPparams>
							<!--any element-->
						</md:EncryptionMethod>
					</md:KeyDescriptor>
				</md:AffiliationDescriptor>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new AffiliationDescriptor
			{
				CacheDuration = new TimeSpan(3 * 30 + 2, 9, 12, 0),
				ValidUntil = new DateTime(2019, 02, 02, 15, 16, 17),
				AffiliationOwnerId = new EntityId("mr owner"),
				Id = "yourGUIDhere",
				AffiliateMembers = {
					new EntityId("http://idp.example.org")
				},
				Extensions = {
					doc.SelectSingleNode("/md:AffiliationDescriptor/md:Extensions/*[1]",
						nsmgr).As<XmlElement>()
				},
				KeyDescriptors = {
					new KeyDescriptor() {
						KeyInfo = new DSigKeyInfo {
							KeyNames = { "string" }
						},
						EncryptionMethods = {
							new EncryptionMethod {
								Algorithm = new Uri("http://www.example.com/"),
								KeySize = 1,
								OAEPparams = Convert.FromBase64String("GpM7")
							}
						}
					}
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadAffiliationDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_AdditionalMetadataLocation()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<md:AdditionalMetadataLocation
				xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
				namespace='http://oasis.org/saml-more#'>
				http://www.example.com/
			</md:AdditionalMetadataLocation>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new AdditionalMetadataLocation
			{
				Namespace = "http://oasis.org/saml-more#",
				Uri = new Uri("http://www.example.com/")
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadAdditionalMetadataLocation(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadPDPDescriptor()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<md:PDPDescriptor
					xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					xmlns:xenc='http://www.w3.org/2001/04/xmlenc#'
					ID='ID' 
					protocolSupportEnumeration='http://www.example.com/ http://www.example.com/'>
				   <ds:Signature>
					  <ds:SignedInfo>
						 <ds:CanonicalizationMethod Algorithm='http://www.example.com/'>Any text, intermingled with:
							<!--any element-->
						 </ds:CanonicalizationMethod>
						 <ds:SignatureMethod Algorithm='http://www.example.com/'>Any text, intermingled with:...
						 </ds:SignatureMethod>
						 <ds:Reference URI='http://www.example.com/'>...
						 </ds:Reference>
					  </ds:SignedInfo>
					  <ds:SignatureValue>GpM7</ds:SignatureValue>
					  <ds:KeyInfo>Any text, intermingled with:
						 <ds:KeyName>string</ds:KeyName>
					  </ds:KeyInfo>
					  <ds:Object>Any text, intermingled with:
						 <!--any element-->
					  </ds:Object>
				   </ds:Signature>
				   <md:Extensions>
					  <!--any element-->
				   </md:Extensions>
				   <md:KeyDescriptor>
					  <ds:KeyInfo>Any text, intermingled with:
						 <ds:KeyName>string</ds:KeyName>
					  </ds:KeyInfo>
					  <md:EncryptionMethod Algorithm='http://www.example.com/'>Any text, intermingled with:
						 <xenc:KeySize>1</xenc:KeySize>
						 <xenc:OAEPparams>GpM7</xenc:OAEPparams>
						 <!--any element-->
					  </md:EncryptionMethod>
				   </md:KeyDescriptor>
				   <md:Organization>
					  <md:Extensions>
						 <!--any element-->
					  </md:Extensions>
					  <md:OrganizationName xml:lang='en-US'>string</md:OrganizationName>
					  <md:OrganizationDisplayName xml:lang='en-US'>string</md:OrganizationDisplayName>
					  <md:OrganizationURL xml:lang='en-US'>http://www.example.com/</md:OrganizationURL>
				   </md:Organization>
				   <md:ContactPerson contactType='technical'>
					  <md:Extensions>
						 <!--any element-->
					  </md:Extensions>
					  <md:Company>string</md:Company>
					  <md:GivenName>string</md:GivenName>
					  <md:SurName>string</md:SurName>
					  <md:EmailAddress>http://www.example.com/</md:EmailAddress>
					  <md:TelephoneNumber>string</md:TelephoneNumber>
				   </md:ContactPerson>
				   <md:AuthzService Binding='http://www.example.com/' Location='http://www.example.com/'>
					  <!--any element-->
				   </md:AuthzService>
				   <md:AssertionIDRequestService Binding='http://www.example.com/' Location='http://www.example.com/'>
					  <!--any element-->
				   </md:AssertionIDRequestService>
				   <md:NameIDFormat>http://www.example.com/</md:NameIDFormat>
				</md:PDPDescriptor>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new PDPDescriptor {
				Id = "ID",
				ProtocolsSupported = {
					new Uri("http://www.example.com/"),
					new Uri("http://www.example.com/")
				},
				Extensions = { },
				Keys = {
					new KeyDescriptor() {
						KeyInfo = new DSigKeyInfo {
							KeyNames = { "string" },
						},
						EncryptionMethods = {
							new EncryptionMethod {
								Algorithm = new Uri("http://www.example.com/"),
								KeySize = 1,
								OAEPparams = Convert.FromBase64String("GpM7")
							}
						}
					}
				},
				Organization = new Organization {
					Extensions = { },
					Names = {
						new LocalizedName("string", "en-US")
					},
					DisplayNames = {
						new LocalizedName("string", "en-US")
					},
					Urls = {
						new LocalizedUri(new Uri("http://www.example.com/"), "en-US")
					}
				},
				Contacts = {
					new ContactPerson {
						Type = ContactType.Technical,
						Extensions = { },
						Company = "string",
						GivenName = "string",
						Surname = "string",
						EmailAddresses = { "http://www.example.com/" },
						TelephoneNumbers = { "string" }
					}
				},
				AuthzServices = {
					new AuthzService {
						Binding = new Uri("http://www.example.com/"),
						Location = new Uri("http://www.example.com/")
					}
				},
				AssertionIdRequestServices = {
					new AssertionIdRequestService {
						Binding = new Uri("http://www.example.com/"),
						Location = new Uri("http://www.example.com/")
					}
				},
				NameIDFormats = {
					new NameIDFormat
					{
						Uri = new Uri("http://www.example.com/"),
					}
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadPDPDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAuthnAuthorityDescriptor()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<md:AuthnAuthorityDescriptor 
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					xmlns:xenc='http://www.w3.org/2001/04/xmlenc#'
					xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
					ID='ID' protocolSupportEnumeration='http://www.example.com/ http://www.example.com/'>
				   <ds:Signature>
					  <ds:SignedInfo>
						 <ds:CanonicalizationMethod Algorithm='http://www.example.com/'>Any text, intermingled with:
							<!--any element-->
						 </ds:CanonicalizationMethod>
						 <ds:SignatureMethod Algorithm='http://www.example.com/'>Any text, intermingled with:...
						 </ds:SignatureMethod>
						 <ds:Reference URI='http://www.example.com/'>...
						 </ds:Reference>
					  </ds:SignedInfo>
					  <ds:SignatureValue>GpM7</ds:SignatureValue>
					  <ds:KeyInfo>Any text, intermingled with:
						 <ds:KeyName>string</ds:KeyName>
					  </ds:KeyInfo>
					  <ds:Object>Any text, intermingled with:
						 <!--any element-->
					  </ds:Object>
				   </ds:Signature>
				   <md:Extensions>
					  <!--any element-->
				   </md:Extensions>
				   <md:KeyDescriptor>
					  <ds:KeyInfo>Any text, intermingled with:
						 <ds:KeyName>string</ds:KeyName>
					  </ds:KeyInfo>
					  <md:EncryptionMethod Algorithm='http://www.example.com/'>Any text, intermingled with:
						 <xenc:KeySize>1</xenc:KeySize>
						 <xenc:OAEPparams>GpM7</xenc:OAEPparams>
						 <!--any element-->
					  </md:EncryptionMethod>
				   </md:KeyDescriptor>
				   <md:Organization>
					  <md:Extensions>
						 <!--any element-->
					  </md:Extensions>
					  <md:OrganizationName xml:lang='en-US'>string</md:OrganizationName>
					  <md:OrganizationDisplayName xml:lang='en-US'>string</md:OrganizationDisplayName>
					  <md:OrganizationURL xml:lang='en-US'>http://www.example.com/</md:OrganizationURL>
				   </md:Organization>
				   <md:ContactPerson contactType='technical'>
					  <md:Extensions>
						 <!--any element-->
					  </md:Extensions>
					  <md:Company>string</md:Company>
					  <md:GivenName>string</md:GivenName>
					  <md:SurName>string</md:SurName>
					  <md:EmailAddress>http://www.example.com/</md:EmailAddress>
					  <md:TelephoneNumber>string</md:TelephoneNumber>
				   </md:ContactPerson>
				   <md:AuthnQueryService Binding='http://www.example.com/' Location='http://www.example.com/'>
					  <!--any element-->
				   </md:AuthnQueryService>
				   <md:AssertionIDRequestService Binding='http://www.example.com/' Location='http://www.example.com/'>
					  <!--any element-->
				   </md:AssertionIDRequestService>
				   <md:NameIDFormat>http://www.example.com/</md:NameIDFormat>
				</md:AuthnAuthorityDescriptor>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new AuthnAuthorityDescriptor {
				Id = "ID",
				ProtocolsSupported = {
					new Uri("http://www.example.com/"),
					new Uri("http://www.example.com/")
				},
				Extensions = { },
				Keys = {
					new KeyDescriptor {
						KeyInfo = new DSigKeyInfo {
							KeyNames = { "string" }
						},
						EncryptionMethods = {
							new EncryptionMethod {
								Algorithm = new Uri("http://www.example.com/"),
								KeySize = 1,
								OAEPparams = Convert.FromBase64String("GpM7")
							}
						}
					}
				},
				Organization = new Organization {
					Extensions = { },
					Names = {
						new LocalizedName("string", "en-US")
					},
					DisplayNames = {
						new LocalizedName("string", "en-US")
					},
					Urls = {
						new LocalizedUri(new Uri("http://www.example.com/"), "en-US")
					}
				},
				Contacts = {
					new ContactPerson {
						Type = ContactType.Technical,
						Extensions = { },
						Company = "string",
						GivenName = "string",
						Surname = "string",
						EmailAddresses = { "http://www.example.com/" },
						TelephoneNumbers = { "string" }
					}
				},
				AuthnQueryServices = {
					new AuthnQueryService {
						Binding = new Uri("http://www.example.com/"),
						Location = new Uri("http://www.example.com/")
					}
				},
				AssertionIdRequestServices = {
					new AssertionIdRequestService {
						Binding = new Uri("http://www.example.com/"),
						Location = new Uri("http://www.example.com/")
					}
				},
				NameIDFormats = {
					new NameIDFormat {
						Uri = new Uri("http://www.example.com/")
					}
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadAuthnAuthorityDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAttributeAuthorityDescriptor()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<md:AttributeAuthorityDescriptor 
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					xmlns:xenc='http://www.w3.org/2001/04/xmlenc#'
					xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
					xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion'
					ID='ID'
					protocolSupportEnumeration='http://www.example.com/ http://www.example.com/'>
				   <ds:Signature>
					  <ds:SignedInfo>
						 <ds:CanonicalizationMethod Algorithm='http://www.example.com/'>Any text, intermingled with:
							<!--any element-->
						 </ds:CanonicalizationMethod>
						 <ds:SignatureMethod Algorithm='http://www.example.com/'>Any text, intermingled with:...
						 </ds:SignatureMethod>
						 <ds:Reference URI='http://www.example.com/'>...
						 </ds:Reference>
					  </ds:SignedInfo>
					  <ds:SignatureValue>GpM7</ds:SignatureValue>
					  <ds:KeyInfo>Any text, intermingled with:
						 <ds:KeyName>string</ds:KeyName>
					  </ds:KeyInfo>
					  <ds:Object>Any text, intermingled with:
						 <!--any element-->
					  </ds:Object>
				   </ds:Signature>
				   <md:Extensions>
					  <!--any element-->
				   </md:Extensions>
				   <md:KeyDescriptor>
					  <ds:KeyInfo>Any text, intermingled with:
						 <ds:KeyName>string</ds:KeyName>
					  </ds:KeyInfo>
					  <md:EncryptionMethod Algorithm='http://www.example.com/'>Any text, intermingled with:
						 <xenc:KeySize>1</xenc:KeySize>
						 <xenc:OAEPparams>GpM7</xenc:OAEPparams>
						 <!--any element-->
					  </md:EncryptionMethod>
				   </md:KeyDescriptor>
				   <md:Organization>
					  <md:Extensions>
						 <!--any element-->
					  </md:Extensions>
					  <md:OrganizationName xml:lang='en-US'>string</md:OrganizationName>
					  <md:OrganizationDisplayName xml:lang='en-US'>string</md:OrganizationDisplayName>
					  <md:OrganizationURL xml:lang='en-US'>http://www.example.com/</md:OrganizationURL>
				   </md:Organization>
				   <md:ContactPerson contactType='technical'>
					  <md:Extensions>
						 <!--any element-->
					  </md:Extensions>
					  <md:Company>string</md:Company>
					  <md:GivenName>string</md:GivenName>
					  <md:SurName>string</md:SurName>
					  <md:EmailAddress>http://www.example.com/</md:EmailAddress>
					  <md:TelephoneNumber>string</md:TelephoneNumber>
				   </md:ContactPerson>
				   <md:AttributeService Binding='http://www.example.com/' Location='http://www.example.com/'>
					  <!--any element-->
				   </md:AttributeService>
				   <md:AssertionIDRequestService Binding='http://www.example.com/' Location='http://www.example.com/'>
					  <!--any element-->
				   </md:AssertionIDRequestService>
				   <md:NameIDFormat>http://www.example.com/</md:NameIDFormat>
				   <md:AttributeProfile>http://www.example.com/</md:AttributeProfile>
				   <saml:Attribute Name='string'>
					  <saml:AttributeValue>any content</saml:AttributeValue>
				   </saml:Attribute>
				</md:AttributeAuthorityDescriptor>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new AttributeAuthorityDescriptor
			{
				Id = "ID",
				ProtocolsSupported = {
					new Uri("http://www.example.com/"),
					new Uri("http://www.example.com/")
				},
				Extensions = { },
				Keys = {
					new KeyDescriptor {
						KeyInfo = new DSigKeyInfo {
							KeyNames = { "string" }
						},
						EncryptionMethods = {
							new EncryptionMethod {
								Algorithm = new Uri("http://www.example.com/"),
								KeySize = 1,
								OAEPparams = Convert.FromBase64String("GpM7")
							}
						}
					}
				},
				Organization = new Organization
				{
					Extensions = { },
					Names = {
						new LocalizedName("string", "en-US")
					},
					DisplayNames = {
						new LocalizedName("string", "en-US")
					},
					Urls = {
						new LocalizedUri(new Uri("http://www.example.com/"), "en-US")
					}
				},
				Contacts = {
					new ContactPerson {
						Type = ContactType.Technical,
						Extensions = { },
						Company = "string",
						GivenName = "string",
						Surname = "string",
						EmailAddresses = { "http://www.example.com/" },
						TelephoneNumbers = { "string" }
					}
				},
				AttributeServices = {
					new AttributeService {
						Binding = new Uri("http://www.example.com/"),
						Location = new Uri("http://www.example.com/")
					}
				},
				AssertionIdRequestServices = {
					new AssertionIdRequestService {
						Binding = new Uri("http://www.example.com/"),
						Location = new Uri("http://www.example.com/")
					}
				},
				NameIDFormats = {
					new NameIDFormat {
						Uri = new Uri("http://www.example.com/")
					}
				},
				AttributeProfiles = {
					new AttributeProfile {
						Uri = new Uri("http://www.example.com/")
					}
				},
				Attributes = {
					new Saml2Attribute("string", "any content")
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadAttributeAuthorityDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadNameIDFormat()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<md:NameIDFormat
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'>
					http://www.example.org/
				</md:NameIDFormat>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new NameIDFormat
			{
				Uri = new Uri("http://www.example.org/"),
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadNameIDFormat(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEntityDescriptor()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<EntityDescriptor
				xmlns='urn:oasis:names:tc:SAML:2.0:metadata'
				xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
				xmlns:shibmd='urn:mace:shibboleth:metadata:1.0'
				xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'
				xmlns:xenc='http://www.w3.org/2001/04/xmlenc#'
				entityID='https://idp.example.org/idp/shibboleth'>
				<IDPSSODescriptor protocolSupportEnumeration='urn:mace:shibboleth:1.0 urn:oasis:names:tc:SAML:2.0:protocol'>
					<Extensions>
						<shibmd:Scope regexp='false'>example.org</shibmd:Scope>
					</Extensions>
					<KeyDescriptor>
						<ds:KeyInfo>
						<ds:X509Data>
							<ds:X509Certificate>" + certData + @"</ds:X509Certificate>
							</ds:X509Data>
						</ds:KeyInfo>
					</KeyDescriptor>
 
					<NameIDFormat>urn:mace:shibboleth:1.0:nameIdentifier</NameIDFormat>
					<NameIDFormat>urn:oasis:names:tc:SAML:2.0:nameid-format:transient</NameIDFormat>
 
					<SingleSignOnService Binding='urn:mace:shibboleth:1.0:profiles:AuthnRequest'
							Location='https://idp.example.org/idp/profile/Shibboleth/SSO' />
         
					<SingleSignOnService Binding='urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST'
							Location='https://idp.example.org/idp/profile/SAML2/POST/SSO' />
 
					<SingleSignOnService Binding='urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect'
							Location='https://idp.example.org/idp/profile/SAML2/Redirect/SSO' />
				</IDPSSODescriptor>
     
				<AttributeAuthorityDescriptor protocolSupportEnumeration='urn:oasis:names:tc:SAML:1.1:protocol urn:oasis:names:tc:SAML:2.0:protocol'>
					<KeyDescriptor>
						<ds:KeyInfo>
							<ds:X509Data>
								<ds:X509Certificate>" + certData + @"
								</ds:X509Certificate>
							</ds:X509Data>
						</ds:KeyInfo>
					</KeyDescriptor>
 
					<AttributeService Binding='urn:oasis:names:tc:SAML:1.0:bindings:SOAP-binding'
									Location='https://idp.example.org:8443/idp/profile/SAML1/SOAP/AttributeQuery' />
         
					<AttributeService Binding='urn:oasis:names:tc:SAML:2.0:bindings:SOAP'
									Location='https://idp.example.org:8443/idp/profile/SAML2/SOAP/AttributeQuery' />
         
					<NameIDFormat>urn:mace:shibboleth:1.0:nameIdentifier</NameIDFormat>
					<NameIDFormat>urn:oasis:names:tc:SAML:2.0:nameid-format:transient</NameIDFormat>
 
				</AttributeAuthorityDescriptor>
 
				<AuthnAuthorityDescriptor ID='ID' protocolSupportEnumeration='http://www.example.com/ http://www.example.com/'>
					<KeyDescriptor>
						<ds:KeyInfo>Any text, intermingled with:
							<ds:KeyName>string</ds:KeyName>
						</ds:KeyInfo>
						<EncryptionMethod Algorithm='http://www.example.com/'>Any text, intermingled with:
							<xenc:KeySize>1</xenc:KeySize>
							<xenc:OAEPparams>GpM7</xenc:OAEPparams>
							<!--any element-->
						</EncryptionMethod>
					</KeyDescriptor>
					<AuthnQueryService Binding='http://www.example.com/' Location='http://www.example.com/'>
						<!--any element-->
					</AuthnQueryService>
					<AssertionIDRequestService Binding='http://www.example.com/' Location='http://www.example.com/'>
						<!--any element-->
					</AssertionIDRequestService>
					<NameIDFormat>http://www.example.com/</NameIDFormat>
				</AuthnAuthorityDescriptor>

				<PDPDescriptor
					ID='ID' 
					protocolSupportEnumeration='http://www.example.com/ http://www.example.com/'>
				   <ds:Signature>
					  <ds:SignedInfo>
						 <ds:CanonicalizationMethod Algorithm='http://www.example.com/'>Any text, intermingled with:
							<!--any element-->
						 </ds:CanonicalizationMethod>
						 <ds:SignatureMethod Algorithm='http://www.example.com/'>Any text, intermingled with:...
						 </ds:SignatureMethod>
						 <ds:Reference URI='http://www.example.com/'>...
						 </ds:Reference>
					  </ds:SignedInfo>
					  <ds:SignatureValue>GpM7</ds:SignatureValue>
					  <ds:KeyInfo>Any text, intermingled with:
						 <ds:KeyName>string</ds:KeyName>
					  </ds:KeyInfo>
					  <ds:Object>Any text, intermingled with:
						 <!--any element-->
					  </ds:Object>
				   </ds:Signature>
				   <Extensions>
					  <!--any element-->
				   </Extensions>
				   <KeyDescriptor>
					  <ds:KeyInfo>Any text, intermingled with:
						 <ds:KeyName>string</ds:KeyName>
					  </ds:KeyInfo>
					  <EncryptionMethod Algorithm='http://www.example.com/'>Any text, intermingled with:
						 <xenc:KeySize>1</xenc:KeySize>
						 <xenc:OAEPparams>GpM7</xenc:OAEPparams>
						 <!--any element-->
					  </EncryptionMethod>
				   </KeyDescriptor>
				   <AuthzService Binding='http://www.example.com/' Location='http://www.example.com/'>
					  <!--any element-->
				   </AuthzService>
				   <AssertionIDRequestService Binding='http://www.example.com/' Location='http://www.example.com/'>
					  <!--any element-->
				   </AssertionIDRequestService>
				   <NameIDFormat>http://www.example.com/</NameIDFormat>
				</PDPDescriptor>

				<Organization>
						<OrganizationName xml:lang='en'>Your Identities</OrganizationName>
						<OrganizationDisplayName xml:lang='en'>Your Identities</OrganizationDisplayName>
						<OrganizationURL xml:lang='en'>http://www.example.org/</OrganizationURL>
				</Organization>
				<ContactPerson contactType='technical'>
						<GivenName>Your</GivenName>
						<SurName>Contact</SurName>
						<EmailAddress>admin@example.org</EmailAddress>
				</ContactPerson>
			</EntityDescriptor>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new EntityDescriptor(new EntityId("https://idp.example.org/idp/shibboleth")) {
				RoleDescriptors = {
					new IdpSsoDescriptor
					{
						ProtocolsSupported = {
							new Uri("urn:mace:shibboleth:1.0"),
							new Uri("urn:oasis:names:tc:SAML:2.0:protocol")
						},
						Extensions = {
							doc.SelectSingleNode(
								"md:EntityDescriptor[1]/md:IDPSSODescriptor/md:Extensions/*[1]",
								nsmgr).As<XmlElement>()
						},
						Keys = {
							new KeyDescriptor {
								KeyInfo = new DSigKeyInfo {
									Data = {
										new X509Data {
											Certificates = {
												new X509Certificate2(Convert.FromBase64String(certData))
											}
										}
									}
								}
							}
						},
						NameIdentifierFormats = {
							new NameIDFormat { Uri = new Uri("urn:mace:shibboleth:1.0:nameIdentifier") },
							new NameIDFormat { Uri = new Uri("urn:oasis:names:tc:SAML:2.0:nameid-format:transient") },
						},
						SingleSignOnServices = {
							new SingleSignOnService {
								Binding = new Uri("urn:mace:shibboleth:1.0:profiles:AuthnRequest"),
								Location = new Uri("https://idp.example.org/idp/profile/Shibboleth/SSO")
							},
							new SingleSignOnService {
								Binding = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST"),
								Location = new Uri("https://idp.example.org/idp/profile/SAML2/POST/SSO")
							},
							new SingleSignOnService {
								Binding = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect"),
								Location = new Uri("https://idp.example.org/idp/profile/SAML2/Redirect/SSO")
							}
						}
					},
					new AttributeAuthorityDescriptor() {
						ProtocolsSupported = {
							new Uri("urn:oasis:names:tc:SAML:1.1:protocol"),
							new Uri("urn:oasis:names:tc:SAML:2.0:protocol")
						},
						Keys = {
							new KeyDescriptor {
								KeyInfo = new DSigKeyInfo {
									Data = {
										new X509Data {
											Certificates = {
												new X509Certificate2(Convert.FromBase64String(certData))
											}
										}
									}
								}
							}
						},
						AttributeServices = {
							new AttributeService {
								Binding = new Uri("urn:oasis:names:tc:SAML:1.0:bindings:SOAP-binding"),
								Location = new Uri("https://idp.example.org:8443/idp/profile/SAML1/SOAP/AttributeQuery")
							},
							new AttributeService {
								Binding = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:SOAP"),
								Location = new Uri("https://idp.example.org:8443/idp/profile/SAML2/SOAP/AttributeQuery")
							}
						},
						NameIDFormats = {
							new NameIDFormat {
								Uri = new Uri("urn:mace:shibboleth:1.0:nameIdentifier")
							},
							new NameIDFormat {
								Uri = new Uri("urn:oasis:names:tc:SAML:2.0:nameid-format:transient")
							}
						}
					},
					new AuthnAuthorityDescriptor {
						Id = "ID",
						ProtocolsSupported = {
							new Uri("http://www.example.com/"),
							new Uri("http://www.example.com/")
						},
						Extensions = { },
						Keys = {
							new KeyDescriptor {
								KeyInfo = new DSigKeyInfo {
									KeyNames = { "string" }
								},
								EncryptionMethods = {
									new EncryptionMethod {
										Algorithm = new Uri("http://www.example.com/"),
										KeySize = 1,
										OAEPparams = Convert.FromBase64String("GpM7")
									}
								}
							}
						},
						AuthnQueryServices = {
							new AuthnQueryService {
								Binding = new Uri("http://www.example.com/"),
								Location = new Uri("http://www.example.com/")
							}
						},
						AssertionIdRequestServices = {
							new AssertionIdRequestService {
								Binding = new Uri("http://www.example.com/"),
								Location = new Uri("http://www.example.com/")
							}
						},
						NameIDFormats = {
							new NameIDFormat {
								Uri = new Uri("http://www.example.com/")
							}
						}
					},
					new PDPDescriptor {
						Id = "ID",
						ProtocolsSupported = {
							new Uri("http://www.example.com/"),
							new Uri("http://www.example.com/")
						},
						Extensions = { },
						Keys = {
							new KeyDescriptor() {
								KeyInfo = new DSigKeyInfo {
									KeyNames = { "string" },
								},
								EncryptionMethods = {
									new EncryptionMethod {
										Algorithm = new Uri("http://www.example.com/"),
										KeySize = 1,
										OAEPparams = Convert.FromBase64String("GpM7")
									}
								}
							}
						},
						AuthzServices = {
							new AuthzService {
								Binding = new Uri("http://www.example.com/"),
								Location = new Uri("http://www.example.com/")
							}
						},
						AssertionIdRequestServices = {
							new AssertionIdRequestService {
								Binding = new Uri("http://www.example.com/"),
								Location = new Uri("http://www.example.com/")
							}
						},
						NameIDFormats = {
							new NameIDFormat
							{
								Uri = new Uri("http://www.example.com/"),
							}
						}
					}
				},
				Organization = new Organization {
					Names = {
						new LocalizedName("Your Identities", "en")
					},
					DisplayNames = {
						new LocalizedName("Your Identities", "en")
					},
					Urls = {
						new LocalizedUri(new Uri("http://www.example.org/"), "en")
					}
				},
				Contacts = {
					new ContactPerson {
						Type = ContactType.Technical,
						GivenName = "Your",
						Surname = "Contact",
						EmailAddresses = { "admin@example.org" }
					}
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadEntityDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadLocalizedName()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<LocalName xml:lang='en'>NameValue</LocalName>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new LocalizedName("NameValue", "en");
			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadLocalizedName(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadLocalizedUri()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<LocalUri xml:lang='en'>http://www.foo.org/</LocalUri>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new LocalizedUri(new Uri("http://www.foo.org/"), "en");
			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadLocalizedUri(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadOrganization()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<Organization xmlns='urn:oasis:names:tc:SAML:2.0:metadata'>
					<Extensions>
						<ext-elt/>
					</Extensions>
					<OrganizationName xml:lang='en'>Acme Ltd</OrganizationName>
					<OrganizationDisplayName xml:lang='en'>Acme Ltd (display)</OrganizationDisplayName>
					<OrganizationURL xml:lang='en'>http://acme.co/</OrganizationURL>
				</Organization>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new Organization
			{
				Extensions = {
					doc.SelectSingleNode("/md:Organization/md:Extensions/*[1]",
						nsmgr).As<XmlElement>()
				},
				Names = {
					new LocalizedName("Acme Ltd", "en")
				},
				DisplayNames = {
					new LocalizedName("Acme Ltd (display)", "en")
				},
				Urls = {
					new LocalizedUri(new Uri("http://acme.co/"), "en")
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadOrganization(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadMetadata()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<EntitiesDescriptor Name='https://your-federation.org/metadata/federation-name.xml'
				xmlns='urn:oasis:names:tc:SAML:2.0:metadata'
				xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
				xmlns:shibmd='urn:mace:shibboleth:metadata:1.0'
				xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
 
				<EntityDescriptor entityID='https://idp.example.org/idp/shibboleth'>
				   <IDPSSODescriptor protocolSupportEnumeration='urn:mace:shibboleth:1.0 urn:oasis:names:tc:SAML:2.0:protocol'>
					   <SingleSignOnService Binding='urn:mace:shibboleth:1.0:profiles:AuthnRequest'
								Location='https://idp.example.org/idp/profile/Shibboleth/SSO' />
				   </IDPSSODescriptor>
				</EntityDescriptor>
				<EntityDescriptor entityID='https://sp.example.org/shibboleth-sp'>
					<SPSSODescriptor protocolSupportEnumeration='urn:oasis:names:tc:SAML:2.0:protocol urn:oasis:names:tc:SAML:1.1:protocol'>
						<Extensions>
							<idpdisc:DiscoveryResponse xmlns:idpdisc='urn:oasis:names:tc:SAML:profiles:SSO:idp-discovery-protocol'
									index='1' Binding='urn:oasis:names:tc:SAML:profiles:SSO:idp-discovery-protocol'
									Location='http://sp.example.org/Shibboleth.sso/DS'/>
						</Extensions>
					</SPSSODescriptor>
				</EntityDescriptor>
			</EntitiesDescriptor>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new EntitiesDescriptor
			{
				Name = "https://your-federation.org/metadata/federation-name.xml",
				ChildEntities = {
					new EntityDescriptor(new EntityId("https://idp.example.org/idp/shibboleth")) {
						RoleDescriptors = {
							new IdpSsoDescriptor
							{
								ProtocolsSupported = {
									new Uri("urn:mace:shibboleth:1.0"),
									new Uri("urn:oasis:names:tc:SAML:2.0:protocol")
								},
								SingleSignOnServices = {
									new SingleSignOnService {
										Binding = new Uri("urn:mace:shibboleth:1.0:profiles:AuthnRequest"),
										Location = new Uri("https://idp.example.org/idp/profile/Shibboleth/SSO")
									}
								}
							}
						}
					},
					new EntityDescriptor(new EntityId("https://sp.example.org/shibboleth-sp"))
					{
						RoleDescriptors = {
							new SpSsoDescriptor
							{
								ProtocolsSupported = {
									new Uri("urn:oasis:names:tc:SAML:2.0:protocol"),
									new Uri("urn:oasis:names:tc:SAML:1.1:protocol")
								},
								DiscoveryResponses = {
									{ 1,
										new DiscoveryResponse {
											Index = 1,
											Binding = new Uri("urn:oasis:names:tc:SAML:profiles:SSO:idp-discovery-protocol"),
											Location = new Uri("http://sp.example.org/Shibboleth.sso/DS"),
										}
									}
								}
							}
						}
					}
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
			{
				using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
				{
					return serializer.ReadMetadata(ms);
				}
			});
			ReadTest(xml, expected, (serializer, reader) =>
			{
				using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
				using (var xr = XmlReader.Create(ms))
				{
					return serializer.ReadMetadata(xr);
				}
			});
			ReadTest(xml, expected, (serializer, readeR) =>
			{
				using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
				using (var xr = XmlReader.Create(ms))
				{
					return serializer.ReadMetadata(xr, NullSecurityTokenResolver.Instance);
				}
			});
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadSecurityTokenServiceDescriptor()
		{
			string xml =
				@"<?xml version='1.0' encoding='UTF-8' ?>
				<RoleDescriptor 
					xsi:type='fed:SecurityTokenServiceType'
					protocolSupportEnumeration='http://docs.oasis-open.org/ws-sx/ws-trust/200512 http://schemas.xmlsoap.org/ws/2005/02/trust http://docs.oasis-open.org/wsfed/federation/200706'
					ServiceDisplayName='www.netiq.com' 
					ServiceDescription='test'
					xmlns='urn:oasis:names:tc:SAML:2.0:metadata'
					xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' 
					xmlns:fed='http://docs.oasis-open.org/wsfed/federation/200706'
					xmlns:wsa='http://www.w3.org/2005/08/addressing'>
					<KeyDescriptor use='signing'>
						<KeyInfo 
							xmlns='http://www.w3.org/2000/09/xmldsig#'>
							<X509Data>
								<X509Certificate>" + certData + @"</X509Certificate>
							</X509Data>
						</KeyInfo>
					</KeyDescriptor>
					<fed:TokenTypesOffered>
						<fed:TokenType Uri='urn:oasis:names:tc:SAML:2.0:assertion'/>
						<fed:TokenType Uri='urn:oasis:names:tc:SAML:1.0:assertion'/>
					</fed:TokenTypesOffered>
					<fed:ClaimTypesOffered>
						<auth:ClaimType Uri='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>E-Mail Address</auth:DisplayName>
							<auth:Description>The e-mail address of the user</auth:Description>
						</auth:ClaimType>
						<auth:ClaimType Uri='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>Given Name</auth:DisplayName>
							<auth:Description>The given name of the user</auth:Description>
						</auth:ClaimType>
						<auth:ClaimType Uri='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>Name</auth:DisplayName>
							<auth:Description>The unique name of the user</auth:Description>
						</auth:ClaimType>
						<auth:ClaimType Uri='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>UPN</auth:DisplayName>
							<auth:Description>The user principal name (UPN) of the user</auth:Description>
						</auth:ClaimType>
						<auth:ClaimType Uri='http://schemas.xmlsoap.org/claims/CommonName' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>Common Name</auth:DisplayName>
							<auth:Description>The common name of the user</auth:Description>
						</auth:ClaimType>
						<auth:ClaimType Uri='http://schemas.xmlsoap.org/claims/EmailAddress' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>AD FS 1.x E-Mail Address</auth:DisplayName>
							<auth:Description>The e-mail address of the user when interoperating with AD FS 1.1 or ADFS 1.0</auth:Description>
						</auth:ClaimType>
						<auth:ClaimType Uri='http://schemas.xmlsoap.org/claims/Group' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>Group</auth:DisplayName>
							<auth:Description>A group that the user is a member of</auth:Description>
						</auth:ClaimType>
						<auth:ClaimType Uri='http://schemas.xmlsoap.org/claims/UPN' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>AD FS 1.x UPN</auth:DisplayName>
							<auth:Description>The UPN of the user when interoperating with AD FS 1.1 or ADFS 1.0</auth:Description>
						</auth:ClaimType>
						<auth:ClaimType Uri='http://schemas.microsoft.com/ws/2008/06/identity/claims/role' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>Role</auth:DisplayName>
							<auth:Description>A role that the user has</auth:Description>
						</auth:ClaimType>
						<auth:ClaimType Uri='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>Surname</auth:DisplayName>
							<auth:Description>The surname of the user</auth:Description>
						</auth:ClaimType>
						<auth:ClaimType Uri='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/privatepersonalidentifier' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>PPID</auth:DisplayName>
							<auth:Description>The private identifier of the user</auth:Description>
						</auth:ClaimType>
						<auth:ClaimType Uri='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>Name ID</auth:DisplayName>
							<auth:Description>The SAML name identifier of the user</auth:Description>
						</auth:ClaimType>
						<auth:ClaimType Uri='http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationinstant' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>Authentication time stamp</auth:DisplayName>
							<auth:Description>Used to display the time and date that the user was authenticated</auth:Description>
						</auth:ClaimType>
						<auth:ClaimType Uri='http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>Authentication method</auth:DisplayName>
							<auth:Description>The method used to authenticate the user</auth:Description>
						</auth:ClaimType>
						<auth:ClaimType Uri='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/denyonlysid' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>Deny only group SID</auth:DisplayName>
							<auth:Description>The deny-only group SID of the user</auth:Description>
						</auth:ClaimType>
						<auth:ClaimType Uri='http://schemas.microsoft.com/ws/2008/06/identity/claims/denyonlyprimarysid' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>Deny only primary SID</auth:DisplayName>
							<auth:Description>The deny-only primary SID of the user</auth:Description>
						</auth:ClaimType>
						<auth:ClaimType Uri='http://schemas.microsoft.com/ws/2008/06/identity/claims/denyonlyprimarygroupsid' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>Deny only primary group SID</auth:DisplayName>
							<auth:Description>The deny-only primary group SID of the user</auth:Description>
						</auth:ClaimType>
						<auth:ClaimType Uri='http://schemas.microsoft.com/ws/2008/06/identity/claims/groupsid' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>Group SID</auth:DisplayName>
							<auth:Description>The group SID of the user</auth:Description>
						</auth:ClaimType>
						<auth:ClaimType Uri='http://schemas.microsoft.com/ws/2008/06/identity/claims/primarygroupsid' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>Primary group SID</auth:DisplayName>
							<auth:Description>The primary group SID of the user</auth:Description>
						</auth:ClaimType>
						<auth:ClaimType Uri='http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>Primary SID</auth:DisplayName>
							<auth:Description>The primary SID of the user</auth:Description>
						</auth:ClaimType>
						<auth:ClaimType Uri='http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsaccountname' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>Windows account name</auth:DisplayName>
							<auth:Description>The domain account name of the user in the form of &lt;domain&gt;\&lt;user&gt;</auth:Description>
						</auth:ClaimType>
					</fed:ClaimTypesOffered>
					<fed:SecurityTokenServiceEndpoint>
						<wsa:EndpointReference>
							<wsa:Address>https://www.netiq.com/nidp/wsfed/ep</wsa:Address>
						</wsa:EndpointReference>
					</fed:SecurityTokenServiceEndpoint>
					<fed:PassiveRequestorEndpoint>
						<wsa:EndpointReference>
							<wsa:Address>https://www.netiq.com/nidp/wsfed/ep</wsa:Address>
						</wsa:EndpointReference>
					</fed:PassiveRequestorEndpoint>
					<fed:SingleSignOutSubscriptionEndpoint>
						<wsa:EndpointReference>
							<wsa:Address>https://www.netiq.com/nidp/wsfed/ep-ssos</wsa:Address>
						</wsa:EndpointReference>
					</fed:SingleSignOutSubscriptionEndpoint>
					<fed:SingleSignOutNotificationEndpoint>
						<wsa:EndpointReference>
							<wsa:Address>https://www.netiq.com/nidp/wsfed/ep-sson</wsa:Address>
						</wsa:EndpointReference>
					</fed:SingleSignOutNotificationEndpoint>
				</RoleDescriptor>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new SecurityTokenServiceDescriptor
			{
				ProtocolsSupported = {
					new Uri("http://docs.oasis-open.org/ws-sx/ws-trust/200512"),
					new Uri("http://schemas.xmlsoap.org/ws/2005/02/trust"),
					new Uri("http://docs.oasis-open.org/wsfed/federation/200706"),
				},
				ServiceDisplayName = "www.netiq.com",
				ServiceDescription = "test",
				Keys = {
					new KeyDescriptor {
						Use = KeyType.Signing,
						KeyInfo = new DSigKeyInfo {
							Data = {
								new X509Data {
									Certificates = {
										new X509Certificate2(Convert.FromBase64String(certData))
									}
								}
							}
						}
					}
				},
				TokenTypesOffered = {
					new Uri("urn:oasis:names:tc:SAML:2.0:assertion"),
					new Uri("urn:oasis:names:tc:SAML:1.0:assertion")
				},
				ClaimTypesOffered = {
					new DisplayClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
					{
						DisplayName = "E-Mail Address",
						Description = "The e-mail address of the user"
					},
					new DisplayClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")
					{
						DisplayName = "Given Name",
						Description = "The given name of the user"
					},
					new DisplayClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
					{
						DisplayName = "Name",
						Description = "The unique name of the user"
					},
					new DisplayClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")
					{
						DisplayName = "UPN",
						Description = "The user principal name (UPN) of the user"
					},
					new DisplayClaim("http://schemas.xmlsoap.org/claims/CommonName")
					{
						DisplayName = "Common Name",
						Description = "The common name of the user"
					},
					new DisplayClaim("http://schemas.xmlsoap.org/claims/EmailAddress")
					{
						DisplayName = "AD FS 1.x E-Mail Address",
						Description = "The e-mail address of the user when interoperating with AD FS 1.1 or ADFS 1.0"
					},
					new DisplayClaim("http://schemas.xmlsoap.org/claims/Group")
					{
						DisplayName = "Group",
						Description = "A group that the user is a member of"
					},
					new DisplayClaim("http://schemas.xmlsoap.org/claims/UPN")
					{
						DisplayName = "AD FS 1.x UPN",
						Description = "The UPN of the user when interoperating with AD FS 1.1 or ADFS 1.0"
					},
					new DisplayClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
					{
						DisplayName = "Role",
						Description = "A role that the user has"
					},
					new DisplayClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname")
					{
						DisplayName = "Surname",
						Description = "The surname of the user"
					},
					new DisplayClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/privatepersonalidentifier")
					{
						DisplayName = "PPID",
						Description = "The private identifier of the user"
					},
					new DisplayClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
					{
						DisplayName = "Name ID",
						Description = "The SAML name identifier of the user"
					},
					new DisplayClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationinstant")
					{
						DisplayName = "Authentication time stamp",
						Description = "Used to display the time and date that the user was authenticated"
					},
					new DisplayClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod")
					{
						DisplayName = "Authentication method",
						Description = "The method used to authenticate the user"
					},
					new DisplayClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/denyonlysid")
					{
						DisplayName = "Deny only group SID",
						Description = "The deny-only group SID of the user"
					},
					new DisplayClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/denyonlyprimarysid")
					{
						DisplayName = "Deny only primary SID",
						Description = "The deny-only primary SID of the user"
					},
					new DisplayClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/denyonlyprimarygroupsid")
					{
						DisplayName = "Deny only primary group SID",
						Description = "The deny-only primary group SID of the user"
					},
					new DisplayClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/groupsid")
					{
						DisplayName = "Group SID",
						Description = "The group SID of the user"
					},
					new DisplayClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/primarygroupsid")
					{
						DisplayName = "Primary group SID",
						Description = "The primary group SID of the user"
					},
					new DisplayClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid")
					{
						DisplayName = "Primary SID",
						Description = "The primary SID of the user"
					},
					new DisplayClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsaccountname")
					{
						DisplayName = "Windows account name",
						Description = "The domain account name of the user in the form of <domain>\\<user>"
					}
				},
				SecurityTokenServiceEndpoints = {
					new EndpointReference("https://www.netiq.com/nidp/wsfed/ep")
				},
				PassiveRequestorEndpoints = {
					new EndpointReference("https://www.netiq.com/nidp/wsfed/ep")
				},
				SingleSignOutNotificationEndpoints = {
					new EndpointReference("https://www.netiq.com/nidp/wsfed/ep-sson")
				},
				SingleSignOutSubscriptionEndpoints = {
					new EndpointReference("https://www.netiq.com/nidp/wsfed/ep-ssos")
				},
			};
			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadSecurityTokenServiceDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAttributeConsumingService()
		{
			string xml =
				@"<?xml version='1.0' encoding='UTF-8' ?>
				<md:AttributeConsumingService index='1'
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'>
					<md:ServiceName xml:lang='en'>Sample Service</md:ServiceName>
					<md:ServiceDescription xml:lang='en'>An example service that requires a human-readable identifier and optional name and e-mail address.</md:ServiceDescription>
 
					<md:RequestedAttribute FriendlyName='eduPersonPrincipalName' Name='urn:mace:dir:attribute-def:eduPersonPrincipalName' NameFormat='urn:mace:shibboleth:1.0:attributeNamespace:uri'/>
					<md:RequestedAttribute FriendlyName='mail' Name='urn:mace:dir:attribute-def:mail' NameFormat='urn:mace:shibboleth:1.0:attributeNamespace:uri'/>
					<md:RequestedAttribute FriendlyName='displayName' Name='urn:mace:dir:attribute-def:displayName' NameFormat='urn:mace:shibboleth:1.0:attributeNamespace:uri'/>
 
					<md:RequestedAttribute FriendlyName='eduPersonPrincipalName' Name='urn:oid:1.3.6.1.4.1.5923.1.1.1.6' NameFormat='urn:oasis:names:tc:SAML:2.0:attrname-format:uri'/>
					<md:RequestedAttribute FriendlyName='mail' Name='urn:oid:0.9.2342.19200300.100.1.3' NameFormat='urn:oasis:names:tc:SAML:2.0:attrname-format:uri'/>
					<md:RequestedAttribute FriendlyName='displayName' Name='urn:oid:2.16.840.1.113730.3.1.241' NameFormat='urn:oasis:names:tc:SAML:2.0:attrname-format:uri'/>
				</md:AttributeConsumingService>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new AttributeConsumingService
			{
				Index = 1,

				ServiceNames = {
					new LocalizedName("Sample Service", "en")
				},

				ServiceDescriptions = {
					new LocalizedName("An example service that requires a human-readable identifier and optional name and e-mail address.", "en")
				},
 
				RequestedAttributes = {
					new RequestedAttribute("urn:mace:dir:attribute-def:eduPersonPrincipalName") {
						FriendlyName = "eduPersonPrincipalName",
						NameFormat = new Uri("urn:mace:shibboleth:1.0:attributeNamespace:uri")
					},
					new RequestedAttribute("urn:mace:dir:attribute-def:mail") {
						FriendlyName = "mail",
						NameFormat = new Uri("urn:mace:shibboleth:1.0:attributeNamespace:uri")
					},
					new RequestedAttribute("urn:mace:dir:attribute-def:displayName") {
						FriendlyName = "displayName",
						NameFormat = new Uri("urn:mace:shibboleth:1.0:attributeNamespace:uri")
					},
					new RequestedAttribute("urn:oid:1.3.6.1.4.1.5923.1.1.1.6") {
						FriendlyName = "eduPersonPrincipalName",
						NameFormat = new Uri("urn:oasis:names:tc:SAML:2.0:attrname-format:uri")
					},
					new RequestedAttribute("urn:oid:0.9.2342.19200300.100.1.3") {
						FriendlyName = "mail",
						NameFormat = new Uri("urn:oasis:names:tc:SAML:2.0:attrname-format:uri")
					},
					new RequestedAttribute("urn:oid:2.16.840.1.113730.3.1.241") {
						FriendlyName = "displayName",
						NameFormat = new Uri("urn:oasis:names:tc:SAML:2.0:attrname-format:uri")
					}
				}
			};
			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadAttributeConsumingService(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadRequestedAttribute()
		{
			string xml =
				@"<?xml version='1.0' encoding='UTF-8' ?>
				<md:RequestedAttribute 
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					isRequired='true'
					FriendlyName='eduPersonPrincipalName' 
					Name='urn:mace:dir:attribute-def:eduPersonPrincipalName'
					NameFormat='urn:mace:shibboleth:1.0:attributeNamespace:uri'>
				</md:RequestedAttribute>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new RequestedAttribute("urn:mace:dir:attribute-def:eduPersonPrincipalName") {
				IsRequired = true,
				FriendlyName = "eduPersonPrincipalName",
				NameFormat = new Uri("urn:mace:shibboleth:1.0:attributeNamespace:uri")
			};
			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadRequestedAttribute(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadSpSsoDescriptor()
		{
			string xml =
				@"<?xml version='1.0' encoding='UTF-8' ?>
				<SPSSODescriptor
					xmlns='urn:oasis:names:tc:SAML:2.0:metadata'
					xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
					protocolSupportEnumeration='urn:oasis:names:tc:SAML:2.0:protocol urn:oasis:names:tc:SAML:1.1:protocol'
					AuthnRequestsSigned='true'
					WantAssertionsSigned='false'>
					<Extensions>
						<idpdisc:DiscoveryResponse xmlns:idpdisc='urn:oasis:names:tc:SAML:profiles:SSO:idp-discovery-protocol'
								index='1' Binding='urn:oasis:names:tc:SAML:profiles:SSO:idp-discovery-protocol'
								Location='http://sp.example.org/Shibboleth.sso/DS'/>
						<idpdisc:DiscoveryResponse xmlns:idpdisc='urn:oasis:names:tc:SAML:profiles:SSO:idp-discovery-protocol'
								index='2' Binding='urn:oasis:names:tc:SAML:profiles:SSO:idp-discovery-protocol'
								Location='https://sp.example.org/Shibboleth.sso/DS'/>
					</Extensions>
 
					<KeyDescriptor>
						<ds:KeyInfo>
							<ds:X509Data>
								<ds:X509Certificate>" + certData + @"
								</ds:X509Certificate>
							</ds:X509Data>
						</ds:KeyInfo>
					</KeyDescriptor>
 
					<NameIDFormat>urn:oasis:names:tc:SAML:2.0:nameid-format:transient</NameIDFormat>
					<NameIDFormat>urn:mace:shibboleth:1.0:nameIdentifier</NameIDFormat>
 
					<AssertionConsumerService index='1' isDefault='true'
							Binding='urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST'
							Location='https://sp.example.org/Shibboleth.sso/SAML2/POST'/>
					<AssertionConsumerService index='2'
							Binding='urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST-SimpleSign'
							Location='https://sp.example.org/Shibboleth.sso/SAML2/POST-SimpleSign'/>
					<AssertionConsumerService index='3'
							Binding='urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Artifact'
							Location='https://sp.example.org/Shibboleth.sso/SAML2/Artifact'/>
					<AssertionConsumerService index='4'
							Binding='urn:oasis:names:tc:SAML:1.0:profiles:browser-post'
							Location='https://sp.example.org/Shibboleth.sso/SAML/POST'/>
					<AssertionConsumerService index='5'
							Binding='urn:oasis:names:tc:SAML:1.0:profiles:artifact-01'
							Location='https://sp.example.org/Shibboleth.sso/SAML/Artifact'/>
 
				</SPSSODescriptor>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var expected = new SpSsoDescriptor
			{
				AuthnRequestsSigned = true,
				WantAssertionsSigned = false,
				ProtocolsSupported = {
					new Uri("urn:oasis:names:tc:SAML:2.0:protocol"),
					new Uri("urn:oasis:names:tc:SAML:1.1:protocol")
				},
				DiscoveryResponses = {
					{ 1,
						new DiscoveryResponse {
							Index = 1,
							Binding = new Uri("urn:oasis:names:tc:SAML:profiles:SSO:idp-discovery-protocol"),
							Location = new Uri("http://sp.example.org/Shibboleth.sso/DS"),
						}
					},
					{ 2,
						new DiscoveryResponse {
							Index = 2,
							Binding = new Uri("urn:oasis:names:tc:SAML:profiles:SSO:idp-discovery-protocol"),
							Location = new Uri("https://sp.example.org/Shibboleth.sso/DS")
						}
					}
				},
				Keys = {
					new KeyDescriptor {
						KeyInfo = new DSigKeyInfo {
							Data = {
								new X509Data {
									Certificates = {
										new X509Certificate2(Convert.FromBase64String(certData))
									}
								}
							}
						}
					}
				},
				NameIdentifierFormats = {
					new NameIDFormat {
						Uri = new Uri("urn:oasis:names:tc:SAML:2.0:nameid-format:transient")
					},
					new NameIDFormat {
						Uri = new Uri("urn:mace:shibboleth:1.0:nameIdentifier")
					}
				},
				AssertionConsumerServices = {
					{ 1,
						new AssertionConsumerService {
							Index = 1,
							IsDefault = true,
							Binding = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST"),
							Location = new Uri("https://sp.example.org/Shibboleth.sso/SAML2/POST")
						}
					}, { 2,
						new AssertionConsumerService {
							Index = 2,
							Binding = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST-SimpleSign"),
							Location = new Uri("https://sp.example.org/Shibboleth.sso/SAML2/POST-SimpleSign")
						}
					}, { 3,
						new AssertionConsumerService {
							Index = 3,
							Binding = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Artifact"),
							Location = new Uri("https://sp.example.org/Shibboleth.sso/SAML2/Artifact")
						}
					}, { 4,
						new AssertionConsumerService {
							Index = 4,
							Binding = new Uri("urn:oasis:names:tc:SAML:1.0:profiles:browser-post"),
							Location = new Uri("https://sp.example.org/Shibboleth.sso/SAML/POST")
						}
					}, { 5,
						new AssertionConsumerService {
							Index = 5,
							Binding = new Uri("urn:oasis:names:tc:SAML:1.0:profiles:artifact-01"),
							Location = new Uri("https://sp.example.org/Shibboleth.sso/SAML/Artifact")
						}
					}
				}
			};

			ReadTest(xml, expected, (serializer, reader) =>
				serializer.TestReadSpSsoDescriptor(reader));
		}

#if FALSE

		static void WriteWrappedElements(XmlWriter writer, string wrapPrefix,
			string wrapName, string wrapNs, IEnumerable<XmlElement> elts)
		{
			if (elts.Any())
			{
				writer.WriteStartElement(wrapPrefix, wrapName, wrapNs);
				foreach (var elt in elts)
				{
					elt.WriteTo(writer);
				}
				writer.WriteEndElement();
			}
		}

		void WriteEndpointReference(XmlWriter writer, EndpointReference endpointReference)
		{
			writer.WriteStartElement("wsa", "EndpointReference", WsaNs);
			writer.WriteStartElement("wsa", "Address", WsaNs);
			writer.WriteStartElement(endpointReference.Uri.ToString());
			writer.WriteEndElement();

			WriteWrappedElements(writer, "wsa", "ReferenceParameters", WsaNs,
				endpointReference.ReferenceParameters);
			WriteWrappedElements(writer, "wsa", "Metadata", WsaNs,
				endpointReference.Metadata);

			writer.WriteEndElement();
		}

		void WriteEndpointReferences(XmlWriter writer, string elName, string elNs,
			ICollection<EndpointReference> endpointReferences)
		{
			foreach (var endpointReference in endpointReferences)
			{
				writer.WriteStartElement(elName, elNs);
				WriteEndpointReference(writer, endpointReference);
				writer.WriteEndElement();
			}

		}

		protected virtual void WriteApplicationServiceDescriptor(XmlWriter writer, ApplicationServiceDescriptor appService)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (appService == null)
			{
				throw new ArgumentNullException(nameof(appService));
			}
			writer.WriteStartElement("RoleDescriptor", Saml2MetadataNs);
			writer.WriteAttributeString("xsi", "type", XsiNs, "fed:ApplicationServiceType");
			writer.WriteAttributeString("xmlns", "fed", null, FedNs);

			WriteWebServiceDescriptorAttributes(writer, appService);
			WriteCustomAttributes(writer, appService);

			WriteWebServiceDescriptorElements(writer, appService);

			WriteEndpointReferences(writer, "ApplicationServiceEndpoint",
				FedNs, appService.Endpoints);
			WriteEndpointReferences(writer, "SingleSignOutNotificationEndpoint",
				FedNs, appService.Endpoints);
			WriteEndpointReferences(writer, "PassiveRequestorEndpoint",
				FedNs, appService.Endpoints);
			writer.WriteEndElement();
		}

		static string GetContactTypeString(ContactType contactType)
		{
			switch (contactType)
			{
				case ContactType.Technical:
					return "technical";
				case ContactType.Support:
					return "support";
				case ContactType.Administrative:
					return "administrative";
				case ContactType.Billing:
					return "billing";
				case ContactType.Other:
					return "other";
				default:
					throw new InvalidOperationException(
						$"Unknown ContactType enumeration value {contactType}");
			}
		}

		static void WriteStringElementIfPresent(XmlWriter writer, string elName,
			string elNs, string value)
		{
			if (!String.IsNullOrEmpty(value))
			{
				writer.WriteElementString(elName, elNs, value);
			}
		}

		static void WriteBase64Element(XmlWriter writer, string elName,
			string elNs, byte[] value)
		{
			if (value != null)
			{
				writer.WriteElementString(elName, elNs, Convert.ToBase64String(value));
			}
		}

		static void WriteStringAttributeIfPresent(XmlWriter writer, string attName,
			string attNs, string value)
		{
			if (!String.IsNullOrEmpty(value))
			{
				writer.WriteAttributeString(attName, attNs, value);
			}
		}

		static void WriteUriAttributeIfPresent(XmlWriter writer, string attName,
			string attNs, Uri value)
		{
			if (value != null)
			{
				writer.WriteAttributeString(attName, attNs, value.ToString());
			}
		}

		static void WriteBooleanAttribute(XmlWriter writer, string attName,
			string attNs, bool? value)
		{
			if (value.HasValue)
			{
				writer.WriteAttributeString(attName, attNs, value.Value ? "true" : "false");
			}
		}

		static void WriteStringElements(XmlWriter writer, string elName, string elNs,
			IEnumerable<string> values)
		{
			foreach (string value in values)
			{
				writer.WriteElementString(elName, elNs, value);
			}
		}

		protected virtual void WriteContactPerson(XmlWriter writer, ContactPerson contactPerson)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (contactPerson == null)
			{
				throw new ArgumentNullException(nameof(contactPerson));
			}

			writer.WriteStartElement("ContactPerson", Saml2MetadataNs);
			writer.WriteAttributeString("contactType", GetContactTypeString(contactPerson.Type));
			WriteCustomAttributes(writer, contactPerson);

			WriteStringElementIfPresent(writer, "Company", Saml2MetadataNs, contactPerson.Company);
			WriteStringElementIfPresent(writer, "GivenName", Saml2MetadataNs, contactPerson.GivenName);
			WriteStringElementIfPresent(writer, "SurName", Saml2MetadataNs, contactPerson.Surname);
			WriteStringElements(writer, "EmailAddress", Saml2MetadataNs, contactPerson.EmailAddresses);
			WriteStringElements(writer, "TelephoneNumber", Saml2MetadataNs, contactPerson.TelephoneNumbers);
			WriteCustomElements(writer, contactPerson);

			writer.WriteEndElement();
		}

		protected virtual void WriteCustomAttributes<T>(XmlWriter writer, T source)
		{
		}

		protected virtual void WriteCustomElements<T>(XmlWriter writer, T source)
		{
		}

		protected virtual void WriteEndpointAttributes(XmlWriter writer, Endpoint endpoint)
		{
			writer.WriteAttributeString("Binding", endpoint.Binding.ToString());
			writer.WriteAttributeString("Location", endpoint.Location.ToString());
			WriteUriAttributeIfPresent(writer, "ResponseLocation", null, endpoint.ResponseLocation);
			WriteCustomAttributes(writer, endpoint);
		}

		protected virtual void WriteEndpoint(XmlWriter writer, Endpoint endpoint,
			string name, string ns)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (endpoint == null)
			{
				throw new ArgumentNullException(nameof(endpoint));
			}
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}
			if (ns == null)
			{
				throw new ArgumentNullException(nameof(ns));
			}
			writer.WriteStartElement(name, ns);
			WriteEndpointAttributes(writer, endpoint);
			WriteCustomAttributes(writer, endpoint);
			WriteCustomElements(writer, endpoint);
			writer.WriteEndElement();
		}

		protected virtual void WriteIndexedEndpoint(XmlWriter writer, IndexedEndpoint endpoint,
			string name, string ns)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (endpoint == null)
			{
				throw new ArgumentNullException(nameof(endpoint));
			}
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}
			if (ns == null)
			{
				throw new ArgumentNullException(nameof(ns));
			}
			writer.WriteStartElement(name, ns);
			WriteEndpointAttributes(writer, endpoint);
			WriteBooleanAttribute(writer, "isDefault", null, endpoint.IsDefault);
			writer.WriteAttributeString("index", endpoint.Index.ToString());
			WriteCustomAttributes(writer, endpoint);
			WriteCustomElements(writer, endpoint);
			writer.WriteEndElement();
		}

		protected virtual void WriteEndpoints(XmlWriter writer,
			IEnumerable<Endpoint> endpoints, string name, string ns) =>
				WriteCollection(writer, endpoints, (writer_, endpoint) =>
					WriteEndpoint(writer_, endpoint, name, ns));

		protected virtual void WriteIndexedEndpoints(XmlWriter writer,
			IEnumerable<IndexedEndpoint> endpoints, string name, string ns) =>
				WriteCollection(writer, endpoints, (writer_, endpoint) =>
					WriteIndexedEndpoint(writer_, endpoint, name, ns));

		static void WriteStringElement(XmlWriter writer, string elName, string elNs, string value)
		{
			if (!String.IsNullOrEmpty(value))
			{
				writer.WriteStartElement(elName, elNs);
				writer.WriteString(value);
				writer.WriteEndElement();
			}
		}

		protected virtual void WriteEncryptionMethod(XmlWriter writer, EncryptionMethod method)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (method == null)
			{
				throw new ArgumentNullException(nameof(method));
			}

			writer.WriteStartElement("EncryptionMethod", XEncNs);
			writer.WriteAttributeString("Algorithm", method.Algorithm.ToString());
			WriteCustomAttributes(writer, method);

			if (method.KeySize != 0)
			{
				writer.WriteStartElement("KeySize", XEncNs);
				writer.WriteString(method.KeySize.ToString());
				writer.WriteEndElement();
			}
			WriteStringElement(writer, "OAEPparams", XEncNs, method.OAEPparams);

			WriteCustomElements(writer, method);
			writer.WriteEndElement();
		}

		void WriteCollection<T>(XmlWriter writer, IEnumerable<T> elts, Action<XmlWriter, T> writeHandler)
		{
			foreach (var elt in elts)
			{
				writeHandler(writer, elt);
			}
		}

		protected virtual void WriteRSAKeyValue(XmlWriter writer, RsaKeyValue rsa)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (rsa == null)
			{
				throw new ArgumentNullException(nameof(rsa));
			}

			writer.WriteStartElement("RSAKeyValue", DSigNs);
			WriteCustomAttributes(writer, rsa);

			WriteBase64Element(writer, "Modulus", DSigNs, rsa.Parameters.Modulus);
			WriteBase64Element(writer, "Exponent", DSigNs, rsa.Parameters.Exponent);

			WriteCustomElements(writer, rsa);
			writer.WriteEndElement();
		}

		static byte[] GetIntAsBigEndian(int value)
		{
			byte[] data = new byte[4];
			data[0] = (byte)(((uint)value >> 24) & 0xff);
			data[1] = (byte)(((uint)value >> 16) & 0xff);
			data[2] = (byte)(((uint)value >> 8) & 0xff);
			data[3] = (byte)((uint)value & 0xff);
			return data;
		}

		protected virtual void WriteDSAKeyValue(XmlWriter writer, DsaKeyValue dsa)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (dsa == null)
			{
				throw new ArgumentNullException(nameof(dsa));
			}

			writer.WriteStartElement("DSAKeyValue", DSigNs);
			WriteCustomAttributes(writer, dsa);

			WriteBase64Element(writer, "P", DSigNs, dsa.Parameters.P);
			WriteBase64Element(writer, "Q", DSigNs, dsa.Parameters.Q);
			WriteBase64Element(writer, "G", DSigNs, dsa.Parameters.G);
			WriteBase64Element(writer, "J", DSigNs, dsa.Parameters.J);
			WriteBase64Element(writer, "Y", DSigNs, dsa.Parameters.Y);
			WriteBase64Element(writer, "Seed", DSigNs, dsa.Parameters.Seed);
			WriteBase64Element(writer, "PgenCounter", DSigNs,
				GetIntAsBigEndian(dsa.Parameters.Counter));

			WriteCustomElements(writer, dsa);
			writer.WriteEndElement();
		}

		protected virtual void WriteECKeyValue(XmlWriter writer, EcKeyValue ec)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (ec == null)
			{
				throw new ArgumentNullException(nameof(ec));
			}

			writer.WriteStartElement("ECKeyValue", DSig11Ns);
			WriteCustomAttributes(writer, ec);

			WriteStringElement(writer, "NamedCurve", DSig11Ns,
				"urn:oid:" + ec.Parameters.Curve.Oid.ToString());

			writer.WriteStartElement("PublicKey", DSig11Ns);
			WriteBase64Element(writer, "X", DSig11Ns, ec.Parameters.Q.X);
			WriteBase64Element(writer, "Y", DSig11Ns, ec.Parameters.Q.Y);
			writer.WriteEndElement();

			WriteCustomElements(writer, ec);
			writer.WriteEndElement();
		}

		protected virtual void WriteKeyValue(XmlWriter writer, KeyValue keyValue)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (keyValue == null)
			{
				throw new ArgumentNullException(nameof(keyValue));
			}
			writer.WriteStartElement("KeyValue", DSigNs);
			WriteCustomAttributes(writer, keyValue);
			if (keyValue is RsaKeyValue rsa)
			{
				WriteRSAKeyValue(writer, rsa);
			}
			else if (keyValue is DsaKeyValue dsa)
			{
				WriteDSAKeyValue(writer, dsa);
			}
			else if (keyValue is EcKeyValue ec)
			{
				WriteECKeyValue(writer, ec);
			}
			WriteCustomElements(writer, keyValue);
			writer.WriteEndElement();
		}

		protected virtual void WriteRetrievalMethod(XmlWriter writer, RetrievalMethod method)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (method == null)
			{
				throw new ArgumentNullException(nameof(method));
			}

			writer.WriteStartElement("RetrievalMethod", DSigNs);
			WriteUriAttributeIfPresent(writer, "URI", null, method.Uri);
			WriteUriAttributeIfPresent(writer, "Type", null, method.Type);
			WriteCustomAttributes(writer, method);

			WriteWrappedElements(writer, "ds", "Transforms", DSigNs, method.Transforms); 

			WriteCustomElements(writer, method);
			writer.WriteEndElement();
		}

		protected virtual void WriteX509IssuerSerial(XmlWriter writer, X509IssuerSerial issuerSerial)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (issuerSerial == null)
			{
				throw new ArgumentNullException(nameof(issuerSerial));
			}

			writer.WriteStartElement("X509IssuerSerial", DSigNs);
			WriteCustomAttributes(writer, issuerSerial);
			WriteStringElement(writer, "X509IssuerSerial", DSigNs, issuerSerial.Name);
			WriteStringElement(writer, "X509SerialNumber", DSigNs, issuerSerial.Serial);
			WriteCustomElements(writer, issuerSerial);
			writer.WriteEndElement();
		}

		protected virtual void WriteX509Digest(XmlWriter writer, X509Digest digest)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (digest == null)
			{
				throw new ArgumentNullException(nameof(digest));
			}

			writer.WriteStartElement("X509Digest", DSigNs);
			writer.WriteAttributeString("Algorithm", digest.Algorithm.ToString());
			WriteCustomAttributes(writer, digest);
			writer.WriteBase64(digest.Value, 0, digest.Value.Length);
			WriteCustomElements(writer, digest);
			writer.WriteEndElement();
		}

		protected virtual void WriteX509Data(XmlWriter writer, X509Data data)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			writer.WriteStartElement("X509Data", DSigNs);
			WriteCustomAttributes(writer, data);
			if (data.IssuerSerial != null)
			{
				WriteX509IssuerSerial(writer, data.IssuerSerial);
			}
			if (data.SKI != null)
			{
				WriteBase64Element(writer, "X509SKI", DSigNs, data.SKI);
			}
			WriteStringElementIfPresent(writer, "X509SubjectName", DSigNs, data.SubjectName);
			foreach (var cert in data.Certificates)
			{
				WriteBase64Element(writer, "X509Certificate", DSigNs, cert.GetRawCertData());
			}
			if (data.CRL != null)
			{
				WriteBase64Element(writer, "X509CRL", DSigNs, data.CRL);
			}
			if (data.Digest != null)
			{
				WriteX509Digest(writer, data.Digest);
			}

			WriteCustomElements(writer, data);
			writer.WriteEndElement();
		}

		protected virtual void WriteKeyData(XmlWriter writer, KeyData keyData)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (keyData == null)
			{
				throw new ArgumentNullException(nameof(keyData));
			}
			if (keyData is X509Data x509Data)
			{
				WriteX509Data(writer, x509Data);
			}
		}

		protected virtual void WriteDSigKeyInfo(XmlWriter writer, DSigKeyInfo keyInfo)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (keyInfo == null)
			{
				throw new ArgumentNullException(nameof(keyInfo));
			}

			writer.WriteStartElement("KeyInfo", DSigNs);
			WriteStringAttributeIfPresent(writer, "Id", null, keyInfo.Id);
			WriteCustomAttributes(writer, keyInfo);

			WriteStringElements(writer, "KeyName", DSigNs, keyInfo.KeyNames);
			WriteCollection(writer, keyInfo.KeyValues, WriteKeyValue);
			WriteCollection(writer, keyInfo.RetrievalMethods, WriteRetrievalMethod);
			WriteCollection(writer, keyInfo.Data, WriteKeyData);
			
			WriteCustomElements(writer, keyInfo);
			writer.WriteEndElement();
		}

		protected virtual void WriteCipherReference(XmlWriter writer, CipherReference reference)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (reference == null)
			{
				throw new ArgumentNullException(nameof(reference));
			}

			writer.WriteStartElement("CipherReference", XEncNs);
			WriteUriAttributeIfPresent(writer, "URI", null, reference.Uri);
			WriteCustomAttributes(writer, reference);

			WriteWrappedElements(writer, "xenc", "Transforms", XEncNs, reference.Transforms);
			WriteCustomElements(writer, reference);
			writer.WriteEndElement();
		}

		protected virtual void WriteCipherData(XmlWriter writer, CipherData data)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			writer.WriteStartElement("CipherData", XEncNs);
			WriteCustomAttributes(writer, data);
			if (data.CipherValue != null)
			{
				WriteStringElement(writer, "CipherValue", XEncNs, data.CipherValue);
			}
			if (data.CipherReference != null)
			{
				WriteCipherReference(writer, data.CipherReference);
			}
			WriteCustomElements(writer, data);
			writer.WriteEndElement();
		}

		// <element name="EncryptionProperty" type="xenc:EncryptionPropertyType"/> 
		// 
		// <complexType name="EncryptionPropertyType" mixed="true">
		//   <choice maxOccurs="unbounded">
		//     <any namespace="##other" processContents="lax"/>
		//   </choice>
		//   <attribute name="Target" type="anyURI" use="optional"/> 
		//   <attribute name="Id" type="ID" use="optional"/> 
		//   <anyAttribute namespace="http://www.w3.org/XML/1998/namespace"/>
		// </complexType>
		protected virtual void WriteEncryptionProperty(XmlWriter writer, EncryptionProperty property)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (property == null)
			{
				throw new ArgumentNullException(nameof(property));
			}

			writer.WriteStartElement("EncryptionProperty", XEncNs);
			WriteUriAttributeIfPresent(writer, "Target", null, property.Target);
			WriteStringAttributeIfPresent(writer, "Target", null, property.Id);
			WriteCustomAttributes(writer, property);
			WriteCustomElements(writer, property);
			writer.WriteEndElement();
		}

		protected virtual void WriteEncryptionProperties(XmlWriter writer, EncryptionProperties properties)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (properties == null)
			{
				throw new ArgumentNullException(nameof(properties));
			}
			writer.WriteStartElement("EncryptionProperties", XEncNs);
			WriteStringAttributeIfPresent(writer, "Id", null, properties.Id);
			WriteCustomAttributes(writer, properties);
			WriteCollection(writer, properties.Properties, WriteEncryptionProperty);
			WriteCustomElements(writer, properties);
			writer.WriteEndElement();
		}

		protected virtual void WriteEncryptedData(XmlWriter writer, EncryptedData data)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			writer.WriteStartElement("EncryptedData", XEncNs);
			WriteStringAttributeIfPresent(writer, "Id", null, data.Id);
			WriteUriAttributeIfPresent(writer, "Type", null, data.Type);
			WriteStringAttributeIfPresent(writer, "MimeType", null, data.MimeType);
			WriteStringAttributeIfPresent(writer, "Encoding", null, data.MimeType);
			WriteCustomAttributes(writer, data);

			if (data.EncryptionMethod != null)
			{
				WriteEncryptionMethod(writer, data.EncryptionMethod);
			}
			if (data.KeyInfo != null)
			{
				WriteDSigKeyInfo(writer, data.KeyInfo);
			}
			if (data.CipherData != null)
			{
				WriteCipherData(writer, data.CipherData);
			}
			if (data.EncryptionProperties != null)
			{
				WriteEncryptionProperties(writer, data.EncryptionProperties);
			}

			WriteCustomElements(writer, data);
			writer.WriteEndElement();
		}

		protected virtual void WriteEncryptedValue(XmlWriter writer, EncryptedValue value)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			writer.WriteStartElement("EncryptedValue", AuthNs);
			writer.WriteAttributeString("DecryptionCondition", value.DecryptionCondition.ToString());
			WriteCustomAttributes(writer, value);

			WriteEncryptedData(writer, value.EncryptedData);

			WriteCustomElements(writer, value);
			writer.WriteEndElement();
		}

		protected virtual void WriteClaimValue(XmlWriter writer, ClaimValue value)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			if (!String.IsNullOrEmpty(value.Value) && value.StructuredValue != null)
			{
				throw new MetadataSerializationException(
					"Invalid claim value that has both Value and StructuredValue properties set");
			}
			if (value.Value == null && value.StructuredValue == null)
			{
				throw new MetadataSerializationException(
					"Invalid claim value that has neither Value nor StructuredValue properties set");
			}

			if (value.Value != null)
			{
				WriteStringElement(writer, "Value", AuthNs, value.Value);
			}
			else
			{
				writer.WriteStartElement("StructuredValue", AuthNs);
				value.StructuredValue.WriteTo(writer);
				writer.WriteEndElement();
			}
		}

		protected virtual void WriteCompareConstraint(XmlWriter writer,
			ConstrainedValue.CompareConstraint constraint)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (constraint == null)
			{
				throw new ArgumentNullException(nameof(constraint));
			}

			string elName;
			switch (constraint.CompareOp)
			{
				case ConstrainedValue.CompareConstraint.CompareOperator.Lt:
					elName = "ValueLessThan";
					break;
				case ConstrainedValue.CompareConstraint.CompareOperator.Lte:
					elName = "ValueLessThanOrEqual";
					break;
				case ConstrainedValue.CompareConstraint.CompareOperator.Gt:
					elName = "ValueGreaterThan";
					break;
				case ConstrainedValue.CompareConstraint.CompareOperator.Gte:
					elName = "ValueGreaterThanOrEqual";
					break;
				default:
					throw new MetadataSerializationException(
						$"Unknown constrained value compare operator '{constraint.CompareOp}'");
			}

			writer.WriteStartElement(elName, AuthNs);
			WriteCustomAttributes(writer, constraint);
			WriteClaimValue(writer, constraint.Value);
			WriteCustomElements(writer, constraint);
			writer.WriteEndElement();
		}

		protected virtual void WriteListContraint(XmlWriter writer,
			ConstrainedValue.ListConstraint constraint)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (constraint == null)
			{
				throw new ArgumentNullException(nameof(constraint));
			}

			writer.WriteStartElement("ValueOneOf", AuthNs);
			WriteCustomAttributes(writer, constraint);
			foreach (var value in constraint.Values)
			{
				WriteClaimValue(writer, value);
			}
			WriteCustomElements(writer, constraint);
			writer.WriteEndElement();
		}

		protected virtual void WriteRangeConstraint(XmlWriter writer,
			ConstrainedValue.RangeConstraint constraint)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (constraint == null)
			{
				throw new ArgumentNullException(nameof(constraint));
			}

			writer.WriteStartElement("ValueInRange", AuthNs);
			WriteCustomAttributes(writer, constraint);
			if (constraint.UpperBound != null)
			{
				writer.WriteStartElement("ValueUpperBound", AuthNs);
				WriteClaimValue(writer, constraint.UpperBound);
				writer.WriteEndElement();
			}
			if (constraint.LowerBound != null)
			{
				writer.WriteStartElement("ValueLowerBound", AuthNs);
				WriteClaimValue(writer, constraint.LowerBound);
				writer.WriteEndElement();
			}
			WriteCustomElements(writer, constraint);
			writer.WriteEndElement();
		}

		protected virtual void WriteListConstraint(XmlWriter writer,
			ConstrainedValue.ListConstraint constraint)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (constraint == null)
			{
				throw new ArgumentNullException(nameof(constraint));
			}

			writer.WriteStartElement("ValueOneOf", AuthNs);
			WriteCustomAttributes(writer, constraint);
			foreach (var value in constraint.Values)
			{
				WriteClaimValue(writer, value);
			}
			WriteCustomElements(writer, constraint);
			writer.WriteEndElement();
		}

		protected virtual void WriteConstrainedValue(XmlWriter writer, ConstrainedValue value)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			writer.WriteStartElement("ConstrainedValue", AuthNs);
			WriteBooleanAttribute(writer, "AssertConstraint", null, value.AssertConstraint);
			WriteCustomAttributes(writer, value);
			foreach (var constraint in value.Constraints)
			{
				if (constraint is ConstrainedValue.CompareConstraint cc)
				{
					WriteCompareConstraint(writer, cc);
				}
				else if (constraint is ConstrainedValue.ListConstraint lc)
				{
					WriteListConstraint(writer, lc);
				}
				else if (constraint is ConstrainedValue.RangeConstraint rc)
				{
					WriteRangeConstraint(writer, rc);
				}
				else
				{
					throw new MetadataSerializationException(
						$"Unknown constraint type '{constraint.GetType()}'");
				}
			}
			WriteCustomElements(writer, value);
			writer.WriteEndElement();
		}

		protected virtual void WriteDisplayClaim(XmlWriter writer, DisplayClaim claim)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (claim == null)
			{
				throw new ArgumentNullException(nameof(claim));
			}

			writer.WriteStartElement("ClaimType", AuthNs);
			writer.WriteAttributeString("@Uri", claim.ClaimType);
			WriteBooleanAttribute(writer, "Optional", null, claim.Optional);
			WriteCustomAttributes(writer, claim);

			WriteStringElement(writer, "DisplayName", AuthNs, claim.DisplayName);
			WriteStringElement(writer, "Description", AuthNs, claim.Description);
			WriteStringElement(writer, "DisplayValue", AuthNs, claim.DisplayValue);
			WriteStringElement(writer, "Value", AuthNs, claim.Value);
			if (claim.StructuredValue != null)
			{
				writer.WriteStartElement("StructuredValue", AuthNs);
				claim.StructuredValue.WriteTo(writer);
				writer.WriteEndElement();
			}
			WriteEncryptedValue(writer, claim.EncryptedValue);
			WriteConstrainedValue(writer, claim.ConstrainedValue);
			WriteCustomElements(writer, claim);
			writer.WriteEndElement();
		}

		static void WriteCachedMetadataAttributes(XmlWriter writer, ICachedMetadata cachedMetadata)
		{
			if (cachedMetadata.CacheDuration.HasValue)
			{
				writer.WriteAttributeString("cacheDuration",
					XmlConvert.ToString(cachedMetadata.CacheDuration.Value));
			}
			if (cachedMetadata.ValidUntil.HasValue)
			{
				writer.WriteAttributeString("validUntil",
					XmlConvert.ToString(cachedMetadata.ValidUntil.Value, XmlDateTimeSerializationMode.Utc));
			}
		}

		protected virtual void WriteEntitiesDescriptor(XmlWriter writer, EntitiesDescriptor entitiesDescriptor)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (entitiesDescriptor == null)
			{
				throw new ArgumentNullException(nameof(entitiesDescriptor));
			}

			EnvelopedSignatureWriter signatureWriter = null;
			if (entitiesDescriptor.SigningCredentials != null)
			{
				string referenceId = Guid.NewGuid().ToString("N");
				signatureWriter = new EnvelopedSignatureWriter(writer,
					entitiesDescriptor.SigningCredentials, referenceId);
				writer = signatureWriter;
			}

			writer.WriteStartElement("EntitiesDescriptor", Saml2MetadataNs);
			WriteCachedMetadataAttributes(writer, entitiesDescriptor);
			WriteStringAttributeIfPresent(writer, "ID", Saml2MetadataNs, entitiesDescriptor.Id);
			WriteStringAttributeIfPresent(writer, "Name", Saml2MetadataNs, entitiesDescriptor.Name);
			WriteCustomAttributes(writer, entitiesDescriptor);

			if (signatureWriter != null)
			{
				signatureWriter.WriteSignature();
			}

			WriteWrappedElements(writer, null, "Extensions", Saml2MetadataNs,
				entitiesDescriptor.Extensions);

			foreach (var childEntity in entitiesDescriptor.ChildEntities)
			{
				WriteEntityDescriptor(writer, childEntity);
			}

			foreach (var childEntityDescriptor in entitiesDescriptor.ChildEntityGroups)
			{
				WriteEntitiesDescriptor(writer, childEntityDescriptor);
			}
			
			WriteCustomElements(writer, entitiesDescriptor);
			writer.WriteEndElement();
		}

		protected virtual void WriteNameIDFormat(XmlWriter writer, NameIDFormat nameIDFormat)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (nameIDFormat == null)
			{
				throw new ArgumentNullException(nameof(nameIDFormat));
			}
			writer.WriteStartElement("NameIDFormat", Saml2MetadataNs);
			WriteCustomAttributes(writer, nameIDFormat);
			writer.WriteString(nameIDFormat.Uri.ToString());
			WriteCustomElements(writer, nameIDFormat);
			writer.WriteEndElement();
		}

		protected virtual void WriteAuthnAuthorityDescriptor(XmlWriter writer, AuthnAuthorityDescriptor descriptor)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (descriptor == null)
			{
				throw new ArgumentNullException(nameof(descriptor));
			}
			writer.WriteStartElement("AuthnAuthorityDescriptor", Saml2MetadataNs);
			WriteRoleDescriptorAttributes(writer, descriptor);
			WriteCustomAttributes(writer, descriptor);
			WriteRoleDescriptorElements(writer, descriptor);

			WriteEndpoints(writer, descriptor.AuthnQueryServices,
				"AuthnQueryService", Saml2MetadataNs);
			WriteEndpoints(writer, descriptor.AssertionIdRequestServices, 
				"AssertionIDRequestService", Saml2MetadataNs);
			WriteCollection(writer, descriptor.NameIDFormats, WriteNameIDFormat);

			WriteCustomElements(writer, descriptor);
			writer.WriteEndElement();
		}

		protected virtual void WriteAttributeProfile(XmlWriter writer, AttributeProfile profile)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (profile == null)
			{
				throw new ArgumentNullException(nameof(profile));
			}
			writer.WriteStartElement("AttributeProfile", Saml2MetadataNs);
			WriteCustomAttributes(writer, profile);
			writer.WriteString(profile.Uri.ToString());
			WriteCustomElements(writer, profile);
			writer.WriteEndElement();
		}

		protected virtual void WriteAttributeAuthorityDescriptor(XmlWriter writer, AttributeAuthorityDescriptor descriptor)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (descriptor == null)
			{
				throw new ArgumentNullException(nameof(descriptor));
			}
			writer.WriteStartElement("AttributeAuthorityDescriptor", Saml2MetadataNs);
			WriteRoleDescriptorAttributes(writer, descriptor);
			WriteCustomAttributes(writer, descriptor);
			WriteRoleDescriptorElements(writer, descriptor);

			foreach (var service in descriptor.AttributeServices)
			{
				WriteEndpoint(writer, service, "AttributeService", Saml2MetadataNs);
			}

			foreach (var ars in descriptor.AttributeServices)
			{
				WriteEndpoint(writer, ars, "AssertionIDRequestService", Saml2MetadataNs);
			}

			foreach (var nameIDFormat in descriptor.NameIDFormats)
			{
				WriteNameIDFormat(writer, nameIDFormat);
			}

			foreach (var attributeProfile in descriptor.AttributeProfiles)
			{
				WriteAttributeProfile(writer, attributeProfile);
			}

			foreach (var attribute in descriptor.Attributes)
			{
				WriteAttribute(writer, attribute);
			}

			WriteCustomElements(writer, descriptor);
			writer.WriteEndElement();
		}

		protected virtual void WritePDPDescriptor(XmlWriter writer, PDPDescriptor descriptor)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (descriptor == null)
			{
				throw new ArgumentNullException(nameof(descriptor));
			}
			writer.WriteStartElement("PDPDescriptor", Saml2MetadataNs);
			WriteRoleDescriptorAttributes(writer, descriptor);
			WriteCustomAttributes(writer, descriptor);

			WriteRoleDescriptorElements(writer, descriptor);
			WriteEndpoints(writer, descriptor.AuthzServices,
				"AuthzService", Saml2MetadataNs);
			WriteEndpoints(writer, descriptor.AssertionIdRequestServices,
				"AssertionIDRequestService", Saml2MetadataNs);
			foreach (var nameIdFormat in descriptor.NameIDFormats)
			{
				WriteNameIDFormat(writer, nameIdFormat);
			}

			WriteCustomElements(writer, descriptor);
			writer.WriteEndElement();
		}

		protected virtual void WriteAffiliationDescriptor(XmlWriter writer, AffiliationDescriptor descriptor)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (descriptor == null)
			{
				throw new ArgumentNullException(nameof(descriptor));
			}

			writer.WriteStartElement("AffiliationDescriptor", Saml2MetadataNs);
			writer.WriteAttributeString("affiliationOwnerID", descriptor.AffiliationOwnerId.Id);
			WriteCachedMetadataAttributes(writer, descriptor);
			WriteStringAttributeIfPresent(writer, "ID", null, descriptor.Id);
			WriteCustomAttributes(writer, descriptor);

			WriteWrappedElements(writer, "md", "Extensions", Saml2MetadataNs, descriptor.Extensions);
			WriteCollection(writer, descriptor.AffiliateMembers, (writer_, member) =>
			{
				WriteStringElement(writer_, "AffiliateMember", Saml2MetadataNs, member.Id);
			});
			WriteCollection(writer, descriptor.KeyDescriptors, WriteKeyDescriptor);

			WriteCustomElements(writer, descriptor);
			writer.WriteEndElement();
		}

		protected virtual void WriteAdditionalMetadataLocation(XmlWriter writer, AdditionalMetadataLocation location)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (location == null)
			{
				throw new ArgumentNullException(nameof(location));
			}

			writer.WriteStartElement("AdditionalMetadataLocation", Saml2MetadataNs);
			WriteCustomAttributes(writer, location);
			writer.WriteAttributeString("namespace", location.Namespace);
			writer.WriteString(location.Uri.ToString());
			WriteCustomElements(writer, location);
			writer.WriteEndElement();
		}

		protected virtual void WriteEntityDescriptor(XmlWriter writer, EntityDescriptor entityDescriptor)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (entityDescriptor == null)
			{
				throw new ArgumentNullException(nameof(entityDescriptor));
			}

			EnvelopedSignatureWriter signatureWriter = null;
			if (entityDescriptor.SigningCredentials != null)
			{
				string referenceId = Guid.NewGuid().ToString("N");
				signatureWriter = new EnvelopedSignatureWriter(writer,
					entityDescriptor.SigningCredentials, referenceId);
			}

			writer.WriteStartElement("EntityDescriptor", Saml2MetadataNs);
			WriteCachedMetadataAttributes(writer, entityDescriptor);
			WriteStringAttributeIfPresent(writer, "ID", null, entityDescriptor.Id);
			writer.WriteAttributeString("entityID", entityDescriptor.EntityId.Id);
			WriteCustomAttributes(writer, entityDescriptor);

			if (signatureWriter != null)
			{
				signatureWriter.WriteSignature();
			}

			WriteWrappedElements(writer, null, "Extensions", Saml2MetadataNs,
				entityDescriptor.Extensions);

			foreach (var roleDescriptor in entityDescriptor.RoleDescriptors)
			{
				if (roleDescriptor is ApplicationServiceDescriptor appDescriptor)
				{
					WriteApplicationServiceDescriptor(writer, appDescriptor);
				}
				else if (roleDescriptor is SecurityTokenServiceDescriptor secDescriptor)
				{
					WriteSecurityTokenServiceDescriptor(writer, secDescriptor);
				}
				else if (roleDescriptor is IdpSsoDescriptor idpSsoDescriptor)
				{
					WriteIdpSsoDescriptor(writer, idpSsoDescriptor);
				}
				else if (roleDescriptor is SpSsoDescriptor spSsoDescriptor)
				{
					WriteSpSsoDescriptor(writer, spSsoDescriptor);
				}
				else if (roleDescriptor is AuthnAuthorityDescriptor authDescriptor)
				{
					WriteAuthnAuthorityDescriptor(writer, authDescriptor);
				}
				else if (roleDescriptor is AttributeAuthorityDescriptor attDescriptor)
				{
					WriteAttributeAuthorityDescriptor(writer, attDescriptor);
				}
				else if (roleDescriptor is PDPDescriptor pdpDescriptor)
				{
					WritePDPDescriptor(writer, pdpDescriptor);
				}
			}
			foreach (AffiliationDescriptor affDescriptor in entityDescriptor.AffiliationDescriptors)
			{
				WriteAffiliationDescriptor(writer, affDescriptor);
			}
			if (entityDescriptor.Organization != null)
			{
				WriteOrganization(writer, entityDescriptor.Organization);
			}
			foreach (var person in entityDescriptor.Contacts)
			{
				WriteContactPerson(writer, person);
			}
			foreach (var meta in entityDescriptor.AdditionalMetadataLocations)
			{
				WriteAdditionalMetadataLocation(writer, meta);
			}
			WriteCustomElements(writer, entityDescriptor);
			writer.WriteEndElement();
		}

		protected virtual void WriteIdpSsoDescriptor(XmlWriter writer, IdpSsoDescriptor descriptor)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (descriptor == null)
			{
				throw new ArgumentNullException(nameof(descriptor));
			}

			writer.WriteStartElement("IDPSSODescriptor", Saml2MetadataNs);
			WriteBooleanAttribute(writer, "WantAuthnRequestsSigned", null, descriptor.WantAuthnRequestsSigned);
			WriteSsoDescriptorAttributes(writer, descriptor);
			WriteCustomAttributes(writer, descriptor);

			WriteSsoDescriptorElements(writer, descriptor);
			WriteEndpoints(writer, descriptor.SingleSignOnServices,
				"SingleSignOnService", Saml2MetadataNs);
			WriteEndpoints(writer, descriptor.NameIDMappingServices,
				"NameIDMappingService", Saml2MetadataNs);
			WriteEndpoints(writer, descriptor.AssertionIDRequestServices,
				"AssertionIDRequestService", Saml2MetadataNs);
			foreach (var attProfile in descriptor.AttributeProfiles)
			{
				WriteAttributeProfile(writer, attProfile);
			}
			foreach (var attribute in descriptor.SupportedAttributes)
			{
				WriteAttribute(writer, attribute);
			}

			WriteCustomElements(writer, descriptor);
			writer.WriteEndElement();
		}

		protected virtual void WriteKeyDescriptor(XmlWriter writer, KeyDescriptor keyDescriptor)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (keyDescriptor == null)
			{
				throw new ArgumentNullException(nameof(keyDescriptor));
			}

			writer.WriteStartElement("KeyDescriptor", Saml2MetadataNs);
			if (keyDescriptor.Use != KeyType.Unspecified)
			{
				string useValue;
				switch (keyDescriptor.Use)
				{
					case KeyType.Signing:
						useValue = "signing";
						break;
					case KeyType.Encryption:
						useValue = "encryption";
						break;
					default:
						throw new MetadataSerializationException(
							$"Unknown KeyType enumeration entry '{keyDescriptor.Use}'");
				}
				writer.WriteAttributeString("use", useValue);
			}
			WriteCustomAttributes(writer, keyDescriptor);

			if (keyDescriptor.KeyInfo != null)
			{
				WriteDSigKeyInfo(writer, keyDescriptor.KeyInfo);
			}

			WriteCollection(writer, keyDescriptor.EncryptionMethods, WriteEncryptionMethod);

			WriteCustomElements(writer, keyDescriptor);
			writer.WriteEndElement();
		}

		protected virtual void WriteLocalizedName(XmlWriter writer, LocalizedName name,
			string elName, string ns)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}
			if (elName == null)
			{
				throw new ArgumentNullException(nameof(elName));
			}
			if (ns == null)
			{
				throw new ArgumentNullException(nameof(ns));
			}

			writer.WriteStartElement(elName, ns);
			writer.WriteAttributeString("xml", "lang", XmlNs, name.Language);
			WriteCustomAttributes(writer, name);
			writer.WriteString(name.Name);
			WriteCustomElements(writer, name);
			writer.WriteEndElement();
		}

		void WriteLocalizedNames(XmlWriter writer, IEnumerable<LocalizedName> names,
			string elName, string ns) =>
				WriteCollection(writer, names, (writer_, name) =>
					WriteLocalizedName(writer_, name, elName, ns));

		protected virtual void WriteLocalizedUri(XmlWriter writer, LocalizedUri uri,
			string name, string ns)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}
			if (ns == null)
			{
				throw new ArgumentNullException(nameof(ns));
			}

			writer.WriteStartElement(name, ns);
			writer.WriteAttributeString("xml", "lang", XmlNs, uri.Language);
			WriteCustomAttributes(writer, name);
			writer.WriteString(uri.Uri.ToString());
			WriteCustomElements(writer, name);
			writer.WriteEndElement();
		}

		public void WriteMetadata(Stream stream, MetadataBase metadata)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}
			if (metadata == null)
			{
				throw new ArgumentNullException(nameof(metadata));
			}
			using (var writer = XmlWriter.Create(stream))
			{
				WriteMetadata(writer, metadata);
			}
		}

		public void WriteMetadata(XmlWriter writer, MetadataBase metadata)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (metadata == null)
			{
				throw new ArgumentNullException(nameof(metadata));
			}
			WriteMetadataCore(writer, metadata);
		}

		protected virtual void WriteMetadataCore(XmlWriter writer, MetadataBase metadata)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (metadata == null)
			{
				throw new ArgumentNullException(nameof(metadata));
			}

			if (metadata is EntitiesDescriptor entities)
			{
				WriteEntitiesDescriptor(writer, entities);
			}
			else if (metadata is EntityDescriptor entity)
			{
				WriteEntityDescriptor(writer, entity);
			}
		}

		protected virtual void WriteOrganization(XmlWriter writer, Organization organization)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (organization == null)
			{
				throw new ArgumentNullException(nameof(organization));
			}
			if (organization.Names.Count == 0)
			{
				throw new MetadataSerializationException(
					"An organisation must have at least one Name property");
			}
			if (organization.DisplayNames.Count == 0)
			{
				throw new MetadataSerializationException(
					"An organisation must have at least one DisplayName property");
			}
			if (organization.Urls.Count == 0)
			{
				throw new MetadataSerializationException(
					"An organisation must have at least one Url property");
			}

			writer.WriteStartElement("Organization", Saml2MetadataNs);
			WriteCustomAttributes(writer, organization);

			if (organization.Extensions.Count > 0)
			{
				writer.WriteStartElement("Extensions", Saml2MetadataNs);
				foreach (var extension in organization.Extensions)
				{
					extension.WriteTo(writer);
				}
				writer.WriteEndElement();
			}
			WriteLocalizedNames(writer, organization.Names,
				"OrganizationName", Saml2MetadataNs);
			WriteLocalizedNames(writer, organization.DisplayNames,
				"OrganizationDisplayName", Saml2MetadataNs);
			WriteCollection(writer, organization.Urls, (writer_, uri) =>
				WriteLocalizedUri(writer_, uri, "OrganizationURL", Saml2MetadataNs));

			WriteCustomElements(writer, organization);
			writer.WriteEndElement();
		}

		protected virtual void WriteRoleDescriptorAttributes(XmlWriter writer, RoleDescriptor descriptor)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (descriptor == null)
			{
				throw new ArgumentNullException(nameof(descriptor));
			}

			WriteStringAttributeIfPresent(writer, "ID", null, descriptor.Id);
			WriteUriAttributeIfPresent(writer, "errorURL", null, descriptor.ErrorUrl);
			WriteCachedMetadataAttributes(writer, descriptor);
			string protocolsSupported = descriptor.ProtocolsSupported.Aggregate("",
				(list, uri) => $"{list}{(list == "" ? "" : " ")}{uri}");
			writer.WriteAttributeString("protocolSupportEnumeration", protocolsSupported);
			WriteCustomAttributes(writer, descriptor);
		}

		void WriteRoleDescriptorElements(XmlWriter writer, RoleDescriptor descriptor, bool writeExtensions)
		{
			if (writeExtensions)
			{
				WriteWrappedElements(writer, null, "Extensions", Saml2MetadataNs,
					descriptor.Extensions);
			}
			foreach (var kd in descriptor.Keys)
			{
				WriteKeyDescriptor(writer, kd);
			}
			if (descriptor.Organization != null)
			{
				WriteOrganization(writer, descriptor.Organization);
			}
			foreach (var contact in descriptor.Contacts)
			{
				WriteContactPerson(writer, contact);
			}
			WriteCustomElements(writer, descriptor);
		}

		protected virtual void WriteRoleDescriptorElements(XmlWriter writer, RoleDescriptor descriptor)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (descriptor == null)
			{
				throw new ArgumentNullException(nameof(descriptor));
			}
			WriteRoleDescriptorElements(writer, descriptor, true);
		}

		protected virtual void WriteSecurityTokenServiceDescriptor(XmlWriter writer,
			SecurityTokenServiceDescriptor descriptor)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (descriptor == null)
			{
				throw new ArgumentNullException(nameof(descriptor));
			}

			writer.WriteStartElement("SecurityTokenServiceType", FedNs);
			WriteWebServiceDescriptorAttributes(writer, descriptor);
			WriteCustomAttributes(writer, descriptor);

			WriteWebServiceDescriptorElements(writer, descriptor);
			WriteEndpointReferences(writer, "SingleSignOutSubscriptionEndpoint",
				FedNs, descriptor.SecurityTokenServiceEndpoints);
			WriteEndpointReferences(writer, "SingleSignOutSubscriptionEndpoint",
				FedNs, descriptor.SingleSignOutSubscriptionEndpoints);
			WriteEndpointReferences(writer, "SingleSignOutNotificationEndpoint",
				FedNs, descriptor.SingleSignOutNotificationEndpoints);
			WriteEndpointReferences(writer, "PassiveRequestorEndpoint",
				FedNs, descriptor.PassiveRequestorEndpoints);

			WriteCustomElements(writer, descriptor);

			writer.WriteEndElement();
		}

		protected virtual void WriteRequestedAttribute(XmlWriter writer, RequestedAttribute attribute)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (attribute == null)
			{
				throw new ArgumentNullException(nameof(attribute));
			}

			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (attribute == null)
			{
				throw new ArgumentNullException(nameof(attribute));
			}

			writer.WriteStartElement("RequestedAttribute", Saml2MetadataNs);
			WriteBooleanAttribute(writer, "isRequired", null, attribute.IsRequired);
			WriteAttributeAttributes(writer, attribute);
			WriteAttributeElements(writer, attribute);
			writer.WriteEndElement();
		}

		protected virtual void WriteAttributeConsumingService(XmlWriter writer, AttributeConsumingService service)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (service == null)
			{
				throw new ArgumentNullException(nameof(service));
			}

			writer.WriteStartElement("AttributeConsumingService", Saml2MetadataNs);
			writer.WriteAttributeString("index", service.Index.ToString());
			WriteBooleanAttribute(writer, "isDefault", null, service.IsDefault);
			WriteCustomAttributes(writer, service);

			WriteLocalizedNames(writer, service.ServiceNames,
				"ServiceName", Saml2MetadataNs);
			WriteLocalizedNames(writer, service.ServiceDescriptions,
				"ServiceDescription", Saml2MetadataNs);
			WriteCollection(writer, service.RequestedAttributes,
				WriteRequestedAttribute);
			WriteCustomElements(writer, service);
			writer.WriteEndElement();
		}

		protected virtual void WriteSpSsoDescriptor(XmlWriter writer, SpSsoDescriptor descriptor)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (descriptor == null)
			{
				throw new ArgumentNullException(nameof(descriptor));
			}

			writer.WriteStartElement("SPSSODescriptor", Saml2MetadataNs);
			WriteBooleanAttribute(writer, "AuthnRequestsSigned", null, descriptor.AuthnRequestsSigned);
			WriteBooleanAttribute(writer, "WantAssertionsSigned", null, descriptor.WantAssertionsSigned);
			WriteSsoDescriptorAttributes(writer, descriptor);
			WriteCustomAttributes(writer, descriptor);

			if (descriptor.DiscoveryResponses.Count > 0 || descriptor.Extensions.Any())
			{
				writer.WriteStartElement("Extensions", Saml2MetadataNs);
				WriteIndexedEndpoints(writer, descriptor.DiscoveryResponses.Values,
					"DiscoveryResponse", IdpDiscNs);
				foreach (var extension in descriptor.Extensions)
				{
					extension.WriteTo(writer);
				}
				writer.WriteEndElement();
			}
			WriteSsoDescriptorElements(writer, descriptor, false);
			WriteIndexedEndpoints(writer, descriptor.AssertionConsumerServices.Values,
				"AssertionConsumerService", Saml2MetadataNs);
			WriteCollection(writer, descriptor.AttributeConsumingServices.Values,
				WriteAttributeConsumingService);

			WriteCustomElements(writer, descriptor);
			writer.WriteEndElement();
		}

		protected virtual void WriteSsoDescriptorAttributes(XmlWriter writer, SsoDescriptor descriptor)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (descriptor == null)
			{
				throw new ArgumentNullException(nameof(descriptor));
			}
			WriteRoleDescriptorAttributes(writer, descriptor);
			WriteCustomAttributes(writer, descriptor);
		}

		void WriteSsoDescriptorElements(XmlWriter writer, SsoDescriptor descriptor, bool writeExtensions)
		{
			WriteRoleDescriptorElements(writer, descriptor, writeExtensions);
			WriteIndexedEndpoints(writer, descriptor.ArtifactResolutionServices.Values,
				"ArtifactResolutionService", Saml2MetadataNs);
			WriteEndpoints(writer, descriptor.SingleLogoutServices,
				"SingleLogoutService", Saml2MetadataNs);
			WriteEndpoints(writer, descriptor.ManageNameIDServices,
				"ManageNameIDService", Saml2MetadataNs);
			WriteCollection(writer, descriptor.NameIdentifierFormats, WriteNameIDFormat);
			WriteCustomElements(writer, descriptor);
		}

		protected virtual void WriteSsoDescriptorElements(XmlWriter writer, SsoDescriptor descriptor)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (descriptor == null)
			{
				throw new ArgumentNullException(nameof(descriptor));
			}

			WriteSsoDescriptorElements(writer, descriptor, true);
		}

		void WriteUris(XmlWriter writer, string parentElementName,
			string childElementName, string ns, IEnumerable<Uri> uris)
		{
			if (!uris.Any())
			{
				return;
			}
			writer.WriteStartElement(parentElementName, ns);
			foreach (var uri in uris)
			{
				writer.WriteStartElement(childElementName, ns);
				writer.WriteAttributeString("Uri", uri.ToString());
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		protected virtual void WriteWebServiceDescriptorAttributes(XmlWriter writer, WebServiceDescriptor descriptor)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (descriptor == null)
			{
				throw new ArgumentNullException(nameof(descriptor));
			}

			WriteRoleDescriptorAttributes(writer, descriptor);
			WriteStringAttributeIfPresent(writer, "ServiceDisplayName",
				null, descriptor.ServiceDisplayName);
			WriteStringAttributeIfPresent(writer, "ServiceDescription",
				null, descriptor.ServiceDescription);
		}

		void WriteDisplayClaims(XmlWriter writer, string parentName, string parentNs,
			IEnumerable<DisplayClaim> claims)
		{
			if (!claims.Any())
			{
				return;
			}

			writer.WriteStartElement(parentName, parentNs);
			WriteCollection(writer, claims, WriteDisplayClaim);
			writer.WriteEndElement();
		}

		protected virtual void WriteWebServiceDescriptorElements(XmlWriter writer, WebServiceDescriptor descriptor)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (descriptor == null)
			{
				throw new ArgumentNullException(nameof(descriptor));
			}

			WriteRoleDescriptorElements(writer, descriptor);

			WriteUris(writer, "LogicalServiceNamesOffered",
				"IssuerName", FedNs, descriptor.LogicalServiceNamesOffered);
			WriteUris(writer, "TokenTypesOffered",
				"TokenType", FedNs, descriptor.TokenTypesOffered);
			WriteUris(writer, "ClaimDialectsOffered",
				"ClaimDialect", FedNs, descriptor.ClaimDialectsOffered);

			WriteDisplayClaims(writer, "ClaimTypesOffered", FedNs,
				descriptor.ClaimTypesOffered);
			WriteDisplayClaims(writer, "ClaimTypesRequested", FedNs,
				descriptor.ClaimTypesRequested);

			if (descriptor.AutomaticPseudonyms.HasValue)
			{
				writer.WriteStartElement("AutomaticPseudonyms", FedNs);
				writer.WriteString(descriptor.AutomaticPseudonyms.Value ? "true" : "false");
				writer.WriteEndElement();
			}
			if (descriptor.TargetScopes.Count > 0)
			{
				writer.WriteStartElement("TargetScopes", FedNs);
				WriteCollection(writer, descriptor.TargetScopes, WriteEndpointReference);
				writer.WriteEndElement();
			}

			WriteCustomElements(writer, descriptor);
		}

		protected virtual void WriteAttributeAttributes(XmlWriter writer, Saml2Attribute attribute)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (attribute == null)
			{
				throw new ArgumentNullException(nameof(attribute));
			}

			writer.WriteAttributeString("Name", attribute.Name);
			WriteUriAttributeIfPresent(writer, "NameFormat", null, attribute.NameFormat);
			WriteStringAttributeIfPresent(writer, "FriendlyName", null, attribute.FriendlyName);
			WriteCustomAttributes(writer, attribute);
		}

		protected virtual void WriteAttributeElements(XmlWriter writer,
			Saml2Attribute attribute)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (attribute == null)
			{
				throw new ArgumentNullException(nameof(attribute));
			}

			WriteCollection(writer, attribute.Values, (writer_, value) =>
				WriteStringElement(writer, "AttributeValue", Saml2AssertionNs, value));
			WriteCustomElements(writer, attribute);
		}

		protected virtual void WriteAttribute(XmlWriter writer, Saml2Attribute attribute)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			if (attribute == null)
			{
				throw new ArgumentNullException(nameof(attribute));
			}

			writer.WriteStartElement("attribute", Saml2AssertionNs);
			WriteAttributeAttributes(writer, attribute);
			WriteAttributeElements(writer, attribute);
			writer.WriteEndElement();
		}
	}
}
#endif
	}
}
