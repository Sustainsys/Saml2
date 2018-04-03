using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Sustainsys.Saml2.Metadata;
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

		public AssertionConsumerService TestReadAssertionConsumerService(XmlReader reader) =>
			base.ReadAssertionConsumerService(reader);
	}

	[TestClass]
	public class MetadataSerializerTests
	{
		static Regex whitespaceRe = new Regex("[ \t\r\n]");

		XmlNamespaceManager CreateNamespaceManager(XmlDocument doc)
		{
			var nsmgr = new XmlNamespaceManager(doc.NameTable);
			// const string XmlNs = "http://www.w3.org/XML/1998/namespace";
			nsmgr.AddNamespace("auth", "http://docs.oasis-open.org/wsfed/authorization/200706/authclaims");
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
					<wsa:Address>http://dummy.idp.com/ApplicationServiceEndpoint/</wsa:Address>
					<wsa:ReferenceParameters><ref>Some reference</ref></wsa:ReferenceParameters>
					<wsa:Metadata><any>Anything at all</any></wsa:Metadata>
				</fed:ApplicationServiceEndpoint>
				<fed:SingleSignOutNotificationEndpoint>
					<wsa:Address>http://dummy.idp.com/SingleSignOutNotificationEndpoint/</wsa:Address>
					<wsa:ReferenceParameters><ref>Some reference 2</ref></wsa:ReferenceParameters>
					<wsa:Metadata><any>Anything at all 2</any></wsa:Metadata>
				</fed:SingleSignOutNotificationEndpoint>
				<fed:PassiveRequestorEndpoint>
					<wsa:Address>http://dummy.idp.com/PassiveRequestorEndpoint/</wsa:Address>
					<wsa:ReferenceParameters><ref>Some reference 3</ref></wsa:ReferenceParameters>
					<wsa:Metadata><any>Anything at all 3</any></wsa:Metadata>
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
							"md:RoleDescriptor/fed:ApplicationServiceEndpoint/wsa:ReferenceParameters/*",
							nsmgr).As<XmlElement>() },
						Metadata = { doc.SelectSingleNode(
							"md:RoleDescriptor/fed:ApplicationServiceEndpoint/wsa:Metadata/*",
							nsmgr).As<XmlElement>() }
					}
				},
				SingleSignOutEndpoints = {
					new EndpointReference()
					{
						Uri = new Uri("http://dummy.idp.com/SingleSignOutNotificationEndpoint/"),
						ReferenceParameters = { doc.SelectSingleNode(
							"md:RoleDescriptor/fed:SingleSignOutNotificationEndpoint/wsa:ReferenceParameters/*",
							nsmgr).As<XmlElement>() },
						Metadata = { doc.SelectSingleNode(
							"md:RoleDescriptor/fed:SingleSignOutNotificationEndpoint/wsa:Metadata/*",
							nsmgr).As<XmlElement>() }
					}
				},
				PassiveRequestorEndpoints = {
					new EndpointReference()
					{
						Uri = new Uri("http://dummy.idp.com/PassiveRequestorEndpoint/"),
						ReferenceParameters = { doc.SelectSingleNode(
							"md:RoleDescriptor/fed:PassiveRequestorEndpoint/wsa:ReferenceParameters/*",
							nsmgr).As<XmlElement>() },
						Metadata = { doc.SelectSingleNode(
							"md:RoleDescriptor/fed:PassiveRequestorEndpoint/wsa:Metadata/*",
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
		// 

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
			string certData = whitespaceRe.Replace(@"
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

#if FALSE

		// <complexType name="EncryptedType" abstract="true">
		//   <sequence>
		//     <element name="EncryptionMethod" type="xenc:EncryptionMethodType" 
		//              minOccurs="0"/>
		//     <element ref="ds:KeyInfo" minOccurs="0"/>
		//     <element ref="xenc:CipherData"/>
		//     <element ref="xenc:EncryptionProperties" minOccurs="0"/>
		//   </sequence>
		//   <attribute name="Id" type="ID" use="optional"/>
		//   <attribute name="Type" type="anyURI" use="optional"/>
		//   <attribute name="MimeType" type="string" use="optional"/>
		//   <attribute name="Encoding" type="anyURI" use="optional"/> 
		// </complexType>
		//
		// <element name="EncryptedData" type="xenc:EncryptedDataType"/>
		// <complexType name="EncryptedDataType">
		//   <complexContent>
		//     <extension base="xenc:EncryptedType">
		//     </extension>
		//   </complexContent>
		// </complexType>
		protected virtual EncryptedData ReadEncryptedData(XmlReader reader)
		{
			var data = CreateEncryptedDataInstance();
			data.Id = GetAttribute(reader, "Id", data.Id);
			data.Type = GetUriAttribute(reader, "Type", data.Type);
			data.MimeType = GetAttribute(reader, "MimeType", data.MimeType);
			data.Encoding = GetUriAttribute(reader, "Encoding", data.Encoding);
			ReadCustomAttributes(reader, data);

			ReadChildren(reader, () =>
			{
				if (reader.IsStartElement("EncryptionMethod", XEncNs))
				{
					data.EncryptionMethod = ReadEncryptionMethod(reader);
				}
				else if (reader.IsStartElement("KeyInfo", DSigNs))
				{
					data.KeyInfo = ReadDSigKeyInfo(reader);
				}
				else if (reader.IsStartElement("CipherData", XEncNs))
				{
					data.CipherData = ReadCipherData(reader);
				}
				else if (reader.IsStartElement("EncryptionProperties", XEncNs))
				{
					data.EncryptionProperties = ReadEncryptionProperties(reader);
				}
				else
				{
					return ReadCustomElement(reader, data);
				}
				return true; // handled above
			});

			if (data.CipherData == null)
			{
				throw new MetadataSerializationException("EncryptedData without required CipherData element");
			}

			return data;
		}

		// <auth:EncryptedValue @DecryptionCondition="xs:anyURI">
		//   <xenc:EncryptedData>...</xenc:EncryptedData>
		// <auth:EncryptedValue>)
		protected virtual EncryptedValue ReadEncryptedValue(XmlReader reader)
		{
			var value = CreateEncryptedValueInstance();
			value.DecryptionCondition = GetUriAttribute(reader, "DecryptionCondition",
				value.DecryptionCondition);

			ReadChildren(reader, () =>
			{
				if (reader.IsStartElement("EncryptedData", XEncNs))
				{
					value.EncryptedData = ReadEncryptedData(reader);
				}
				else
				{
					return ReadCustomElement(reader, value);
				}
				return true; // handled above
			});

			return value;
		}

		// <auth:ConstrainedValue AssertConstraint="xs:boolean">
		//    ( <auth:ValueLessThan>
		//        (<auth:Value> xs:string </auth:Value> |
		//        <auth:StructuredValue> xs:any </auth:StructuredValue>)
		//      </auth:ValueLessThan> |
		//      <auth:ValueLessThanOrEqual>
		//        (<auth:Value> xs:string </auth:Value> |
		//        <auth:StructuredValue> xs:any </auth:StructuredValue>)
		//      </auth:ValueLessThanOrEqual> |
		//      <auth:ValueGreaterThan>
		//        (<auth:Value> xs:string </auth:Value> |
		//        <auth:StructuredValue> xs:any </auth:StructuredValue>)
		//      </auth:ValueGreaterThan> |
		//      <auth:ValueGreaterThanOrEqual>
		//        (<auth:Value> xs:string </auth:Value> |
		//        <auth:StructuredValue> xs:any </auth:StructuredValue>)
		//      </auth:ValueGreaterThanOrEqual> |
		//      <auth:ValueInRange>
		//        <auth:ValueUpperBound>
		//          (<auth:Value> xs:string </auth:Value> |
		//          <auth:StructuredValue> xs:any </auth:StructuredValue>)
		//        </auth:ValueUpperBound>
		//        <auth:ValueLowerBound>
		//          (<auth:Value> xs:string </auth:Value> |
		//          <auth:StructuredValue> xs:any </auth:StructuredValue>)
		//        </auth:ValueLowerBound>
		//      </auth:ValueInRange> |
		//      <auth:ValueOneOf>
		//        (<auth:Value> xs:string </auth:Value> |
		//         <auth:StructuredValue> xs:any </auth:StructuredValue>) +
		//      </auth:ValueOneOf> ) ?
		//     ...
		//   </auth:ConstrainedValue> ?
		protected virtual ClaimValue ReadClaimValue(XmlReader reader)
		{
			var value = CreateClaimValueInstance();
			ReadCustomAttributes(reader, value);

			ReadChildren(reader, () =>
			{
				if (reader.IsStartElement("Value", AuthNs))
				{
					value.Value = reader.ReadElementContentAsString();
				}
				else if (reader.IsStartElement("StructuredValue", AuthNs))
				{
					value.StructuredValue = ReadCurrentNodeAsXmlElement(reader);
				}
				else
				{
					return ReadCustomElement(reader, value);
				}
				return true;
			});

			return value;
		}

		protected virtual ConstrainedValue.CompareConstraint ReadCompareConstraint(
			XmlReader reader, ConstrainedValue.CompareConstraint.CompareOperator op)
		{
			var cc = new ConstrainedValue.CompareConstraint(op);
			cc.Value = ReadClaimValue(reader);
			return cc;
		}

		protected virtual ConstrainedValue.RangeConstraint ReadRangeConstraint(XmlReader reader)
		{
			var range = new ConstrainedValue.RangeConstraint();
			ReadChildren(reader, () =>
			{
				if (reader.IsStartElement("ValueUpperBound", AuthNs))
				{
					range.UpperBound = ReadClaimValue(reader);
				}
				else if (reader.IsStartElement("ValueLowerBound", AuthNs))
				{
					range.LowerBound = ReadClaimValue(reader);
				}
				else
				{
					reader.Skip();
				}
				return true;
			});
			return range;
		}

		protected virtual ConstrainedValue ReadConstrainedValue(XmlReader reader)
		{
			var constraint = CreateConstrainedValueInstance();
			constraint.AssertConstraint = GetBooleanAttribute(reader,
				"AssertConstraint", constraint.AssertConstraint);
			ReadCustomAttributes(reader, constraint);

			ConstrainedValue.ListConstraint listConstraint = null;
			ReadChildren(reader, () =>
			{
				if (reader.IsStartElement("ValueLessThan", AuthNs))
				{
					constraint.Constraints.Add(
						ReadCompareConstraint(reader,
							ConstrainedValue.CompareConstraint.CompareOperator.Lt));
				}
				else if (reader.IsStartElement("ValueLessThanOrEqual", AuthNs))
				{
					constraint.Constraints.Add(
						ReadCompareConstraint(reader,
							ConstrainedValue.CompareConstraint.CompareOperator.Lte));
				}
				else if (reader.IsStartElement("ValueGreaterThan", AuthNs))
				{
					constraint.Constraints.Add(
						ReadCompareConstraint(reader,
							ConstrainedValue.CompareConstraint.CompareOperator.Gt));
				}
				else if (reader.IsStartElement("ValueGreaterThanOrEqual", AuthNs))
				{
					constraint.Constraints.Add(
						ReadCompareConstraint(reader,
							ConstrainedValue.CompareConstraint.CompareOperator.Gte));
				}
				else if (reader.IsStartElement("ValueInRange", AuthNs))
				{
					constraint.Constraints.Add(ReadRangeConstraint(reader));
				}
				else if (reader.IsStartElement("ValueOneOf", AuthNs))
				{
					if (listConstraint == null)
					{
						listConstraint = new ConstrainedValue.ListConstraint();
					}
					listConstraint.Values.Add(ReadClaimValue(reader));
				}
				else
				{
					return ReadCustomElement(reader, constraint);
				}
				return true; // hanlded above
			});

			return constraint;
		}

		// <auth:ClaimType Uri="xs:anyURI" Optional="xs:boolean">
		//   <auth:DisplayName  ...> xs:string </auth:DisplayName> ?
		//   <auth:Description  ...> xs:string </auth:Description> ?
		//   <auth:DisplayValue ...> xs:string </auth:DisplayValue> ?
		//   (<auth:Value>...</auth:Value> |
		//    <auth:StructuredValue ...>...</auth:StructuredValue> |
		//    (<auth:EncryptedValue @DecryptionCondition="xs:anyURI">
		//       <xenc:EncryptedData>...</xenc:EncryptedData>
		//     <auth:EncryptedValue>) |
		//    <auth:ConstrainedValue>...</auth:ConstrainedValue>) ?
		//    ...
		// </auth:ClaimType>
		protected virtual DisplayClaim ReadDisplayClaim(XmlReader reader)
		{
			string uri = reader.GetAttribute("Uri");
			if (string.IsNullOrEmpty(uri))
			{
				throw new MetadataSerializationException("auth:ClaimType is missing a Uri attribute");
			}
			DisplayClaim claim = new DisplayClaim(uri);
			claim.Optional = GetBooleanAttribute(reader, "Optional", false);
			ReadCustomAttributes(reader, claim);

			ReadChildren(reader, () =>
			{
				if (reader.IsStartElement("DisplayName", AuthNs))
				{
					claim.DisplayName = reader.ReadElementContentAsString();
				}
				else if (reader.IsStartElement("Description", AuthNs))
				{
					claim.Description = reader.ReadElementContentAsString();
				}
				else if (reader.IsStartElement("DisplayValue", AuthNs))
				{
					claim.DisplayValue = reader.ReadElementContentAsString();
				}
				else if (reader.IsStartElement("Value", AuthNs))
				{
					claim.Value = reader.ReadElementContentAsString();
				}
				else if (reader.IsStartElement("StructuredValue", AuthNs))
				{
					ReadChildrenAsXmlElements(reader, (elt) =>
					{
						if (claim.StructuredValue != null)
						{
							throw new MetadataSerializationException(
								"Unexpected StructuredValue with multiple child elements");
						}
						claim.StructuredValue = elt;
					});
				}
				else if (reader.IsStartElement("EncryptedValue", AuthNs))
				{
					claim.EncryptedValue = ReadEncryptedValue(reader);
				}
				else if (reader.IsStartElement("ConstrainedValue", AuthNs))
				{
					claim.ConstrainedValue = ReadConstrainedValue(reader);
				}
				else
				{
					return ReadCustomElement(reader, claim);
				}
				return true; // handled above
			});
			return claim;
		}

		void ReadCachedMetadataAttributes(XmlReader reader, ICachedMetadata obj)
		{
			ReadOptionalDateAttribute(reader, "validUntil",
				v => obj.ValidUntil = v);
			ReadOptionalTimespanAttribute(reader, "cacheDuration",
				v => obj.CacheDuration = v);
		}

		// <element name="EntitiesDescriptor" type="md:EntitiesDescriptorType"/>
		// <complexType name="EntitiesDescriptorType">
		//   <sequence>
		//     <element ref="ds:Signature" minOccurs="0"/>
		//     <element ref="md:Extensions" minOccurs="0"/>
		//     <choice minOccurs="1" maxOccurs="unbounded">
		//     <element ref="md:EntityDescriptor"/>
		//     <element ref="md:EntitiesDescriptor"/>
		//     </choice>
		//   </sequence>
		//  <attribute name="validUntil" type="dateTime" use="optional"/>
		//  <attribute name="cacheDuration" type="duration" use="optional"/>
		//  <attribute name="ID" type="ID" use="optional"/>
		//  <attribute name="Name" type="string" use="optional"/>
		// </complexType>
		// <element name="Extensions" type="md:ExtensionsType"/>
		// <complexType final="#all" name="ExtensionsType">
		//   <sequence>
		//     <any namespace="##other" processContents="lax" maxOccurs="unbounded"/>
		//   </sequence>
		// </complexType>
		protected virtual EntitiesDescriptor ReadEntitiesDescriptor(XmlReader reader, SecurityTokenResolver tokenResolver)
		{
			if (reader == null)
			{
				throw new ArgumentNullException(nameof(reader));
			}
			var entities = CreateEntitiesDescriptorInstance();

			// This should pick up dsig: elements.  It can also validate the signature,
			// (although I couldn't see where it's grabbing the original token stream
			// from the code)
			var sigReader = new EnvelopedSignatureReader(reader);

			entities.Name = GetAttribute(reader, "Name", entities.Id);
			entities.Id = GetAttribute(reader, "ID", entities.Id);
			ReadCachedMetadataAttributes(reader, entities);
			ReadCustomAttributes(reader, entities);

			ReadChildren(reader, () =>
			{
				if (reader.IsStartElement("EntityDescriptor", Saml2MetadataNs))
				{
					entities.ChildEntities.Add(ReadEntityDescriptor(reader, tokenResolver));
				}
				else if (reader.IsStartElement("EntitiesDescriptor", Saml2MetadataNs))
				{
					entities.ChildEntityGroups.Add(ReadEntitiesDescriptor(reader, tokenResolver));
				}
				else if (reader.IsStartElement("Extensions", Saml2MetadataNs))
				{
					ReadChildrenAsXmlElements(reader, entities.Extensions.Add);
				}
				else
				{
					return ReadCustomElement(reader, entities);
				}
				return true; // handled above
			});

			// TODO: something with the signature
			return entities;
		}

		public X509RevocationMode RevocationMode { get; set; }
		public List<string> TrustedIssuers { get; } = new List<string>();

		// TODO: call this?
		protected virtual void ValidateSigningCredential(SigningCredentials signingCredentials)
		{
			if (signingCredentials == null)
			{
				throw new ArgumentNullException(nameof(signingCredentials));
			}

			throw new NotImplementedException();

			/*
            if (CertificateValidationMode != X509CertificateValidationMode.Custom)
            {
                CertificateValidator = X509Util.CreateCertificateValidator(CertificateValidationMode, RevocationMode, TrustedStoreLocation);
            }
            else if (CertificateValidationMode == X509CertificateValidationMode.Custom && CertificateValidator == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.GetString(SR.ID4280)));
            }

            X509Certificate2 certificate = GetMetadataSigningCertificate(signingCredentials.SigningKeyIdentifier);

            ValidateIssuer(certificate);
            CertificateValidator.Validate(certificate);*/
		}

		protected virtual void ValidateIssuer(X509Certificate2 signingCertificate)
		{
		}

		protected virtual X509Certificate2 GetMetadataSigningCertificate(SecurityKeyIdentifier ski)
		{
			if (ski == null)
			{
				throw new ArgumentNullException(nameof(ski));
			}
			X509RawDataKeyIdentifierClause clause;
			if (!ski.TryFind<X509RawDataKeyIdentifierClause>(out clause))
			{
				throw new InvalidOperationException("An x509 certificate is not assocaited with the given SecurityKeyIdentifier");
			}
			return new X509Certificate2(clause.GetX509RawData());
		}

		// <element name="Attribute" type="saml:AttributeType"/>
		// <complexType name="AttributeType">
		//   <sequence>
		//     <element ref="saml:AttributeValue" minOccurs="0" maxOccurs="unbounded"/>
		//   </sequence>
		//   <attribute name="Name" type="string" use="required"/>
		//   <attribute name="NameFormat" type="anyURI" use="optional"/>
		//   <attribute name="FriendlyName" type="string" use="optional"/>
		//   <anyAttribute namespace="##other" processContents="lax"/>
		// </complexType>
		protected T ReadSamlAttributeType<T>(XmlReader reader, Func<string, T> createInstance)
			where T : Saml2Attribute		{			string name = GetAttribute(reader, "Name", null);
			if (String.IsNullOrEmpty(name))
			{
				throw new MetadataSerializationException("saml:Attribute without Name attribute");
			}
			var attribute = createInstance(name);
			attribute.NameFormat = GetUriAttribute(reader, "NameFormat", attribute.NameFormat);
			attribute.FriendlyName = GetAttribute(reader, "FriendlyName", attribute.FriendlyName);

			ReadChildren(reader, () =>
			{
				if (reader.IsStartElement("AttributeValue", Saml2AssertionNs))
				{
					attribute.Values.Add(reader.ReadElementContentAsString());
				}
				else
				{
					return ReadCustomElement(reader, attribute);
				}
				return true; // handled above
			});
			return attribute;
		}

		protected virtual Saml2Attribute ReadSaml2Attribute(XmlReader reader)
		{
			var attribute = ReadSamlAttributeType(reader, CreateSaml2AttributeInstance);
			ReadCustomAttributes(reader, attribute);
			return attribute;
		}

		// <element name="AffiliationDescriptor" type="md:AffiliationDescriptorType"/>
		// <complexType name="AffiliationDescriptorType">
		//   <sequence>
		//     <element ref="ds:Signature" minOccurs="0"/>
		//     <element ref="md:Extensions" minOccurs="0"/>
		//     <element ref="md:AffiliateMember" maxOccurs="unbounded"/>
		//     <element ref="md:KeyDescriptor" minOccurs="0" maxOccurs="unbounded"/>
		//   </sequence>
		//   <attribute name="affiliationOwnerID" type="md:entityIDType"
		//     use="required"/>
		//   <attribute name="validUntil" type="dateTime" use="optional"/>
		//   <attribute name="cacheDuration" type="duration" use="optional"/>
		//   <attribute name="ID" type="ID" use="optional"/>
		//   <anyAttribute namespace="##other" processContents="lax"/>
		// </complexType>
		// <element name="AffiliateMember" type="md:entityIDType"/>
		protected virtual AffiliationDescriptor ReadAffiliationDescriptor(XmlReader reader)
		{
			// TODO: this can be signed, need to validate the signature
			var descriptor = CreateAffiliationDescriptorInstance();

			string affiliationOwnerID = reader.GetAttribute("affiliationOwnerID");
			if (String.IsNullOrEmpty(affiliationOwnerID))
			{
				throw new MetadataSerializationException("AffiliationDescriptor element without affiliationOwnerID attribute");
			}
			descriptor.AffiliationOwnerId = new EntityId(affiliationOwnerID);
			ReadCachedMetadataAttributes(reader, descriptor);
			descriptor.Id = GetAttribute(reader, "ID", descriptor.Id);
			ReadCustomAttributes(reader, descriptor);

			ReadChildren(reader, () =>
			{
				if (reader.IsStartElement("Extensions", Saml2MetadataNs))
				{
					ReadChildrenAsXmlElements(reader, descriptor.Extensions.Add);
				}
				else if (reader.IsStartElement("AffiliateMember", Saml2MetadataNs))
				{
					string member = reader.ReadElementContentAsString();
					if (String.IsNullOrEmpty(member))
					{
						throw new MetadataSerializationException("AffiliateMember element without content");
					}
					descriptor.AffiliateMembers.Add(new EntityId(member));
				}
				else if (reader.IsStartElement("KeyDescriptor", Saml2MetadataNs))
				{
					descriptor.KeyDescriptors.Add(ReadKeyDescriptor(reader));
				}
				else
				{
					return ReadCustomElement(reader, descriptor);
				}
				return true; // handled above
			});

			return descriptor;
		}

		// <element name="AdditionalMetadataLocation"
		//   type="md:AdditionalMetadataLocationType"/>
		// <complexType name="AdditionalMetadataLocationType">
		//   <simpleContent>
		//     <extension base="anyURI">
		//       <attribute name="namespace" type="anyURI" use="required"/>
		//     </extension>
		//   </simpleContent>
		// </complexType>
		protected virtual AdditionalMetadataLocation ReadAdditionalMetadataLocation(XmlReader reader)
		{
			var location = CreateAdditionalMetadataLocationInstance();
			location.Namespace = GetAttribute(reader, "namespace", location.Namespace);
			if (String.IsNullOrEmpty(location.Namespace))
			{
				throw new MetadataSerializationException("AdditionalMetadataLocation element without namespace attribute");
			}
			ReadCustomAttributes(reader, location);
			location.Uri = MakeUri(reader.ReadElementContentAsString());
			return location;
		}

		// <element name="EntityDescriptor" type="md:EntityDescriptorType"/>
		//   <complexType name="EntityDescriptorType">
		//     <sequence>
		//       <element ref="ds:Signature" minOccurs="0"/>
		//       <element ref="md:Extensions" minOccurs="0"/>
		//       <choice>
		//         <choice maxOccurs="unbounded">
		//           <element ref="md:RoleDescriptor"/>
		//           <element ref="md:IDPSSODescriptor"/>
		//           <element ref="md:SPSSODescriptor"/>
		//           <element ref="md:AuthnAuthorityDescriptor"/>
		//           <element ref="md:AttributeAuthorityDescriptor"/>
		//           <element ref="md:PDPDescriptor"/>
		//         </choice>
		//         <element ref="md:AffiliationDescriptor"/>
		//       </choice>
		//       <element ref="md:Organization" minOccurs="0"/>
		//       <element ref="md:ContactPerson" minOccurs="0" maxOccurs="unbounded"/>
		//       <element ref="md:AdditionalMetadataLocation" minOccurs="0"
		//         maxOccurs="unbounded"/>
		//     </sequence>
		//     <attribute name="entityID" type="md:entityIDType" use="required"/>
		//     <attribute name="validUntil" type="dateTime" use="optional"/>
		//     <attribute name="cacheDuration" type="duration" use="optional"/>
		//     <attribute name="ID" type="ID" use="optional"/>
		//     <anyAttribute namespace="##other" processContents="lax"/>
		//   </complexType>
		protected virtual EntityDescriptor ReadEntityDescriptor(XmlReader reader, SecurityTokenResolver tokenResolver)
		{
			if (reader == null)
			{
				throw new ArgumentNullException(nameof(reader));
			}
			var descriptor = CreateEntityDescriptorInstance();
			
			// This should pick up dsig: elements.  It can also validate the signature,
			// (although I couldn't see where it's grabbing the original token stream
			// from the code)
			var sigReader = new EnvelopedSignatureReader(reader);

			string entityId = reader.GetAttribute("entityID");
			if (String.IsNullOrEmpty(entityId))
			{
				throw new MetadataSerializationException("EntityDescriptor is missing required entityID attribute");
			}
			descriptor.EntityId = new EntityId(entityId);
			ReadCachedMetadataAttributes(reader, descriptor);
			descriptor.Id = GetAttribute(reader, "ID", descriptor.Id);

			ReadCustomAttributes(reader, descriptor);

			ReadChildren(reader, () =>
			{
				if (reader.IsStartElement("RoleDescriptor", Saml2MetadataNs))
				{
					string xsi = reader.GetAttribute("xsi", XsiNs);
					int colonPos = xsi.IndexOf(':');
					if (colonPos < 0)
					{
						throw new MetadataSerializationException($"Invalid RoleDescriptor extension type {xsi}");
					}
					string extensionNs = reader.LookupNamespace(xsi.Substring(0, colonPos));
					string extensionType = xsi.Substring(colonPos + 1);

					RoleDescriptor roleDescriptor = null;
					if (String.Equals(extensionNs, FedNs, StringComparison.Ordinal))
					{
						if (String.Equals(extensionType, "ApplicationServiceType", StringComparison.Ordinal))
						{
							descriptor.RoleDescriptors.Add(
								ReadApplicationServiceDescriptor(reader));
						}
						else if (String.Equals(extensionType, "SecurityTokenServiceType", StringComparison.Ordinal))
						{
							descriptor.RoleDescriptors.Add(
								ReadSecurityTokenServiceDescriptor(reader));
						}
					}
					if (roleDescriptor == null)
					{
						ReadCustomRoleDescriptor(xsi, reader, descriptor);
					}
				}
				else if (reader.IsStartElement("IDPSSODescriptor", Saml2MetadataNs))
				{
					descriptor.RoleDescriptors.Add(
						ReadIdpSsoDescriptor(reader));
				}
				else if (reader.IsStartElement("SPSSODescriptor", Saml2MetadataNs))
				{
					descriptor.RoleDescriptors.Add(ReadSpSsoDescriptor(reader));
				}
				else if (reader.IsStartElement("AuthnAuthorityDescriptor", Saml2MetadataNs))
				{
					descriptor.RoleDescriptors.Add(
						ReadAuthnAuthorityDescriptor(reader));
				}
				else if (reader.IsStartElement("AttributeAuthorityDescriptor", Saml2MetadataNs))
				{
					descriptor.RoleDescriptors.Add(
						ReadAttributeAuthorityDescriptor(reader));
				}
				else if (reader.IsStartElement("PDPDescriptor", Saml2MetadataNs))
				{
					descriptor.RoleDescriptors.Add(
						ReadPDPDescriptor(reader));
				}
				else if (reader.IsStartElement("AffiliationDescriptor", Saml2MetadataNs))
				{
					descriptor.AffiliationDescriptors.Add(
						ReadAffiliationDescriptor(reader));
				}
				else if (reader.IsStartElement("Organization", Saml2MetadataNs))
				{
					descriptor.Organization = ReadOrganization(reader);
				}
				else if (reader.IsStartElement("ContactPerson", Saml2MetadataNs))
				{
					descriptor.Contacts.Add(ReadContactPerson(reader));
				}
				else if (reader.IsStartElement("AdditionalMetadataLocation", Saml2MetadataNs))
				{
					descriptor.AdditionalMetadataLocations.Add(
						ReadAdditionalMetadataLocation(reader));
				}
				else if (reader.IsStartElement("Extensions", Saml2MetadataNs))
				{
					ReadChildrenAsXmlElements(reader, descriptor.Extensions.Add);
				}
				else
				{
					return ReadCustomElement(reader, descriptor);
				}
				return true; // handled above
			});

			// TODO: something with the signature
			return descriptor;
		}

		//  <element name="KeyDescriptor" type="md:KeyDescriptorType"/>
		//  <complexType name="KeyDescriptorType">
		//   <sequence>
		//     <element ref="ds:KeyInfo"/>
		//     <element ref="md:EncryptionMethod" minOccurs="0"
		//      maxOccurs="unbounded"/>
		//   </sequence>
		//   <attribute name="use" type="md:KeyTypes" use="optional"/>
		//  </complexType>
		//  <simpleType name="KeyTypes">
		//   <restriction base="string">
		//     <enumeration value="encryption"/>
		//     <enumeration value="signing"/>
		//   </restriction>
		//  </simpleType>
		//  <element name="EncryptionMethod" type="xenc:EncryptionMethodType"/>
		protected virtual KeyDescriptor ReadKeyDescriptor(XmlReader reader)
		{
			var descriptor = CreateKeyDescriptorInstance();
			string use = reader.GetAttribute("use");
			if (!String.IsNullOrEmpty(use))
			{
				switch (use)
				{
					case "encryption":
						descriptor.Use = KeyType.Encryption;
						break;
					case "signing":
						descriptor.Use = KeyType.Signing;
						break;
					default:
						throw new MetadataSerializationException($"Unrecognised key use attribute {use}");
				}
			}
			ReadCustomAttributes(reader, descriptor);

			ReadChildren(reader, () =>
			{
				if (reader.IsStartElement("KeyInfo", DSigNs))
				{
					descriptor.KeyInfo = ReadDSigKeyInfo(reader);
					// descriptor.KeyIdentifier = MakeSecurityKeyIdentifier(descriptor.KeyInfo);
				}
				else if (reader.IsStartElement("EncryptionMethod", Saml2MetadataNs))
				{
					descriptor.EncryptionMethods.Add(ReadEncryptionMethod(reader));
				}
				else
				{
					return ReadCustomElement(reader, descriptor);
				}
				return true; // handled above
			});

			return descriptor;
		}

		T ReadLocalizedEntry<T>(XmlReader reader, Func<T> createInstance,
			Action<T, string> setContent) where T : LocalizedEntry
		{
			var entry = createInstance();
			string lang = reader.GetAttribute("lang", XmlNs);
			if (String.IsNullOrEmpty(lang))
			{
				throw new MetadataSerializationException("localized element without xml:lang attribute");
			}
			entry.Language = lang;
			ReadCustomAttributes(reader, entry);

			string value = reader.ReadElementContentAsString();
			setContent(entry, value);
			return entry;
		}

		// <complexType name="localizedNameType">
		//   <simpleContent>
		//     <extension base="string">
		//       <attribute ref="xml:lang" use="required"/>
		//     </extension>
		//   </simpleContent>
		// </complexType>
		protected virtual LocalizedName ReadLocalizedName(XmlReader reader) =>
			ReadLocalizedEntry(reader, CreateLocalizedNameInstance, (name, value) => name.Name = value);

		// <complexType name="localizedURIType">
		//   <simpleContent>
		//     <extension base="anyURI">
		//       <attribute ref="xml:lang" use="required"/>
		//     </extension>
		//   </simpleContent>
		// </complexType>
		protected virtual LocalizedUri ReadLocalizedUri(XmlReader reader) =>
			ReadLocalizedEntry(reader, CreateLocalizedUriInstance, (uri, value) => uri.Uri = MakeUri(value));

		// Be loose as sometimes the data is a bit lax (e.g. 'www.foo.com')
		Uri MakeUri(string uri) => new Uri(uri, UriKind.RelativeOrAbsolute);

		public MetadataBase ReadMetadata(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}
			return ReadMetadata(XmlReader.Create(stream));
		}

		public MetadataBase ReadMetadata(XmlReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException(nameof(reader));
			}
			return ReadMetadata(reader, NullSecurityTokenResolver.Instance);
		}

		public MetadataBase ReadMetadata(XmlReader reader, SecurityTokenResolver tokenResolver)
		{
			if (reader == null)
			{
				throw new ArgumentNullException(nameof(reader));
			}
			if (tokenResolver == null)
			{
				throw new ArgumentNullException(nameof(tokenResolver));
			}
			return ReadMetadataCore(reader, tokenResolver);
		}

		protected virtual MetadataBase ReadMetadataCore(XmlReader reader, SecurityTokenResolver tokenResolver)
		{
			if (reader.IsStartElement("EntityDescriptor", Saml2MetadataNs))
			{
				return ReadEntityDescriptor(reader, tokenResolver);
			}
			if (reader.IsStartElement("EntitiesDescriptor", Saml2MetadataNs))
			{
				return ReadEntitiesDescriptor(reader, tokenResolver);
			}
			throw new MetadataSerializationException($"Unexpected root entity {reader.Name}");
		}

		// <element name="Organization" type="md:OrganizationType"/>
		// <complexType name="OrganizationType">
		//   <sequence>
		//     <element ref="md:Extensions" minOccurs="0"/>
		//     <element ref="md:OrganizationName" maxOccurs="unbounded"/>
		//     <element ref="md:OrganizationDisplayName" maxOccurs="unbounded"/>
		//     <element ref="md:OrganizationURL" maxOccurs="unbounded"/>
		//   </sequence>
		//   <anyAttribute namespace="##other" processContents="lax"/>
		// </complexType>
		// <element name="OrganizationName" type="md:localizedNameType"/>
		// <element name="OrganizationDisplayName" type="md:localizedNameType"/>
		// <element name="OrganizationURL" type="md:localizedURIType"/>		protected virtual Organization ReadOrganization(XmlReader reader)
		{
			var org = CreateOrganizationInstance();
			ReadCustomAttributes(reader, org);
			ReadChildren(reader, () =>
			{
				if (reader.IsStartElement("Extensions", Saml2MetadataNs))
				{
					ReadChildrenAsXmlElements(reader, org.Extensions.Add);
				}
				else if (reader.IsStartElement("OrganizationName", Saml2MetadataNs))
				{
					org.Names.Add(ReadLocalizedName(reader));
				}
				else if (reader.IsStartElement("OrganizationDisplayName", Saml2MetadataNs))
				{
					org.DisplayNames.Add(ReadLocalizedName(reader));
				}
				else if (reader.IsStartElement("OrganizationURL", Saml2MetadataNs))
				{
					org.Urls.Add(ReadLocalizedUri(reader));
				}
				else
				{
					return ReadCustomElement(reader, org);
				}
				return true; // handled above
			});
			return org;
		}

		static bool ParseBooleanValue(string v)
		{
			try
			{
				return XmlConvert.ToBoolean(v);
			}
			catch (FormatException e)
			{
				throw new MetadataSerializationException($"Invalid boolean value '{v}'", e);
			}		}

		static bool? GetOptionalBooleanAttribute(XmlReader reader, string att)
		{
			string sv = reader.GetAttribute(att);
			return String.IsNullOrEmpty(sv) ? (bool?)null : ParseBooleanValue(sv);
		}

		static bool GetBooleanAttribute(XmlReader reader, string att, bool def)		{			string sv = reader.GetAttribute(att);
			return String.IsNullOrEmpty(sv) ? def : ParseBooleanValue(sv);
		}
		static void ReadOptionalDateAttribute(XmlReader reader, string attribute, Action<DateTime> dateAction)
		{
			string sv = reader.GetAttribute(attribute);
			if (!String.IsNullOrEmpty(sv))
			{
				DateTime v;
				if (!DateTime.TryParse(sv, out v))
				{
					throw new MetadataSerializationException(
						$"Invalid {attribute} attribute value '{sv}'");
				}
				dateAction(v);
			}
		}

		static void ReadOptionalTimespanAttribute(XmlReader reader, 
			string attribute, Action<TimeSpan> durationAction)
		{
			string sv = reader.GetAttribute(attribute);
			if (!String.IsNullOrEmpty(sv))
			{
				TimeSpan v;
				try
				{
					v = XmlConvert.ToTimeSpan(sv);
				}
				catch (FormatException)
				{
					throw new MetadataSerializationException(
						$"Invalid {attribute} attribute value '{sv}'");
				}
				durationAction(v);
			}
		}

		// <element name="RoleDescriptor" type="md:RoleDescriptorType"/>
		//   <complexType name = "RoleDescriptorType" abstract="true">
		//     <sequence>
		//       <element ref="ds:Signature" minOccurs="0"/>
		//       <element ref="md:Extensions" minOccurs="0"/>
		//       <element ref="md:KeyDescriptor" minOccurs="0" maxOccurs="unbounded"/>
		//       <element ref="md:Organization" minOccurs="0"/>
		//       <element ref="md:ContactPerson" minOccurs="0" maxOccurs="unbounded"/>
		//     </sequence>
		//     <attribute name="ID" type="ID" use="optional"/>
		//     <attribute name="validUntil" type="dateTime" use="optional"/>
		//     <attribute name="cacheDuration" type="duration" use="optional"/>
		//     <attribute name="protocolSupportEnumeration" type="md:anyURIListType"
		//       use="required"/>
		//     <attribute name="errorURL" type="anyURI" use="optional"/>
		//     <anyAttribute namespace="##other" processContents="lax"/>
		//   </complexType>
		//   <simpleType name="anyURIListType">
		//     <list itemType="anyURI"/>
		//   </simpleType>
		protected virtual void ReadRoleDescriptorAttributes(XmlReader reader, RoleDescriptor roleDescriptor)
		{
			if (reader == null)
			{
				throw new ArgumentNullException(nameof(reader));
			}
			if (roleDescriptor == null)
			{
				throw new ArgumentNullException(nameof(roleDescriptor));
			}

			// <attribute name="ID" type="ID" use="optional"/>
			roleDescriptor.Id = GetAttribute(reader, "ID", roleDescriptor.Id);

			// <attribute name="validUntil" type="dateTime" use="optional"/>
			// <attribute name="cacheDuration" type="duration" use="optional"/>
			ReadCachedMetadataAttributes(reader, roleDescriptor);

			// <attribute name="protocolSupportEnumeration" type="md:anyURIListType"
			//   use="required"/>
			string protocols = reader.GetAttribute("protocolSupportEnumeration") ?? "";
			foreach (string protocol in protocols.Split(
				new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
			{
				roleDescriptor.ProtocolsSupported.Add(MakeUri(protocol));
			}
			if (roleDescriptor.ProtocolsSupported.Count == 0)
			{
				throw new MetadataSerializationException("RoleDescriptor element with no protocolSupportEnumeration attribute");
			}

			// <attribute name="errorURL" type="anyURI" use="optional"/>
			string errorUrl = reader.GetAttribute("errorURL");
			if (!String.IsNullOrEmpty(errorUrl))
			{
				roleDescriptor.ErrorUrl = MakeUri(errorUrl);
			}

			// <anyAttribute namespace="##other" processContents="lax"/>
			ReadCustomAttributes(reader, roleDescriptor);
		}

		protected virtual bool ReadRoleDescriptorElement(XmlReader reader, RoleDescriptor roleDescriptor)
		{
			if (reader == null)
			{
				throw new ArgumentNullException(nameof(reader));
			}
			if (roleDescriptor == null)
			{
				throw new ArgumentNullException(nameof(roleDescriptor));
			}

			// TODO: what about signing??
			// <element ref="ds:Signature" minOccurs="0"/>
			if (reader.IsStartElement("Extensions", Saml2MetadataNs))
			{
				ReadChildrenAsXmlElements(reader, roleDescriptor.Extensions.Add);
			}
			// <element ref="md:ContactPerson" minOccurs="0" maxOccurs="unbounded"/>
			else if (reader.IsStartElement("ContactPerson", Saml2MetadataNs))
			{
				roleDescriptor.Contacts.Add(ReadContactPerson(reader));
			}
			// <element ref="md:KeyDescriptor" minOccurs="0" maxOccurs="unbounded"/>
			else if (reader.IsStartElement("KeyDescriptor", Saml2MetadataNs))
			{
				roleDescriptor.Keys.Add(ReadKeyDescriptor(reader));
			}
			// <element ref="md:Organization" minOccurs="0"/>
			else if (reader.IsStartElement("Organization", Saml2MetadataNs))
			{
				if (roleDescriptor.Organization != null)
				{
					throw new MetadataSerializationException("RoleDescriptor element with more than one Organization child");
				}
				roleDescriptor.Organization = ReadOrganization(reader);
			}
			// <element ref="md:Extensions" minOccurs="0"/>
			else
			{
				return ReadCustomElement(reader, roleDescriptor);
			}

			return true; // handled above
		}

		// <complexType name="SecurityTokenServiceType">
		//   <extension base="fed:WebServiceDescriptorType">
		//     <sequence>
		//       <element ref="fed:SecurityTokenServiceEndpoint"
		//                minOccurs="1" maxOccurs="unbounded"/>
		//       <element ref="fed:SingleSignOutSubscriptionEndpoint"
		//                minOccurs="0" maxOccurs="unbounded"/>
		//       <element ref="fed:SingleSignOutNotificationEndpoint"
		//                minOccurs="0" maxOccurs="unbounded"/>
		//       <element ref="fed:PassiveRequestorEndpoint"
		//               minOccurs="0" maxOccurs="unbounded"/>
		//      </sequence>
		//   </extension>
		// </complexType>
		// 
		// <element name="fed:SecurityTokenServiceEndpoint"
		//          type="wsa:EndpointReferenceType"/>
		// <element name="fed:SingleSignOutSubscriptionEndpoint"
		//          type="wsa:EndpointReferenceType"/>
		// <element name="fed:SingleSignOutNotificationEndpoint"  
		//          type="wsa:EndpointReferenceType"/>
		// <element name="fed:PassiveRequestorEndpoint"
		//          type="wsa:EndpointReferenceType"/>
		protected virtual SecurityTokenServiceDescriptor ReadSecurityTokenServiceDescriptor(XmlReader reader)
		{
			var descriptor = CreateSecurityTokenServiceDescriptorInstance();
			ReadCustomAttributes(reader, descriptor);
			ReadChildren(reader, () =>
			{
				if (reader.IsStartElement("SecurityTokenServiceEndpoint", FedNs))
				{
					descriptor.SecurityTokenServiceEndpoints.Add(
						ReadEndpointReference(reader));
				}
				else if (reader.IsStartElement("SingleSignOutSubscriptionEndpoint", FedNs))
				{
					descriptor.SingleSignOutSubscriptionEndpoints.Add(
						ReadEndpointReference(reader));
				}
				else if (reader.IsStartElement("SingleSignOutNotificationEndpoint", FedNs))
				{
					descriptor.SingleSignOutNotificationEndpoints.Add(
						ReadEndpointReference(reader));
				}
				else if (reader.IsStartElement("PassiveRequestorEndpoint", FedNs))
				{
					descriptor.PassiveRequestorEndpoints.Add(
						ReadEndpointReference(reader));
				}
				else
				{
					return ReadCustomElement(reader, descriptor);
				}
				return true;
			});
			return descriptor;
		}

		// <element name="RequestedAttribute" type="md:RequestedAttributeType"/>
		// <complexType name="RequestedAttributeType">
		// <complexContent>
		//   <extension base="saml:AttributeType">
		//     <attribute name="isRequired" type="boolean" use="optional"/>
		//   </extension>
		// </complexContent>
		// </complexType>
		protected virtual RequestedAttribute ReadRequestedAttribute(XmlReader reader)		{			var attribute = ReadSamlAttributeType(reader, CreateRequestedAttributeInstance);			attribute.IsRequired = GetOptionalBooleanAttribute(reader, "isRequired");
			ReadCustomAttributes(reader, attribute);
			return attribute;
		}
		// <element name="AttributeConsumingService"
		//   type="md:AttributeConsumingServiceType"/>
		// <complexType name="AttributeConsumingServiceType">
		//   <sequence>
		//     <element ref="md:ServiceName" maxOccurs="unbounded"/>
		//     <element ref="md:ServiceDescription" minOccurs="0"
		//       maxOccurs="unbounded"/>
		//     <element ref="md:RequestedAttribute" maxOccurs="unbounded"/>
		//   </sequence>
		//   <attribute name="index" type="unsignedShort" use="required"/>
		//   <attribute name="isDefault" type="boolean" use="optional"/>
		// </complexType>
		// <element name="ServiceName" type="md:localizedNameType"/>
		// <element name="ServiceDescription" type="md:localizedNameType"/>
		protected virtual AttributeConsumingService ReadAttributeConsumingService(XmlReader reader)
		{
			var acs = CreateAttributeConsumingServiceInstance();

			ReadIndexedEntryWithDefaultAttributes(reader, acs);

			ReadCustomAttributes(reader, acs);

			ReadChildren(reader, () =>
			{
				if (reader.IsStartElement("ServiceName", Saml2MetadataNs))
				{
					acs.ServiceNames.Add(ReadLocalizedName(reader));
				}
				else if (reader.IsStartElement("ServiceDescription", Saml2MetadataNs))
				{
					acs.ServiceDescriptions.Add(ReadLocalizedName(reader));
				}
				else if (reader.IsStartElement("RequestedAttribute", Saml2MetadataNs))
				{
					acs.RequestedAttributes.Add(ReadRequestedAttribute(reader));
				}
				else
				{
					return ReadCustomElement(reader, acs);
				}
				return true; // handled above
			});
			return acs;
		}

		void AddIndexedEntry<T>(IndexedCollectionWithDefault<T> collection, T entry,
			string elName) where T : class, IIndexedEntryWithDefault
		{
			try
			{
				collection.Add(entry.Index, entry);
			}
			catch (ArgumentException e)
			{
				throw new MetadataSerializationException(
					$"Cannot add the {elName} with index '{entry.Index}' " +
					"because it repeats an index already used", e);
			}
		}
		
		//  <element name="SPSSODescriptor" type="md:SPSSODescriptorType"/>
		//  <complexType name="SPSSODescriptorType">
		//    <complexContent>
		//      <extension base="md:SSODescriptorType">
		//        <sequence>
		//        <element ref="md:AssertionConsumerService"
		//          maxOccurs="unbounded"/>
		//        <element ref="md:AttributeConsumingService" minOccurs="0"
		//          maxOccurs="unbounded"/>
		//        </sequence>
		//        <attribute name="AuthnRequestsSigned" type="boolean"
		//          use="optional"/>
		//        <attribute name="WantAssertionsSigned" type="boolean"
		//          use="optional"/>
		//      </extension>
		//    </complexContent>
		//  </complexType>
		//  <element name="AssertionConsumerService" type="md:IndexedEndpoint"/>
		protected virtual SpSsoDescriptor ReadSpSsoDescriptor(XmlReader reader)
		{
			var descriptor = CreateSpSsoDescriptorInstance();
			descriptor.AuthnRequestsSigned = GetBooleanAttribute(reader, "AuthnRequestsSigned", false);
			descriptor.WantAssertionsSigned = GetBooleanAttribute(reader, "WantAssertionsSigned", false);
			ReadSsoDescriptorAttributes(reader, descriptor);
			ReadCustomAttributes(reader, descriptor);

			ReadChildren(reader, () =>
			{
				if (reader.IsStartElement("AssertionConsumerService", Saml2MetadataNs))
				{
					var acs = ReadAssertionConsumerService(reader);
					AddIndexedEntry(descriptor.AssertionConsumerServices,
						acs, "AssertionConsumerService");
				}
				else if (reader.IsStartElement("AttributeConsumingService", Saml2MetadataNs))
				{
					var acs = ReadAttributeConsumingService(reader);
					AddIndexedEntry(descriptor.AttributeConsumingServices,
						acs, "AttributeConsumingService");
				}
				else if (reader.IsStartElement("Extensions", Saml2MetadataNs))
				{
					var doc = new XmlDocument();
					XmlNode rootNode = doc.CreateElement("root");
					doc.AppendChild(rootNode);

					ReadChildren(reader, () =>
					{
						if (reader.IsStartElement("DiscoveryResponse", IdpDiscNs))
						{
							var disco = ReadDiscoveryResponse(reader);
							descriptor.DiscoveryResponses.Add(disco.Index, disco);
						}
						else
						{
							descriptor.Extensions.Add(ReadCurrentNodeAsXmlElement(reader));
						}
						return true;
					});
				}
				else if (ReadSsoDescriptorElement(reader, descriptor))
				{
				}
				else
				{
					return ReadCustomElement(reader, descriptor);
				}
				return true; // handled above
			});
			return descriptor;
		}

		// <complexType name="SSODescriptorType" abstract="true">
		//   <complexContent>
		//     <extension base="md:RoleDescriptorType">
		//       <sequence>
		//         <element ref="md:ArtifactResolutionService" minOccurs="0"
		//           maxOccurs="unbounded"/>
		//         <element ref="md:SingleLogoutService" minOccurs="0"
		//           maxOccurs="unbounded"/>
		//         <element ref="md:ManageNameIDService" minOccurs="0"
		//           maxOccurs="unbounded"/>
		//         <element ref="md:NameIDFormat" minOccurs="0"
		//           maxOccurs="unbounded"/>
		//       </sequence>
		//     </extension>
		//   </complexContent>
		// </complexType>
		// <element name="ArtifactResolutionService" type="md:IndexedEndpoint"/>
		// <element name="SingleLogoutService" type="md:EndpointType"/>
		// <element name="ManageNameIDService" type="md:EndpointType"/>
		// <element name="NameIDFormat" type="anyURI"/>
		protected virtual void ReadSsoDescriptorAttributes(XmlReader reader, SsoDescriptor descriptor)
		{
			ReadRoleDescriptorAttributes(reader, descriptor);
		}

		protected virtual bool ReadSsoDescriptorElement(XmlReader reader, SsoDescriptor descriptor)
		{
			if (reader.IsStartElement("ArtifactResolutionService", Saml2MetadataNs))
			{
				AddIndexedEntry(descriptor.ArtifactResolutionServices,
					ReadArtifactResolutionService(reader), "ArtifactResolutionService");
			}
			else if (reader.IsStartElement("SingleLogoutService", Saml2MetadataNs))
			{
				descriptor.SingleLogoutServices.Add(
					ReadSingleLogoutService(reader));
			}
			else if (reader.IsStartElement("ManageNameIDService", Saml2MetadataNs))
			{
				descriptor.ManageNameIDServices.Add(
					ReadManageNameIDService(reader));
			}
			else if (reader.IsStartElement("NameIDFormat", Saml2MetadataNs))
			{
				descriptor.NameIdentifierFormats.Add(
					ReadNameIDFormat(reader));
			}
			else
			{
				return ReadRoleDescriptorElement(reader, descriptor);
			}
			return true; // handled above
		}

		Uri GetUriAttribute(XmlReader reader, string att, Uri def)
		{
			string sv = reader.GetAttribute(att);
			return !String.IsNullOrEmpty(sv) ? MakeUri(sv) : def;
		}

		protected virtual void ReadEndpointAttributes(XmlReader reader, Endpoint endpoint)
		{
			endpoint.Binding = GetUriAttribute(reader, "Binding", endpoint.Binding);
			if (endpoint.Binding == null)
			{
				throw new MetadataSerializationException("EndpointType with missing Binding Attribute");
			}

			endpoint.Location = GetUriAttribute(reader, "Location", endpoint.Location);
			if (endpoint.Location == null)
			{
				throw new MetadataSerializationException("EndpointType with missing Location Attribute");
			}

			endpoint.ResponseLocation = GetUriAttribute(reader, "ResponseLocation", endpoint.ResponseLocation);
		}

		static void SkipToElementEnd(XmlReader reader)
		{
			while (reader.IsStartElement())
			{
				reader.Skip();
			}
			reader.ReadEndElement();
		}

		// <complexType name="EndpointType">
		//  <sequence>
		//   <any namespace="##other" processContents="lax" minOccurs="0"
		//     maxOccurs="unbounded"/>
		//  </sequence>
		//  <attribute name="Binding" type="anyURI" use="required"/>
		//  <attribute name="Location" type="anyURI" use="required"/>
		//  <attribute name="ResponseLocation" type="anyURI" use="optional"/>
		//  <anyAttribute namespace="##other" processContents="lax"/>
		// </complexType>
		T ReadWrappedEndpoint<T>(XmlReader reader, Func<T> createInstance) where T : Endpoint
		{
			var endpoint = createInstance();
			ReadEndpointAttributes(reader, endpoint);
			ReadCustomAttributes(reader, endpoint);
			ReadChildren(reader, () => false);
			return endpoint;
		}

		protected virtual Endpoint ReadEndpoint(XmlReader reader) =>
			ReadWrappedEndpoint(reader, CreateEndpointInstance);

		protected virtual AttributeService ReadAttributeService(XmlReader reader) =>
			ReadWrappedEndpoint(reader, CreateAttributeServiceInstance);

		protected virtual AuthnQueryService ReadAuthnQueryService(XmlReader reader) =>
			ReadWrappedEndpoint(reader, CreateAuthnQueryServiceInstance);

		protected virtual AssertionIdRequestService ReadAssertionIdRequestService(XmlReader reader) =>
			ReadWrappedEndpoint(reader, CreateAssertionIdRequestServiceInstance);

		protected virtual AuthzService ReadAuthzService(XmlReader reader) =>
			ReadWrappedEndpoint(reader, CreateAuthzServiceInstance);

		protected virtual SingleLogoutService ReadSingleLogoutService(XmlReader reader) =>
			ReadWrappedEndpoint(reader, CreateSingleLogoutServiceInstance);

		protected virtual ManageNameIDService ReadManageNameIDService(XmlReader reader) =>
			ReadWrappedEndpoint(reader, CreateManageNameIDServiceInstance);

		protected virtual SingleSignOutNotificationEndpoint ReadSingleSignOutNotificationEndpoint(XmlReader reader) =>
			ReadWrappedEndpoint(reader, CreateSingleSignOutNotificationEndpointInstance);

		protected virtual ApplicationServiceEndpoint ReadApplicationServiceEndpoint(XmlReader reader) =>
			ReadWrappedEndpoint(reader, CreateApplicationServiceEndpointInstance);

		protected virtual PassiveRequestorEndpoint ReadPassiveRequestorEndpoint(XmlReader reader) =>
			ReadWrappedEndpoint(reader, CreatePassiveRequestorEndpointInstance);

		protected virtual SingleSignOnService ReadSingleSignOnService(XmlReader reader) =>
			ReadWrappedEndpoint(reader, CreateSingleSignOnServiceInstance);
		
		protected virtual NameIDMappingService ReadNameIDMappingService(XmlReader reader) =>
			ReadWrappedEndpoint(reader, CreateNameIDMappingServiceInstance);

		void ReadIndexedEntryWithDefaultAttributes(XmlReader reader, IIndexedEntryWithDefault entry)
		{
			string sv = reader.GetAttribute("index");
			if (String.IsNullOrEmpty(sv))
			{
				throw new MetadataSerializationException("IndexedEndpoint element missing index attribute");
			}

			int index;
			if (!Int32.TryParse(sv, out index))
			{
				throw new MetadataSerializationException($"IndexedEndpoint element with invalid index attribute '{index}'");
			}
			entry.Index = index;
			entry.IsDefault = GetOptionalBooleanAttribute(reader, "isDefault");
		}

		void ReadIndexedEndpointAttributes(XmlReader reader, IndexedEndpoint endpoint)
		{
			ReadIndexedEntryWithDefaultAttributes(reader, endpoint);
			ReadEndpointAttributes(reader, endpoint);
		}

		// <complexType name="IndexedEndpoint">
		//   <complexContent>
		//     <extension base="md:EndpointType">
		//       <attribute name="index" type="unsignedShort" use="required"/>
		//       <attribute name="isDefault" type="boolean" use="optional"/>
		//     </extension>
		//   </complexContent>
		// </complexType>
		T ReadWrappedIndexedEndpoint<T>(XmlReader reader, Func<T> createInstance) where T : IndexedEndpoint
		{
			var endpoint = createInstance();
			ReadIndexedEndpointAttributes(reader, endpoint);
			ReadCustomAttributes(reader, endpoint);
			ReadChildren(reader, () => false);
			return endpoint;
		}

		protected virtual AssertionConsumerService ReadAssertionConsumerService(XmlReader reader) =>
			ReadWrappedIndexedEndpoint(reader, CreateAssertionConsumerServiceInstance);

		protected virtual ArtifactResolutionService ReadArtifactResolutionService(XmlReader reader) =>
			ReadWrappedIndexedEndpoint(reader, CreateArtifactResolutionServiceInstance);

		protected virtual DiscoveryResponse ReadDiscoveryResponse(XmlReader reader) =>
			ReadWrappedIndexedEndpoint(reader, CreateDiscoveryResponseInstance);

		// <element name="PDPDescriptor" type="md:PDPDescriptorType"/>
		// <complexType name="PDPDescriptorType">
		//   <complexContent>
		//     <extension base="md:RoleDescriptorType">
		//       <sequence>
		//         <element ref="md:AuthzService" maxOccurs="unbounded"/>
		//         <element ref="md:AssertionIDRequestService" minOccurs="0"
		//           maxOccurs="unbounded"/>
		//         <element ref="md:NameIDFormat" minOccurs="0"
		//           maxOccurs="unbounded"/>
		//       </sequence>
		//     </extension>
		//   </complexContent>
		// </complexType>
		// <element name="AuthzService" type="md:EndpointType"/>
		protected virtual PDPDescriptor ReadPDPDescriptor(XmlReader reader)
		{
			var descriptor = CreatePDPDescriptorInstance();
			ReadRoleDescriptorAttributes(reader, descriptor);
			ReadCustomAttributes(reader, descriptor);

			ReadChildren(reader, () =>
			{
				if (reader.IsStartElement("AuthzService", Saml2MetadataNs))
				{
					descriptor.AuthzServices.Add(ReadAuthzService(reader));
				}
				else if (reader.IsStartElement("AssertionIDRequestService", Saml2MetadataNs))
				{
					descriptor.AssertionIdRequestServices.Add(
						ReadAssertionIdRequestService(reader));
				}
				else if (reader.IsStartElement("NameIDFormat", Saml2MetadataNs))
				{
					descriptor.NameIDFormats.Add(ReadNameIDFormat(reader));
				}
				else if (ReadRoleDescriptorElement(reader, descriptor))
				{
				}
				else
				{
					return ReadCustomElement(reader, descriptor);
				}
				return true; // handled above
			});
			return descriptor;
		}

		protected virtual NameIDFormat ReadNameIDFormat(XmlReader reader)
		{
			var nameIDFormat = CreateNameIDFormatInstance();
			string uri = reader.ReadElementContentAsString();
			if (String.IsNullOrEmpty(uri))
			{
				throw new MetadataSerializationException("NameIDFormat element with no uri");
			}
			nameIDFormat.Uri = MakeUri(uri);
			return nameIDFormat;
		}


		// <element name="AuthnAuthorityDescriptor"
		//   type="md:AuthnAuthorityDescriptorType"/>
		//   <complexType name="AuthnAuthorityDescriptorType">
		//     <complexContent>
		//       <extension base="md:RoleDescriptorType">
		//         <sequence>
		//           <element ref="md:AuthnQueryService" maxOccurs="unbounded"/>
		//           <element ref="md:AssertionIDRequestService" minOccurs="0"
		//             maxOccurs="unbounded"/>
		//           <element ref="md:NameIDFormat" minOccurs="0"
		//             maxOccurs="unbounded"/>
		//         </sequence>
		//       </extension>
		//     </complexContent>
		//   </complexType>
		// <element name="AuthnQueryService" type="md:EndpointType"/>
		protected virtual AuthnAuthorityDescriptor ReadAuthnAuthorityDescriptor(XmlReader reader)
		{
			var descriptor = CreateAuthnAuthorityDescriptorInstance();
			ReadRoleDescriptorAttributes(reader, descriptor);
			ReadCustomAttributes(reader, descriptor);

			ReadChildren(reader, () =>
			{
				if (reader.IsStartElement("AuthnQueryService", Saml2MetadataNs))
				{
					descriptor.AuthnQueryServices.Add(ReadAuthnQueryService(reader));
				}
				else if (reader.IsStartElement("AssertionIDRequestService", Saml2MetadataNs))
				{
					descriptor.AssertionIdRequestServices.Add(ReadAssertionIdRequestService(reader));
				}
				else if (reader.IsStartElement("NameIDFormat", Saml2MetadataNs))
				{
					descriptor.NameIDFormats.Add(ReadNameIDFormat(reader));
				}
				else if (ReadRoleDescriptorElement(reader, descriptor))
				{
				}
				else
				{
					return ReadCustomElement(reader, descriptor);
				}
				return true; // handled above
			});
			return descriptor;
		}

		protected virtual AttributeProfile ReadAttributeProfile(XmlReader reader)
		{
			var profile = CreateAttributeProfileInstance();
			ReadCustomAttributes(reader, profile);
			profile.Uri = MakeUri(reader.ReadElementContentAsString());
			return profile;
		}

		// <element name="AttributeAuthorityDescriptor"
		//   type="md:AttributeAuthorityDescriptorType"/>
		//   <complexType name="AttributeAuthorityDescriptorType">
		//     <complexContent>
		//       <extension base="md:RoleDescriptorType">
		//       <sequence>
		//         <element ref="md:AttributeService" maxOccurs="unbounded"/>
		//         <element ref="md:AssertionIDRequestService" minOccurs="0"
		//           maxOccurs="unbounded"/>
		//         <element ref="md:NameIDFormat" minOccurs="0"
		//           maxOccurs="unbounded"/>
		//         <element ref="md:AttributeProfile" minOccurs="0"
		//           maxOccurs="unbounded"/>
		//         <element ref="saml:Attribute" minOccurs="0"
		//           maxOccurs="unbounded"/>
		//         </sequence>
		//       </extension>
		//     </complexContent>
		//   </complexType>
		// <element name="AttributeService" type="md:EndpointType"/>
		protected virtual AttributeAuthorityDescriptor ReadAttributeAuthorityDescriptor(XmlReader reader)
		{
			var descriptor = CreateAttributeAuthorityDescriptorInstance();
			ReadRoleDescriptorAttributes(reader, descriptor);
			ReadCustomAttributes(reader, descriptor);

			ReadChildren(reader, () =>
			{
				if (reader.IsStartElement("AttributeService", Saml2MetadataNs))
				{
					descriptor.AttributeServices.Add(ReadAttributeService(reader));
				}
				else if (reader.IsStartElement("AssertionIDRequestService", Saml2MetadataNs))
				{
					descriptor.AssertionIdRequestServices.Add(ReadAssertionIdRequestService(reader));
				}
				else if (reader.IsStartElement("NameIDFormat", Saml2MetadataNs))
				{
					descriptor.NameIDFormats.Add(ReadNameIDFormat(reader));
				}
				else if (reader.IsStartElement("AttributeProfile", Saml2MetadataNs))
				{
					descriptor.AttributeProfiles.Add(ReadAttributeProfile(reader));
				}
				else if (reader.IsStartElement("Attribute", Saml2AssertionNs))
				{
					descriptor.Attributes.Add(ReadSaml2Attribute(reader));
				}
				else if (ReadRoleDescriptorElement(reader, descriptor))
				{
				}
				else
				{
					return ReadCustomElement(reader, descriptor);
				}
				return true; // handled above
			});

			return descriptor;
		}

		// <complexType name="WebServiceDescriptorType" abstract="true">
		// 	<complexContent>
		//	  <extension base="md:RoleDescriptorType">
		// 	    <sequence>
		// 	      <element ref="fed:LogicalServiceNamesOffered" minOccurs="0" maxOccurs="1" />
		//        <element ref="fed:TokenTypesOffered" minOccurs="0" maxOccurs="1" />
		// 	      <element ref="fed:ClaimDialectsOffered" minOccurs="0" maxOccurs="1" />
		// 	      <element ref="fed:ClaimTypesOffered" minOccurs="0" maxOccurs="1" />
		// 	      <element ref="fed:ClaimTypesRequested" minOccurs="0" maxOccurs="1"/>
		// 	      <element ref="fed:AutomaticPseudonyms" minOccurs="0" maxOccurs="1"/>
		// 	      <element ref="fed:TargetScopes" minOccurs="0" maxOccurs="1"/>
		// 	    </sequence>
		// 	    <attribute name="ServiceDisplayName" type="xs:String" use="optional"/>
		// 	    <attribute name="ServiceDescription" type="xs:String" use="optional"/>
		// 	  </extension>
		// 	</complexContent>
		// </complexType>
		// <element name='LogicalServiceNamesOffered' type=fed:LogicalServiceNamesOfferedType' />
		// <element name="fed:TokenTypeOffered" type="fed:TokenType"/>
		// <element name="fed:ClaimDialectsOffered" type="fed:ClaimDialectsOfferedType"/>
		// <element name="fed:ClaimTypesOffered" type="fed:ClaimTypesOfferedType"/>
		// <element name="ClaimTypesRequested" type="tns:ClaimTypesRequestedType"/>
		// <element name="fed:AutomaticPseudonyms" type="xs:boolean"/>
		// <element name="fed:TargetScope" type="tns:EndpointType"/>
		protected virtual void ReadWebServiceDescriptorAttributes(XmlReader reader, WebServiceDescriptor descriptor)
		{
			if (reader == null)
			{
				throw new ArgumentNullException(nameof(reader));
			}
			if (descriptor == null)
			{
				throw new ArgumentNullException(nameof(descriptor));
			}

			ReadRoleDescriptorAttributes(reader, descriptor);
			descriptor.ServiceDisplayName = GetAttribute(reader,
				"ServiceDisplayName", descriptor.ServiceDisplayName);
			descriptor.ServiceDescription = GetAttribute(reader,
				"ServiceDescription", descriptor.ServiceDescription);
			ReadCustomAttributes(reader, descriptor);
		}

		void ReadUris(XmlReader reader, string parentElementName,
			string childElementName, ICollection<Uri> uris)
		{
			bool empty = reader.IsEmptyElement;
			reader.ReadStartElement();
			if (!empty)
			{
				while (reader.IsStartElement())
				{
					if (!reader.IsStartElement(childElementName, FedNs))
					{
						throw new MetadataSerializationException(
							$"<fed:{parentElementName}> element with unexpected child {reader.Name}");
					}
					string uri = reader.GetAttribute("Uri");
					if (String.IsNullOrEmpty(uri))
					{
						throw new MetadataSerializationException(
							$"<fed:{childElementName}> element without Uri attribute");
					}
					uris.Add(MakeUri(uri));
					reader.Skip();
				}
				reader.ReadEndElement();
			}
		}

		static void ReadChildren(XmlReader reader, Func<bool> childAction)
		{
			bool empty = reader.IsEmptyElement;
			reader.ReadStartElement();
			if (!empty)
			{
				while (reader.IsStartElement())
				{
					if (!childAction())
					{
						reader.Skip();
					}
				}
				reader.ReadEndElement();
			}
		}

		void ReadDisplayClaims(XmlReader reader, ICollection<DisplayClaim> claims)
		{
			ReadChildren(reader, () =>
			{
				if (reader.IsStartElement("ClaimType", AuthNs))
				{
					claims.Add(ReadDisplayClaim(reader));
					return true;
				}
				return false;
			});
		}

		XmlElement ReadCurrentNodeAsXmlElement(XmlReader reader)
		{
			bool empty = reader.IsEmptyElement;
			var doc = new XmlDocument();
			using (var subtreeReader = reader.ReadSubtree())
			{
				doc.Load(subtreeReader);
			}
			if (!empty)
			{
				reader.ReadEndElement();
			}
			else
			{
				reader.Skip();
			}
			return doc.DocumentElement;
		}

		void ReadChildrenAsXmlElements(XmlReader reader, Action<XmlElement> childAction)
		{
			ReadChildren(reader, () =>
			{
				childAction(ReadCurrentNodeAsXmlElement(reader));
				return true;
			});
		}

		EndpointReference ReadEndpointReference(XmlReader reader)
		{
			var endpointReference = new EndpointReference();

			// <wsa:EndpointReference>
			//     <wsa:Address>xs:anyURI</wsa:Address>
			//     <wsa:ReferenceParameters>xs:any*</wsa:ReferenceParameters> ?
			//     <wsa:Metadata>xs:any*</wsa:Metadata>?
			// </wsa:EndpointReference>
			ReadChildren(reader, () =>
			{
				if (reader.IsStartElement("Address", WsaNs))
				{
					if (endpointReference.Uri != null)
					{
						throw new MetadataSerializationException("<wsa:EndpointReference> has more than one <wsa:Address> child");
					}
					endpointReference.Uri = MakeUri(reader.ReadElementContentAsString());
				}
				else if (reader.IsStartElement("ReferenceParameters", WsaNs))
				{
					ReadChildrenAsXmlElements(reader, endpointReference.ReferenceParameters.Add);
				}
				else if (reader.IsStartElement("Metadata", WsaNs))
				{
					ReadChildrenAsXmlElements(reader, endpointReference.Metadata.Add);
				}
				else
				{
					return ReadCustomElement(reader, endpointReference);
				}
				return true; // handled above
			});
			return endpointReference;
		}

		public virtual bool ReadWebServiceDescriptorElement(XmlReader reader, WebServiceDescriptor descriptor)
		{
			if (reader == null)
			{
				throw new ArgumentNullException(nameof(reader));
			}
			if (descriptor == null)
			{
				throw new ArgumentNullException(nameof(descriptor));
			}

			if (ReadRoleDescriptorElement(reader, descriptor))
			{
				return true;
			}
			// <element ref="fed:LogicalServiceNamesOffered" minOccurs="0" maxOccurs="1" />
			if (reader.IsStartElement("LogicalServiceNamesOffered", FedNs))
			{
				// <fed:LogicalServiceNamesOffered ...>
				//	<fed:IssuerName Uri="xs:anyURI" .../> +
				// </fed:LogicalServiceNamesOffered>
				ReadUris(reader, "LogicalServiceNamesOffered",
					"IssuerName", descriptor.LogicalServiceNamesOffered);
			}
			// <element ref="fed:TokenTypesOffered" minOccurs="0" maxOccurs="1" />
			else if (reader.IsStartElement("TokenTypesOffered", FedNs))
			{
				// <fed:TokenTypesOffered ...>
				//   <fed:TokenType Uri="xs:anyURI" ...>
				//	   ...
				//   </fed:TokenType> + 
				//     ...
				// </fed:TokenTypesOffered>
				ReadUris(reader, "TokenTypesOffered",
					"TokenType", descriptor.TokenTypesOffered);
			}
			// <element ref="fed:ClaimDialectsOffered" minOccurs="0" maxOccurs="1" />
			else if (reader.IsStartElement("ClaimDialectsOffered", FedNs))
			{
				// <fed:ClaimDialectsOffered>
				//   <fed:ClaimDialect Uri="xs:anyURI" /> +
				// </fed:ClaimDialectsOffered>			
				ReadUris(reader, "ClaimDialectsOffered",
					"ClaimDialect", descriptor.ClaimDialectsOffered);
			}
			// <element ref="fed:ClaimTypesOffered" minOccurs="0" maxOccurs="1" />
			else if (reader.IsStartElement("ClaimTypesOffered", FedNs))
			{
				// <fed:ClaimTypesOffered ...>
				//   <auth:ClaimType ...> ... </auth:ClaimType> +
				// </fed:ClaimTypesOffered>
				ReadDisplayClaims(reader, descriptor.ClaimTypesOffered);
			}
			// <element ref="fed:ClaimTypesRequested" minOccurs="0" maxOccurs="1"/>
			else if (reader.IsStartElement("ClaimTypesRequested", FedNs))
			{
				// <fed:ClaimTypesRequested ...>
				//   <auth:ClaimType ...> ... </auth:ClaimType> +
				// </fed:ClaimTypesRequested>
				ReadDisplayClaims(reader, descriptor.ClaimTypesRequested);
			}
			// <element ref="fed:AutomaticPseudonyms" minOccurs="0" maxOccurs="1"/>
			else if (reader.IsStartElement("AutomaticPseudonyms", FedNs))
			{
				// <fed:AutomaticPseudonyms>
				//   xs:boolean
				// </fed:AutomaticPseudonyms>
				descriptor.AutomaticPseudonyms = reader.ReadContentAsBoolean();
			}
			// <element ref="fed:TargetScopes" minOccurs="0" maxOccurs="1"/>
			else if (reader.IsStartElement("TargetScopes", FedNs))
			{
				// <fed:TargetScopes ...>
				//   <wsa:EndpointReference>
				//     ...
				//   </wsa:EndpointReference> + 
				// </fed:TargetScopes>
				ReadChildren(reader, () =>
				{
					if (reader.IsStartElement("EndpointReference", WsaNs))
					{
						descriptor.TargetScopes.Add(ReadEndpointReference(reader));
						return true;
					}
					return false;
				});
			}
			else
			{
				return ReadCustomElement(reader, descriptor);
			}
			return true; // handled above
		}

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
