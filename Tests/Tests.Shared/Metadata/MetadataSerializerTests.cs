using FluentAssertions;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sustainsys.Saml2.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Sustainsys.Saml2.Metadata.Descriptors;
using Sustainsys.Saml2.Metadata.Exceptions;
using Sustainsys.Saml2.Metadata.Selectors;
using Sustainsys.Saml2.Metadata.Serialization;
using Sustainsys.Saml2.Metadata.Services;

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

		public XEncEncryptionMethod TestCreateXEncEncryptionMethodInstance() =>
			base.CreateXEncEncryptionMethodInstance();

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

		public ServiceName TestCreateServiceNameInstance() =>
			base.CreateServiceNameInstance();

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

		public XEncEncryptionMethod TestReadXEncEncryptionMethod(XmlReader reader) =>
			base.ReadXEncEncryptionMethod(reader);

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

		public void TestWriteApplicationServiceDescriptor(XmlWriter writer,
			ApplicationServiceDescriptor appService) =>
			base.WriteApplicationServiceDescriptor(writer, appService);

		public void TestWriteContactPerson(XmlWriter writer, ContactPerson contactPerson) =>
			base.WriteContactPerson(writer, contactPerson);

		public void TestWriteEndpoint(XmlWriter writer, Endpoint endpoint,
			string name, string ns) =>
			base.WriteEndpoint(writer, endpoint, name, ns);

		public void TestWriteIndexedEndpoint(XmlWriter writer, IndexedEndpoint endpoint,
			string name, string ns) =>
			base.WriteIndexedEndpoint(writer, endpoint, name, ns);

		public void TestWriteEncryptionMethod(XmlWriter writer, EncryptionMethod method) =>
			base.WriteEncryptionMethod(writer, method);

		public void TestWriteXEncEncryptionMethod(XmlWriter writer, XEncEncryptionMethod method) =>
			base.WriteXEncEncryptionMethod(writer, method);

		public void TestWriteRSAKeyValue(XmlWriter writer, RsaKeyValue rsa) =>
			base.WriteRSAKeyValue(writer, rsa);

		public void TestWriteDSAKeyValue(XmlWriter writer, DsaKeyValue dsa) =>
			base.WriteDSAKeyValue(writer, dsa);

		public void TestWriteECKeyValue(XmlWriter writer, EcKeyValue ec) =>
			base.WriteECKeyValue(writer, ec);

		public void TestWriteKeyValue(XmlWriter writer, KeyValue keyValue) =>
			base.WriteKeyValue(writer, keyValue);

		public void TestWriteX509IssuerSerial(XmlWriter writer, X509IssuerSerial issuerSerial) =>
			base.WriteX509IssuerSerial(writer, issuerSerial);

		public void TestWriteX509Digest(XmlWriter writer, X509Digest digest) =>
			base.WriteX509Digest(writer, digest);

		public void TestWriteX509Data(XmlWriter writer, X509Data data) =>
			base.WriteX509Data(writer, data);

		public void TestWriteKeyData(XmlWriter writer, KeyData keyData) =>
			base.WriteKeyData(writer, keyData);

		public void TestWriteDSigKeyInfo(XmlWriter writer, DSigKeyInfo keyInfo) =>
			base.WriteDSigKeyInfo(writer, keyInfo);

		public void TestWriteCipherReference(XmlWriter writer, CipherReference reference) =>
			base.WriteCipherReference(writer, reference);

		public void TestWriteCipherData(XmlWriter writer, CipherData data) =>
			base.WriteCipherData(writer, data);

		public void TestWriteEncryptionProperty(XmlWriter writer, EncryptionProperty property) =>
			base.WriteEncryptionProperty(writer, property);

		public void TestWriteEncryptionProperties(XmlWriter writer, EncryptionProperties properties) =>
			base.WriteEncryptionProperties(writer, properties);

		public void TestWriteEncryptedData(XmlWriter writer, EncryptedData data) =>
			base.WriteEncryptedData(writer, data);

		public void TestWriteEncryptedValue(XmlWriter writer, EncryptedValue value) =>
			base.WriteEncryptedValue(writer, value);

		public void TestWriteClaimValue(XmlWriter writer, ClaimValue value) =>
			base.WriteClaimValue(writer, value);

		public void TestWriteConstrainedValue(XmlWriter writer, ConstrainedValue value) =>
			base.WriteConstrainedValue(writer, value);

		public void TestWriteDisplayClaim(XmlWriter writer, DisplayClaim claim) =>
			base.WriteDisplayClaim(writer, claim);

		public void TestWriteEntitiesDescriptor(XmlWriter writer, EntitiesDescriptor entitiesDescriptor) =>
			base.WriteEntitiesDescriptor(writer, entitiesDescriptor);

		public void TestWriteNameIDFormat(XmlWriter writer, NameIDFormat nameIDFormat) =>
			base.WriteNameIDFormat(writer, nameIDFormat);

		public void TestWriteAuthnAuthorityDescriptor(XmlWriter writer, AuthnAuthorityDescriptor descriptor) =>
			base.WriteAuthnAuthorityDescriptor(writer, descriptor);

		public void TestWriteAttributeProfile(XmlWriter writer, AttributeProfile profile) =>
			base.WriteAttributeProfile(writer, profile);

		public void TestWriteAttributeAuthorityDescriptor(XmlWriter writer, AttributeAuthorityDescriptor descriptor) =>
			base.WriteAttributeAuthorityDescriptor(writer, descriptor);

		public void TestWritePDPDescriptor(XmlWriter writer, PDPDescriptor descriptor) =>
			base.WritePDPDescriptor(writer, descriptor);

		public void TestWriteAffiliationDescriptor(XmlWriter writer, AffiliationDescriptor descriptor) =>
			base.WriteAffiliationDescriptor(writer, descriptor);

		public void TestWriteAdditionalMetadataLocation(XmlWriter writer, AdditionalMetadataLocation location) =>
			base.WriteAdditionalMetadataLocation(writer, location);

		public void TestWriteEntityDescriptor(XmlWriter writer, EntityDescriptor entityDescriptor) =>
			base.WriteEntityDescriptor(writer, entityDescriptor);

		public void TestWriteIdpSsoDescriptor(XmlWriter writer, IdpSsoDescriptor descriptor) =>
			base.WriteIdpSsoDescriptor(writer, descriptor);

		public void TestWriteKeyDescriptor(XmlWriter writer, KeyDescriptor keyDescriptor) =>
			base.WriteKeyDescriptor(writer, keyDescriptor);

		public void TestWriteLocalizedName(XmlWriter writer, LocalizedName name,
			string elName, string ns) =>
			base.WriteLocalizedName(writer, name, elName, ns);

		public void TestWriteLocalizedUri(XmlWriter writer, LocalizedUri uri,
			string name, string ns) =>
			base.WriteLocalizedUri(writer, uri, name, ns);

		public void TestWriteMetadataCore(XmlWriter writer, MetadataBase metadata) =>
			base.WriteMetadataCore(writer, metadata);

		public void TestWriteOrganization(XmlWriter writer, Organization organization) =>
			base.WriteOrganization(writer, organization);

		public void TestWriteRoleDescriptorAttributes(XmlWriter writer, RoleDescriptor descriptor) =>
			base.WriteRoleDescriptorAttributes(writer, descriptor);

		public void TestWriteRoleDescriptorElements(XmlWriter writer, RoleDescriptor descriptor) =>
			base.WriteRoleDescriptorElements(writer, descriptor);

		public void TestWriteSecurityTokenServiceDescriptor(XmlWriter writer,
			SecurityTokenServiceDescriptor descriptor) =>
			base.WriteSecurityTokenServiceDescriptor(writer, descriptor);

		public void TestWriteRequestedAttribute(XmlWriter writer, RequestedAttribute attribute) =>
			base.WriteRequestedAttribute(writer, attribute);

		public void TestWriteAttributeConsumingService(XmlWriter writer, AttributeConsumingService service) =>
			base.WriteAttributeConsumingService(writer, service);

		public void TestWriteSpSsoDescriptor(XmlWriter writer, SpSsoDescriptor descriptor) =>
			base.WriteSpSsoDescriptor(writer, descriptor);

		public void TestWriteSsoDescriptorAttributes(XmlWriter writer, SsoDescriptor descriptor) =>
			base.WriteSsoDescriptorAttributes(writer, descriptor);

		public void TestWriteSsoDescriptorElements(XmlWriter writer, SsoDescriptor descriptor) =>
			base.WriteSsoDescriptorElements(writer, descriptor);

		public void TestWriteWebServiceDescriptorAttributes(XmlWriter writer, WebServiceDescriptor descriptor) =>
			base.WriteWebServiceDescriptorAttributes(writer, descriptor);

		public void TestWriteWebServiceDescriptorElements(XmlWriter writer, WebServiceDescriptor descriptor) =>
			base.WriteWebServiceDescriptorElements(writer, descriptor);

		public void TestWriteAttribute(XmlWriter writer, Saml2Attribute attribute) =>
			base.WriteAttribute(writer, attribute);

		public void TestWriteRetrievalMethod(XmlWriter writer, RetrievalMethod method) =>
			base.WriteRetrievalMethod(writer, method);

		public ArtifactResolutionService TestReadArtifactResolutionService(XmlReader reader) =>
			base.ReadArtifactResolutionService(reader);

		public void TestReadWebServiceDescriptorAttributes(XmlReader reader, WebServiceDescriptor descriptor) =>
			base.ReadWebServiceDescriptorAttributes(reader, descriptor);

		public bool TestReadWebServiceDescriptorElement(XmlReader reader, WebServiceDescriptor descriptor) =>
			base.ReadWebServiceDescriptorElement(reader, descriptor);
		
		public ServiceName TestReadServiceName(XmlReader reader) =>
			base.ReadServiceName(reader);

		public void TestWriteServiceName(XmlWriter writer, ServiceName serviceName) =>
			base.WriteServiceName(writer, serviceName);
	}

	class XmlTestData
	{
		public string Xml { get; private set; }
		public string WrittenXml { get; private set; }
		public object ObjectModel { get; private set; }

		public XmlTestData(string xml, string writtenXml, object obj)
		{
			WrittenXml = writtenXml ?? xml;
			Xml = xml;
			ObjectModel = obj;
		}

		public XmlTestData(string xml, object obj) :
			this(xml, null, obj)
		{
		}
	}

	sealed class TestData : Attribute
	{
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
				+tZ9KynmrbJpTSi0+BM=", "");

		static Dictionary<string, XmlTestData> s_testData
			= new Dictionary<string, XmlTestData>();

		static MetadataSerializerTests()
		{
			var methods = typeof(MetadataSerializerTests).GetMethods(
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
				.Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(TestData)));
			foreach (var method in methods)
			{
				method.Invoke(null, new object[] { });
			}
		}

		static XmlTestData GetTestData(string testName)
		{
			XmlTestData testData;
			if (!s_testData.TryGetValue(testName, out testData))
			{
				throw new KeyNotFoundException(
					$"The test data named {testName} could not be found");
			}
			return testData;
		}

		static string TrimXml(string xml)
		{
			using (var stringReader = new StringReader(xml))
			using (var reader = XmlReader.Create(stringReader))
			using (var memoryStream = new MemoryStream())
			using (var writer = XmlWriter.Create(memoryStream))
			{
				while (reader.Read())
				{
					switch (reader.NodeType)
					{
						case XmlNodeType.Element:
							writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
							writer.WriteAttributes(reader, true);
							if (reader.IsEmptyElement)
							{
								writer.WriteEndElement();
							}
							break;
						case XmlNodeType.Text:
							writer.WriteString(reader.Value.Trim());
							break;
						case XmlNodeType.Comment:
						case XmlNodeType.SignificantWhitespace:
						case XmlNodeType.Whitespace:
							// yum
							break;
						case XmlNodeType.CDATA:
							writer.WriteCData(reader.Value);
							break;
						case XmlNodeType.EntityReference:
							writer.WriteEntityRef(reader.Value);
							break;
						case XmlNodeType.XmlDeclaration:
						case XmlNodeType.ProcessingInstruction:
							writer.WriteProcessingInstruction(reader.Name, reader.Value);
							break;
						case XmlNodeType.DocumentType:
							writer.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"),
								reader.GetAttribute("SYSTEM"), reader.Value);
							break;
						case XmlNodeType.EndElement:
							writer.WriteEndElement();
							break;
					}
				}
				writer.Close();
				memoryStream.Position = 0;
				using (var streamReader = new StreamReader(memoryStream))
				{
					return streamReader.ReadToEnd();
				}
			}
		}

		static void AddTestData(string name, string xml, object obj)
		{
			s_testData.Add(name, new XmlTestData(xml, obj));
		}

		static void AddTestData(string name, string xml, string writtenXml, object obj)
		{
			s_testData.Add(name, new XmlTestData(xml, writtenXml, obj));
		}

		static void AddTestData(string name, string xml, object obj, bool trimXml)
		{
			AddTestData(name, xml, TrimXml(xml), obj);
		}

		static XmlNamespaceManager CreateNamespaceManager(XmlDocument doc)
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

		static (XmlDocument doc, XmlNamespaceManager nsmgr) LoadXml(string xml)
		{
			var doc = new XmlDocument();
			doc.LoadXml(xml);
			var nsmgr = CreateNamespaceManager(doc);
			return (doc, nsmgr);
		}

		void ReadTestThrow<TException>(string xml, Action<TestMetadataSerializer, XmlReader> readFn)
			where TException : Exception
		{
			using (var stringReader = new StringReader(xml))
			using (var xmlReader = XmlReader.Create(stringReader))
			{
				xmlReader.MoveToContent();
				var serializer = new TestMetadataSerializer();
				Action a = () => readFn(serializer, xmlReader);
				a.Should().Throw<TException>();
			}
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

		void ReadTest<T>(string testName, Func<TestMetadataSerializer, XmlReader, T> readFn,
			Func<FluentAssertions.Equivalency.EquivalencyAssertionOptions<T>,
				 FluentAssertions.Equivalency.EquivalencyAssertionOptions<T>> config = null)
		{
			var testData = GetTestData(testName);
			T expected = testData.ObjectModel.As<T>();
			ReadTest(testData.Xml, expected, readFn, config);
		}

		void WriteTest<T>(T obj, string expected, Action<TestMetadataSerializer, XmlWriter> writeFn)
		{
			using (var ms = new MemoryStream())
			{
				using (var xmlWriter = XmlWriter.Create(ms))
				{
					var serializer = new TestMetadataSerializer();
					writeFn(serializer, xmlWriter);
				}

				ms.Position = 0;
				XmlDocument resultDoc = new XmlDocument();
				resultDoc.Load(ms);

				XmlDocument expectedDoc = new XmlDocument();
				expectedDoc.LoadXml(expected);

				resultDoc.Should().BeEquivalentTo(expectedDoc);
			}
		}

		void WriteTest<T>(string testName, Action<TestMetadataSerializer, XmlWriter, T> writeFn)
		{
			var testData = GetTestData(testName);
			T obj = testData.ObjectModel.As<T>();
			WriteTest(obj, testData.WrittenXml,
				(serializer, writer) => writeFn(serializer, writer, obj));
		}

		static void ReadNullTest(Action<TestMetadataSerializer> test)
		{
			Action a = () => test(new TestMetadataSerializer());
			a.Should().Throw<ArgumentNullException>();
		}

		static void WriteNullTest(Action<TestMetadataSerializer, XmlWriter> test)
		{
			using (var ms = new MemoryStream())
			using (XmlWriter xw = XmlWriter.Create(ms))
			{
				Action a = () => test(new TestMetadataSerializer(), xw);
				a.Should().Throw<ArgumentNullException>();
			}
		}

		static void WriteTestThrow<T, TException>(string testName, Action<TestMetadataSerializer, XmlWriter, T> writeFn)
			where TException : Exception
		{
			var testData = GetTestData(testName);
			T obj = testData.ObjectModel.As<T>();

			using (var ms = new MemoryStream())
			using (var xmlWriter = XmlWriter.Create(ms))
			{
				var serializer = new TestMetadataSerializer();
				Action a = () => writeFn(serializer, xmlWriter, obj);
				a.Should().Throw<TException>();
			}
		}

		static void WriteNullWriterTest<T>(string testName, Action<TestMetadataSerializer, T> test)
		{
			WriteTestThrow<T, ArgumentNullException>(
				testName, (serializer, writer, obj) => test(serializer, obj));
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
			serializer.TestCreateXEncEncryptionMethodInstance()
				.Should().BeOfType<XEncEncryptionMethod>();
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
			serializer.TestCreateServiceNameInstance()
				.Should().BeOfType<ServiceName>();

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
		public void MetadataSerializerTests_ReadEndpointReferenceNull()
		{
			ReadNullTest(serializer => serializer.TestReadEndpointReference(null));
		}

		[TestData]
		public static void AddApplicationServiceDescriptorTestData()
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

			var obj = new ApplicationServiceDescriptor()
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

			AddTestData("ApplicationServiceDescriptor1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadApplicationServiceDescriptor()
		{
			ReadTest("ApplicationServiceDescriptor1", (serializer, reader) =>
				serializer.TestReadApplicationServiceDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadApplicationServiceDescriptorNull()
		{
			ReadNullTest(serializer =>
				serializer.TestReadApplicationServiceDescriptor(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteApplicationServiceDescriptorNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteApplicationServiceDescriptor(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteApplicationServiceDescriptorNullWriter()
		{
			WriteNullWriterTest<ApplicationServiceDescriptor>("ApplicationServiceDescriptor1",
				(serializer, obj) =>
					serializer.TestWriteApplicationServiceDescriptor(null, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteApplicationServiceDescriptor()
		{
			WriteTest<ApplicationServiceDescriptor>("ApplicationServiceDescriptor1",
				(serializer, writer, descriptor) =>
					serializer.TestWriteApplicationServiceDescriptor(writer, descriptor));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadApplicationServiceDescriptorNoEndpoints()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<md:RoleDescriptor
				protocolSupportEnumeration='http://docs.oasis-open.org/wsfed/federation/200706'
				xsi:type='fed:ApplicationServiceType'
				xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
				xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'
				xmlns:fed='http://docs.oasis-open.org/wsfed/federation/200706'
				xmlns:wsa='http://www.w3.org/2005/08/addressing'/>";
			ReadTestThrow<MetadataSerializationException>(xml, (serializer, reader) =>
				serializer.TestReadApplicationServiceDescriptor(reader));
		}

		[TestData]
		public static void AddContactPersonTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<md:ContactPerson contactType='technical' xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'>
				<md:Extensions>
					<any-extension-element/>
					<any-other-element/>
				</md:Extensions>
				<md:Company>Test Company</md:Company>
				<md:GivenName>David</md:GivenName>
				<md:SurName>Test</md:SurName>
				<md:EmailAddress>david.test@test.company</md:EmailAddress>
				<md:EmailAddress>david.test2@test.company</md:EmailAddress>
				<md:TelephoneNumber>0123456789</md:TelephoneNumber>
				<md:TelephoneNumber>9876543210</md:TelephoneNumber>
			</md:ContactPerson>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new ContactPerson()
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

			AddTestData("ContactPerson1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadContactPerson()
		{
			ReadTest("ContactPerson1", (serializer, reader) =>
				serializer.TestReadContactPerson(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteContactPerson()
		{
			WriteTest<ContactPerson>("ContactPerson1", (serializer, writer, expected) =>
				serializer.TestWriteContactPerson(writer, expected));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadContactPersonNull()
		{
			ReadNullTest(serializer => serializer.TestReadContactPerson(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteContactPersonNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteContactPerson(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteContactPersonWriterNull()
		{
			WriteNullWriterTest<ContactPerson>("ContactPerson1",
				(serializer, obj) =>
					serializer.TestWriteContactPerson(null, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadContactPersonInvalidType()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<md:ContactPerson contactType='INVALID' xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'/>";
			ReadTestThrow<MetadataSerializationException>(xml, (serializer, reader) =>
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

		[TestData]
		public static void AddIdpSsoDescriptorTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			  <md:IDPSSODescriptor
				xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
				xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion'
				WantAuthnRequestsSigned='true'
				protocolSupportEnumeration='urn:oasis:names:tc:SAML:2.0:protocol'
				cacheDuration='P2Y6M5DT12H35M30S'
				validUntil='2020-01-01T14:32:31Z'
				errorURL='http://idp.example.com/something/went/wrong'
				ID='yourGUIDhere'>
				<md:Extensions>
					<extra-idp-sso-stuff/>
				</md:Extensions>
			    <md:KeyDescriptor use='signing'>
			      <ds:KeyInfo xmlns:ds='http://www.w3.org/2000/09/xmldsig#'/>
			    </md:KeyDescriptor>
				<md:Organization>
					<md:Extensions>
						<ext-elt/>
					</md:Extensions>
					<md:OrganizationName xml:lang='en'>Acme Ltd</md:OrganizationName>
					<md:OrganizationDisplayName xml:lang='en'>Acme Ltd (display)</md:OrganizationDisplayName>
					<md:OrganizationURL xml:lang='en'>http://acme.co/</md:OrganizationURL>
				</md:Organization>
				<md:ContactPerson contactType='administrative' xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'>
					<md:Extensions>
						<time-for-tea/>
						<and-biscuits/>
					</md:Extensions>
					<md:Company>Acme Ltd</md:Company>
					<md:GivenName>Wile E</md:GivenName>
					<md:SurName>Coyote</md:SurName>
					<md:EmailAddress>wile.e.coyto@acme.co</md:EmailAddress>
					<md:TelephoneNumber>11223344</md:TelephoneNumber>
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

			var obj = new IdpSsoDescriptor()
			{
				WantAuthnRequestsSigned = true,
				ProtocolsSupported = { new Uri("urn:oasis:names:tc:SAML:2.0:protocol") },
				CacheDuration = new XsdDuration(years: 2, months: 6, days: 5, hours: 12, minutes: 35, seconds: 30),
				ValidUntil = new DateTime(2020, 01, 01, 14, 32, 31, DateTimeKind.Utc),
				ErrorUrl = new Uri("http://idp.example.com/something/went/wrong"),
				Id = "yourGUIDhere",
				Extensions = {
					doc.SelectSingleNode("/md:IDPSSODescriptor/md:Extensions/*[1]",
						nsmgr).As<XmlElement>()
				},
				Organization = new Organization()
				{
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

			AddTestData("IdpSsoDescriptor1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadIdpSsoDescriptor()
		{
			ReadTest("IdpSsoDescriptor1", (serializer, reader) =>
				serializer.TestReadIdpSsoDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteIdpSsoDescriptor()
		{
			WriteTest<IdpSsoDescriptor>("IdpSsoDescriptor1",
				(serializer, writer, obj) =>
					serializer.TestWriteIdpSsoDescriptor(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadIdpSsoDescriptorNull()
		{
			ReadNullTest((serializer) => serializer.TestReadIdpSsoDescriptor(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteIdpSsoDescriptorNull()
		{
			WriteNullTest((serializer, writer) => 
				serializer.TestWriteIdpSsoDescriptor(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteIdpSsoDescriptorWriterNull()
		{
			WriteNullWriterTest<IdpSsoDescriptor>("IdpSsoDescriptor1",
				(serializer, obj) =>
					serializer.TestWriteIdpSsoDescriptor(null, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadIdpSsoDescriptorTwoOrganizations()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			  <md:IDPSSODescriptor
				xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
				WantAuthnRequestsSigned='true'
				protocolSupportEnumeration='urn:oasis:names:tc:SAML:2.0:protocol'>
				<md:Organization>
					<md:OrganizationName xml:lang='en'>Acme Ltd</md:OrganizationName>
					<md:OrganizationDisplayName xml:lang='en'>Acme Ltd (display)</md:OrganizationDisplayName>
					<md:OrganizationURL xml:lang='en'>http://acme.co/</md:OrganizationURL>
				</md:Organization>
				<md:Organization>
					<md:OrganizationName xml:lang='en'>Acme Ltd2</md:OrganizationName>
				</md:Organization>
			  </md:IDPSSODescriptor>";
			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadIdpSsoDescriptor(reader));
		}
		
		[TestMethod]
		public void MetadataSerializerTests_ReadIdpSsoDescriptorDuplicateIndexes()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			  <md:IDPSSODescriptor
				xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
				protocolSupportEnumeration='urn:oasis:names:tc:SAML:2.0:protocol'>
				<md:ArtifactResolutionService
					index='1'
					isDefault='false'
					Binding='http://idp.example.com/ars1'
					Location='http://idp.example.com/arsloc1'
					ResponseLocation='http://idp.example.com/arsresp1' />
				<md:ArtifactResolutionService
					index='1'
					isDefault='false'
					Binding='http://idp.example.com/ars2'
					Location='http://idp.example.com/arsloc2'
					ResponseLocation='http://idp.example.com/arsresp2' />
			  </md:IDPSSODescriptor>";
			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadIdpSsoDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadArtificatResolutionServiceInvalidIndex()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<md:ArtifactResolutionService
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					index='INVALID'
					isDefault='false'
					Binding='http://idp.example.com/ars1'
					Location='http://idp.example.com/arsloc1'
					ResponseLocation='http://idp.example.com/arsresp1' />";
			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadArtifactResolutionService(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAssertionIdRequestServiceMissingBinding()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<md:AssertionIDRequestService
				xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
				Location='http://idp.example.com/ssolocation'
				ResponseLocation='http://idp.example.com/ssoresponselocation' />";
			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadIdpSsoDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAssertionIdRequestServiceMissingLocation()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<md:AssertionIDRequestService
				xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
				Binding='http://idp.example.com/ssobinding'
				ResponseLocation='http://idp.example.com/ssoresponselocation' />";
			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadIdpSsoDescriptor(reader));
		}

		[TestData]
		public static void AddAssertionConsumerServiceTestData()
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

			var obj = new AssertionConsumerService()
			{
				Index = 150,
				IsDefault = false,
				Binding = new Uri("http://idp.example.com/acs1"),
				Location = new Uri("http://idp.example.com/acsloc1"),
				ResponseLocation = new Uri("http://idp.example.com/acsresp1")
			};

			AddTestData("AssertionConsumerService1", xml, obj, true);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAssertionConsumerService()
		{
			ReadTest("AssertionConsumerService1", (serializer, reader) =>
				serializer.TestReadAssertionConsumerService(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteAssertionConsumerService()
		{
			WriteTest<AssertionConsumerService>("AssertionConsumerService1",
				(serializer, writer, obj) =>
					serializer.TestWriteIndexedEndpoint(writer, obj,
						"AssertionConsumerService", "urn:oasis:names:tc:SAML:2.0:metadata"));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAssertionConsumerServiceNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadAssertionConsumerService(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteAssertionConsumerServiceNull()
		{
			WriteNullTest((serializer, writer) =>
					serializer.TestWriteIndexedEndpoint(writer, null,
						"AssertionConsumerService", "urn:oasis:names:tc:SAML:2.0:metadata"));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteAssertionConsumerServiceWriterNull()
		{
			WriteNullWriterTest<AssertionConsumerService>("AssertionConsumerService1",
				(serializer, obj) =>
					serializer.TestWriteIndexedEndpoint(null, obj,
						"AssertionConsumerService", "urn:oasis:names:tc:SAML:2.0:metadata"));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteAssertionConsumerServiceNullName()
		{
			WriteTestThrow<AssertionConsumerService, ArgumentNullException>(
				"AssertionConsumerService1", (serializer, writer, obj) =>
					serializer.TestWriteIndexedEndpoint(writer, obj,
						null, "urn:oasis:names:tc:SAML:2.0:metadata"));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteAssertionConsumerServiceNullNs()
		{
			WriteTestThrow<AssertionConsumerService, ArgumentNullException>(
				"AssertionConsumerService1", (serializer, writer, obj) =>
					serializer.TestWriteIndexedEndpoint(writer, obj,
						"AssertionConsumerService", null));
		}

		[TestData]
		public static void AddDSAKeyValueTestData()
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

			var obj = new DsaKeyValue(
				new DSAParameters
				{
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

			AddTestData("DSAKeyValue1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadDSAKeyValue_MissingP()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<DSAKeyValue xmlns='http://www.w3.org/2000/09/xmldsig#'>
				<Q>li7dzDacuo67Jg7mtqEm2TRuOMU=</Q>
				<G>Z4Rxsnqc9E7pGknFFH2xqaryRPBaQ01khpMdLRQnG541Awtx/XPaF5Bpsy4pNWMOHCBiNU0NogpsQW5QvnlMpA==</G>
				<Y>qV38IqrWJG0V/mZQvRVi1OHw9Zj84nDC4jO8P0axi1gb6d+475yhMjSc/BrIVC58W3ydbkK+Ri4OKbaRZlYeRA==</Y>
				<J>qV38IqrWJG0V/mZQvRVi1OHw9Zj84nDC4jO8P0axi1gb6d+475yhMjSc/BrIVC58W3ydbkK+Ri4OKbaRZlYeRA==</J>
				<Seed>qV38IqrWJG0V/mZQvRVi1OHw9Zj84nDC4jO8P0axi1gb6d+475yhMjSc/BrIVC58W3ydbkK+Ri4OKbaRZlYeRA==</Seed>
				<PgenCounter>pxXTng==</PgenCounter>
			</DSAKeyValue>";
			ReadTestThrow<MetadataSerializationException>(xml, (serializer, reader) =>
				serializer.TestReadDsaKeyValue(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadDSAKeyValue()
		{
			ReadTest("DSAKeyValue1", (serializer, reader) =>
				serializer.TestReadDsaKeyValue(reader),
				opts => opts.ComparingByMembers<DSAParameters>());
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteDSAKeyValue()
		{
			WriteTest<DsaKeyValue>("DSAKeyValue1",
				(serializer, writer, obj) =>
					serializer.TestWriteDSAKeyValue(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadDSAKeyValueNull()
		{
			ReadNullTest((serializer) => serializer.TestReadDsaKeyValue(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteDSAKeyValueNull()
		{
			WriteNullTest((serializer, writer) =>
					serializer.TestWriteDSAKeyValue(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteDSAKeyValueWriterNull()
		{
			WriteNullWriterTest<DsaKeyValue>("DSAKeyValue1",
				(serializer, obj) =>
					serializer.TestWriteDSAKeyValue(null, obj));
		}

		[TestData]
		public static void AddRSAKeyValueTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<RSAKeyValue xmlns='http://www.w3.org/2000/09/xmldsig#'>
				  <Modulus>xA7SEU+e0yQH5rm9kbCDN9o3aPIo7HbP7tX6WOocLZAtNfyxSZDU16ksL6WjubafOqNEpcwR3RdFsT7bCqnXPBe5ELh5u4VEy19MzxkXRgrMvavzyBpVRgBUwUlV5foK5hhmbktQhyNdy/6LpQRhDUDsTvK+g9Ucj47es9AQJ3U=</Modulus>
				  <Exponent>AQAB</Exponent>
				</RSAKeyValue>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new RsaKeyValue(
				new RSAParameters
				{
					Modulus = Convert.FromBase64String("xA7SEU+e0yQH5rm9kbCDN9o3aPIo7HbP7tX6WOocLZAtNfyxSZDU16ksL6WjubafOqNEpcwR3RdFsT7bCqnXPBe5ELh5u4VEy19MzxkXRgrMvavzyBpVRgBUwUlV5foK5hhmbktQhyNdy/6LpQRhDUDsTvK+g9Ucj47es9AQJ3U="),
					Exponent = Convert.FromBase64String("AQAB")
				}
			);

			AddTestData("RSAKeyValue1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadRSAKeyValue()
		{
			ReadTest("RSAKeyValue1", (serializer, reader) =>
				serializer.TestReadRsaKeyValue(reader),
				opts => opts.ComparingByMembers<RSAParameters>());
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteRSAKeyValue()
		{
			WriteTest<RsaKeyValue>("RSAKeyValue1",
				(serializer, writer, obj) =>
					serializer.TestWriteRSAKeyValue(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadRSAKeyValueNull()
		{
			ReadNullTest((serializer) => serializer.TestReadRsaKeyValue(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteRSAKeyValueNull()
		{
			WriteNullTest((serializer, writer) =>
					serializer.TestWriteRSAKeyValue(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteRSAKeyValueWriterNull()
		{
			WriteNullWriterTest<RsaKeyValue>("RSAKeyValue1",
				(serializer, obj) =>
					serializer.TestWriteRSAKeyValue(null, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadRSAKeyValueMissingModulus()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<RSAKeyValue xmlns='http://www.w3.org/2000/09/xmldsig#'>
				  <Exponent>AQAB</Exponent>
				</RSAKeyValue>";
			ReadTestThrow<MetadataSerializationException>(xml, (serializer, reader) =>
				serializer.TestReadRsaKeyValue(reader));
		}

		[TestData]
		public static void AddECKeyValueTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<ECKeyValue xmlns='http://www.w3.org/2009/xmldsig11#'>
				  <NamedCurve URI='urn:oid:1.2.840.10045.3.1.7' />
				  <PublicKey>BOVKaiLPKEDChhkA64UEBOXTv/VFHnhrUPN+bXqCvEl7rroAYpH5tKzbiGTtMSlp4JO9Pxg44zeX7EoWDvOrpD0=</PublicKey>
				</ECKeyValue>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new EcKeyValue(
				new ECParameters
				{
					Curve = ECCurve.CreateFromOid(new Oid("1.2.840.10045.3.1.7")),
					Q = new ECPoint
					{
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

			AddTestData("ECKeyValue1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadECKeyValue()
		{
			ReadTest("ECKeyValue1", (serializer, reader) =>
				serializer.TestReadEcKeyValue(reader),
				opts => opts.ComparingByMembers<ECParameters>()
							.ComparingByMembers<ECCurve>()
							.ComparingByMembers<ECPoint>()
							.ComparingByMembers<Oid>()
							.Excluding(ctx => ctx.Parameters.Curve.Oid.FriendlyName)
							.WithTracing());
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteECKeyValue()
		{
			WriteTest<EcKeyValue>("ECKeyValue1",
				(serializer, writer, obj) =>
					serializer.TestWriteECKeyValue(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadECKeyValueNull()
		{
			ReadNullTest((serializer) => serializer.TestReadEcKeyValue(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteECKeyValueNull()
		{
			WriteNullTest((serializer, writer) =>
					serializer.TestWriteECKeyValue(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteECKeyValueNullWriter()
		{
			WriteNullWriterTest<EcKeyValue>("ECKeyValue1",
				(serializer, obj) =>
					serializer.TestWriteECKeyValue(null, obj));
		}

		[TestData]
		public static void AddECDSAKeyValueTestData()
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

			var obj = new EcKeyValue(
				new ECParameters
				{
					Curve = ECCurve.CreateFromOid(new Oid("1.2.840.10045.3.1.7")),
					Q = new ECPoint
					{
						X = BigInteger.Parse("58511060653801744393249179046482833320204931884267326155134056258624064349885").ToByteArray(),
						Y = BigInteger.Parse("102403352136827775240910267217779508359028642524881540878079119895764161434936").ToByteArray()
					}
				}
			);

			AddTestData("ECDSAKeyValue1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadECDSAKeyValue()
		{
			ReadTest("ECDSAKeyValue1", (serializer, reader) =>
				serializer.TestReadEcDsaKeyValue(reader),
				opts => opts.ComparingByMembers<ECParameters>()
							.ComparingByMembers<ECCurve>()
							.ComparingByMembers<ECPoint>()
							.ComparingByMembers<Oid>()
							.Excluding(ctx => ctx.Parameters.Curve.Oid.FriendlyName)
							.WithTracing());
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadECDSAKeyValueNull()
		{
			ReadNullTest((serializer) => serializer.TestReadEcDsaKeyValue(null));
		}

		[TestData]
		public static void AddKeyValueTestData1()
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

			var obj = new RsaKeyValue(
				new RSAParameters
				{
					Modulus = Convert.FromBase64String("xA7SEU+e0yQH5rm9kbCDN9o3aPIo7HbP7tX6WOocLZAtNfyxSZDU16ksL6WjubafOqNEpcwR3RdFsT7bCqnXPBe5ELh5u4VEy19MzxkXRgrMvavzyBpVRgBUwUlV5foK5hhmbktQhyNdy/6LpQRhDUDsTvK+g9Ucj47es9AQJ3U="),
					Exponent = Convert.FromBase64String("AQAB")
				}
			);

			AddTestData("KeyValueTestData1", xml, obj);
		}

		[TestData]
		public static void AddKeyValueTestData2()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<KeyValue xmlns='http://www.w3.org/2000/09/xmldsig#'>
					<ECKeyValue xmlns='http://www.w3.org/2009/xmldsig11#'>
					  <NamedCurve URI='urn:oid:1.2.840.10045.3.1.7' />
					  <PublicKey>BOVKaiLPKEDChhkA64UEBOXTv/VFHnhrUPN+bXqCvEl7rroAYpH5tKzbiGTtMSlp4JO9Pxg44zeX7EoWDvOrpD0=</PublicKey>
					</ECKeyValue>
				</KeyValue>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new EcKeyValue(
				new ECParameters
				{
					Curve = ECCurve.CreateFromOid(new Oid("1.2.840.10045.3.1.7")),
					Q = new ECPoint
					{
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

			AddTestData("KeyValueTestData2", xml, obj);
		}

		[TestData]
		public static void AddKeyValueTestData3()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<KeyValue xmlns='http://www.w3.org/2000/09/xmldsig#'>
					<ECDSAKeyValue xmlns='http://www.w3.org/2001/04/xmldsig-more#'>
					  <DomainParameters>
						<NamedCurve URN='urn:oid:1.2.840.10045.3.1.7' />
					  </DomainParameters>
					  <PublicKey>
						<X Value='58511060653801744393249179046482833320204931884267326155134056258624064349885' />
						<Y Value='102403352136827775240910267217779508359028642524881540878079119895764161434936' />
					  </PublicKey>
					</ECDSAKeyValue>
				</KeyValue>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new EcKeyValue(
				new ECParameters
				{
					Curve = ECCurve.CreateFromOid(new Oid("1.2.840.10045.3.1.7")),
					Q = new ECPoint
					{
						X = BigInteger.Parse("58511060653801744393249179046482833320204931884267326155134056258624064349885").ToByteArray(),
						Y = BigInteger.Parse("102403352136827775240910267217779508359028642524881540878079119895764161434936").ToByteArray()
					}
				}
			);

			AddTestData("KeyValueTestData3", xml, obj);
		}

		[TestData]
		public static void AddKeyValueTestData4()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<KeyValue xmlns='http://www.w3.org/2000/09/xmldsig#'>
					<DSAKeyValue xmlns='http://www.w3.org/2000/09/xmldsig#'>
						<P>/KaCzo4Syrom78z3EQ5SbbB4sF7ey80etKII864WF64B81uRpH5t9jQTxeEu0ImbzRMqzVDZkVG9xD7nN1kuFw==</P>
						<Q>li7dzDacuo67Jg7mtqEm2TRuOMU=</Q>
						<G>Z4Rxsnqc9E7pGknFFH2xqaryRPBaQ01khpMdLRQnG541Awtx/XPaF5Bpsy4pNWMOHCBiNU0NogpsQW5QvnlMpA==</G>
						<Y>qV38IqrWJG0V/mZQvRVi1OHw9Zj84nDC4jO8P0axi1gb6d+475yhMjSc/BrIVC58W3ydbkK+Ri4OKbaRZlYeRA==</Y>
						<J>qV38IqrWJG0V/mZQvRVi1OHw9Zj84nDC4jO8P0axi1gb6d+475yhMjSc/BrIVC58W3ydbkK+Ri4OKbaRZlYeRA==</J>
						<Seed>qV38IqrWJG0V/mZQvRVi1OHw9Zj84nDC4jO8P0axi1gb6d+475yhMjSc/BrIVC58W3ydbkK+Ri4OKbaRZlYeRA==</Seed>
						<PgenCounter>pxXTng==</PgenCounter>
					</DSAKeyValue>
				</KeyValue>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new DsaKeyValue(
				new DSAParameters
				{
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

			AddTestData("KeyValueTestData4", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadKeyValue1()
		{
			ReadTest("KeyValueTestData1", (serializer, reader) =>
				serializer.TestReadKeyValue(reader),
				opts => opts.ComparingByMembers<RSAParameters>()
							.RespectingRuntimeTypes());
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadKeyValue2()
		{
			ReadTest("KeyValueTestData2", (serializer, reader) =>
				serializer.TestReadKeyValue(reader),
				opts => opts.ComparingByMembers<ECParameters>()
							.ComparingByMembers<ECCurve>()
							.ComparingByMembers<ECPoint>()
							.ComparingByMembers<Oid>()
							.Excluding(ctx => ((EcKeyValue)ctx).Parameters.Curve.Oid.FriendlyName)
							.RespectingRuntimeTypes());
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadKeyValue3()
		{
			ReadTest("KeyValueTestData3", (serializer, reader) =>
				serializer.TestReadKeyValue(reader),
				opts => opts.ComparingByMembers<ECParameters>()
							.ComparingByMembers<ECCurve>()
							.ComparingByMembers<ECPoint>()
							.ComparingByMembers<Oid>()
							.Excluding(ctx => ((EcKeyValue)ctx).Parameters.Curve.Oid.FriendlyName)
							.RespectingRuntimeTypes());
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadKeyValue4()
		{
			ReadTest("KeyValueTestData4", (serializer, reader) =>
				serializer.TestReadKeyValue(reader),
				opts => opts.ComparingByMembers<DSAParameters>()
							.RespectingRuntimeTypes());
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteKeyValue()
		{
			WriteTest<KeyValue>("KeyValueTestData1",
				(serializer, writer, obj) =>
					serializer.TestWriteKeyValue(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadKeyValueNull()
		{
			ReadNullTest((serializer) => serializer.TestReadKeyValue(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteKeyValueNull()
		{
			WriteNullTest((serializer, writer) =>
					serializer.TestWriteKeyValue(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteKeyValueWriter()
		{
			WriteNullWriterTest<KeyValue>("KeyValueTestData1",
				(serializer, obj) =>
					serializer.TestWriteKeyValue(null, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadKeyValueNoChildren()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<KeyValue xmlns='http://www.w3.org/2000/09/xmldsig#'/>";
			ReadTestThrow<MetadataSerializationException>(xml, (serializer, reader) =>
				serializer.TestReadKeyValue(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadKeyValueUnknownChild()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<KeyValue xmlns='http://www.w3.org/2000/09/xmldsig#'><WHAT/></KeyValue>";
			ReadTestThrow<MetadataSerializationException>(xml, (serializer, reader) =>
				serializer.TestReadKeyValue(reader));
		}

		[TestData]
		public static void AddRetrievalMethodTestData()
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

			var obj = new RetrievalMethod
			{
				Uri = new Uri("http://idp.example.com/signingCert.cer"),
				Type = new Uri("http://idp.example.com/x509certtype"),
				Transforms = {
					doc.SelectSingleNode("/ds:RetrievalMethod/ds:Transforms/*[1]",
						nsmgr).As<XmlElement>(),
					doc.SelectSingleNode("/ds:RetrievalMethod/ds:Transforms/*[2]",
						nsmgr).As<XmlElement>()
				}
			};

			AddTestData("RetrievalMethod1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadRetrievalMethod()
		{
			ReadTest("RetrievalMethod1", (serializer, reader) =>
				serializer.TestReadRetrievalMethod(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteRetrievalMethod()
		{
			WriteTest<RetrievalMethod>("RetrievalMethod1",
				(serializer, writer, obj) =>
					serializer.TestWriteRetrievalMethod(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadRetrievalMethodNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadRetrievalMethod(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteRetrievalMethodNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteRetrievalMethod(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteRetrievalMethodWriterNull()
		{
			WriteNullWriterTest<RetrievalMethod>("RetrievalMethod1",
				(serializer, obj) =>
					serializer.TestWriteRetrievalMethod(null, obj));
		}

		[TestData]
		public static void AddX509IssuerSerialTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<X509IssuerSerial xmlns='http://www.w3.org/2000/09/xmldsig#'>
					<X509IssuerName>Test Issuer</X509IssuerName>
					<X509SerialNumber>128976</X509SerialNumber>
				</X509IssuerSerial>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new X509IssuerSerial("Test Issuer", "128976");

			AddTestData("X509IssuerSerial1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadX509IssuerSerial()
		{
			ReadTest("X509IssuerSerial1", (serializer, reader) =>
				serializer.TestReadX509IssuerSerial(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteX509IssuerSerial()
		{
			WriteTest<X509IssuerSerial>("X509IssuerSerial1",
				(serializer, writer, obj) =>
					serializer.TestWriteX509IssuerSerial(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadX509IssuerSerialMissingIssuerName()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<X509IssuerSerial xmlns='http://www.w3.org/2000/09/xmldsig#'>
					<X509SerialNumber>128976</X509SerialNumber>
				</X509IssuerSerial>";
			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadX509IssuerSerial(reader));
		}

		[TestData]
		public static void AddX509DigestTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<X509Digest xmlns='http://www.w3.org/2000/09/xmldsig#' Algorithm='http://w3c.org/madeUpAlgorithm'>BOVKaiLPKEDChhkA64UEBOXTv/VFHnhrUPN+bXqCvEl7rroAYpH5tKzbiGTtMSlp4JO9Pxg44zeX7EoWDvOrpD0=</X509Digest>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new X509Digest
			{
				Algorithm = new Uri("http://w3c.org/madeUpAlgorithm"),
				Value = Convert.FromBase64String("BOVKaiLPKEDChhkA64UEBOXTv/VFHnhrUPN+bXqCvEl7rroAYpH5tKzbiGTtMSlp4JO9Pxg44zeX7EoWDvOrpD0=")
			};

			AddTestData("X509Digest1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadX509Digest()
		{
			ReadTest("X509Digest1", (serializer, reader) =>
				serializer.TestReadX509Digest(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadX509DigestMissingAlgorithm()
		{
			string xml =
				@"<?xml version='1.0' encoding='UTF-8'?>
				<X509Digest xmlns='http://www.w3.org/2000/09/xmldsig#'>BOVKaiLPKEDChhkA64UEBOXTv/VFHnhrUPN+bXqCvEl7rroAYpH5tKzbiGTtMSlp4JO9Pxg44zeX7EoWDvOrpD0=</X509Digest>";
			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadX509Digest(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteX509Digest()
		{
			WriteTest<X509Digest>("X509Digest1",
				(serializer, writer, obj) =>
					serializer.TestWriteX509Digest(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadX509DigestNull()
		{
			ReadNullTest((serializer) => serializer.TestReadX509Digest(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteX509DigestNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteX509Digest(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteX509DigestWriterNull()
		{
			WriteNullWriterTest<X509Digest>("X509Digest1",
				(serializer, obj) =>
					serializer.TestWriteX509Digest(null, obj));
		}

		[TestData]
		public static void AddX509DataTestData()
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

			var obj = new X509Data
			{
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

			AddTestData("X509Data1", xml, obj, true);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadX509Data()
		{
			ReadTest("X509Data1", (serializer, reader) =>
				serializer.TestReadX509Data(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteX509Data()
		{
			WriteTest<X509Data>("X509Data1",
				(serializer, writer, obj) =>
					serializer.TestWriteX509Data(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadX509DataNull()
		{
			ReadNullTest((serializer) => serializer.TestReadX509Data(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteX509DataNull()
		{
			WriteNullTest((serializer, writer) =>
					serializer.TestWriteX509Data(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteX509DataWriterNull()
		{
			WriteNullWriterTest<X509Data>("X509Data1",
				(serializer, obj) =>
					serializer.TestWriteX509Data(null, obj));
		}


		[TestData]
		public static void AddDSigKeyInfoTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<KeyInfo xmlns='http://www.w3.org/2000/09/xmldsig#' Id='testId'>
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
				<X509Data>
					<X509IssuerSerial> 
					  <X509IssuerName>
						C=JP, ST=Tokyo, L=Chuo-ku, O=Frank4DD, OU=WebCert Support, CN=Frank4DD Web CA/emailAddress=support@frank4dd.com
					  </X509IssuerName>
					  <X509SerialNumber>3580</X509SerialNumber>
					</X509IssuerSerial>
				</X509Data>
			</KeyInfo>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new DSigKeyInfo
			{
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

			AddTestData("DSigKeyInfo1", xml, obj, true);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadDSigKeyInfo()
		{
			ReadTest("DSigKeyInfo1", (serializer, reader) =>
				serializer.TestReadDSigKeyInfo(reader),
				opts => opts.ComparingByMembers<RSAParameters>());
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteDSigKeyInfo()
		{
			WriteTest<DSigKeyInfo>("DSigKeyInfo1",
				(serializer, writer, obj) =>
					serializer.TestWriteDSigKeyInfo(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadDSigKeyInfoNull()
		{
			ReadNullTest((serializer) => serializer.TestReadDSigKeyInfo(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteDSigKeyInfoNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteDSigKeyInfo(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteDSigKeyInfoWriterNull()
		{
			WriteNullWriterTest<DSigKeyInfo>("DSigKeyInfo1",
				(serializer, obj) =>
					serializer.TestWriteDSigKeyInfo(null, obj));
		}

		[TestData]
		public static void AddXEncEncryptionMethodTestData()
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

			var obj = new XEncEncryptionMethod
			{
				OAEPparams = Convert.FromBase64String("9lWu3Q=="),
				Algorithm = new Uri("http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p"),
				KeySize = 2048
			};

			string writtenXml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<EncryptionMethod xmlns='http://www.w3.org/2001/04/xmlenc#'
				xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
				Algorithm='http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p'>
				<KeySize>2048</KeySize>
				<OAEPparams>9lWu3Q==</OAEPparams>
			</EncryptionMethod>";

			AddTestData("XEncEncryptionMethod1", xml, writtenXml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadXEncEncryptionMethod()
		{
			ReadTest("XEncEncryptionMethod1", (serializer, reader) =>
				serializer.TestReadXEncEncryptionMethod(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteXEncEncryptionMethod()
		{
			WriteTest<XEncEncryptionMethod>("XEncEncryptionMethod1",
				(serializer, writer, obj) =>
					serializer.TestWriteXEncEncryptionMethod(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadXEncEncryptionMethodNull()
		{
			ReadNullTest((serializer) => serializer.TestReadXEncEncryptionMethod(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteXEncEncryptionMethodNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteXEncEncryptionMethod(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteXEncEncryptionMethodWriterNull()
		{
			WriteNullWriterTest<XEncEncryptionMethod>("XEncEncryptionMethod1",
				(serializer, obj) =>
					serializer.TestWriteXEncEncryptionMethod(null, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadXEncEncryptionMethodInvalidKeySize()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<EncryptionMethod xmlns='http://www.w3.org/2001/04/xmlenc#'
				xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
				Algorithm='http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p'>
				<KeySize>LARGE HORSE</KeySize>
				<OAEPparams> 9lWu3Q== </OAEPparams>
				<ds:DigestMethod Algorithm='http://www.w3.org/2000/09/xmldsig#sha1'/>
			</EncryptionMethod>";
			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadXEncEncryptionMethod(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadXEncEncryptionMethodMissingAlgorithm()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<EncryptionMethod xmlns='http://www.w3.org/2001/04/xmlenc#' />";
			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadXEncEncryptionMethod(reader));
		}

		[TestData]
		public static void AddEncryptionMethodTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<md:EncryptionMethod
				xmlns:xenc='http://www.w3.org/2001/04/xmlenc#'
				xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
				xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
				Algorithm='http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p'>
				<xenc:KeySize>4096</xenc:KeySize>
				<xenc:OAEPparams>9lWu3Q==</xenc:OAEPparams>
			</md:EncryptionMethod>";

			var obj = new EncryptionMethod
			{
				OAEPparams = Convert.FromBase64String("9lWu3Q=="),
				Algorithm = new Uri("http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p"),
				KeySize = 4096
			};

			AddTestData("EncryptionMethod1", xml, obj, true);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEncryptionMethod()
		{
			ReadTest("EncryptionMethod1", (serializer, reader) =>
				serializer.TestReadEncryptionMethod(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEncryptionMethod()
		{
			WriteTest<EncryptionMethod>("EncryptionMethod1",
				(serializer, writer, obj) =>
					serializer.TestWriteEncryptionMethod(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEncryptionMethodNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadEncryptionMethod(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEncryptionMethodNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteEncryptionMethod(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEncryptionMethodWriterNull()
		{
			WriteNullWriterTest<EncryptionMethod>("EncryptionMethod1",
				(serializer, obj) =>
					serializer.TestWriteEncryptionMethod(null, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEncryptionMethodMissingAlgorithm()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<md:EncryptionMethod xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'/>";

			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadEncryptionMethod(reader));
		}

		[TestData]
		public static void AddReadCipherReferenceTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			  <CipherReference xmlns='http://www.w3.org/2001/04/xmlenc#'
				xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
				URI='http://www.example.com/CipherValues.xml'>
				<Transforms>
				  <ds:Transform 
				   Algorithm='http://www.w3.org/TR/1999/REC-xpath-19991116'>
					  <ds:XPath xmlns:rep='http://www.example.org/repository'>self::text()[parent::rep:CipherValue[@Id='example1']]</ds:XPath>
				  </ds:Transform>
				  <ds:Transform Algorithm='http://www.w3.org/2000/09/xmldsig#base64'/>
				</Transforms>
			  </CipherReference>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new CipherReference
			{
				Uri = new Uri("http://www.example.com/CipherValues.xml"),
				Transforms = {
					doc.SelectSingleNode("/xenc:CipherReference/xenc:Transforms/*[1]",
						nsmgr).As<XmlElement>(),
					doc.SelectSingleNode("/xenc:CipherReference/xenc:Transforms/*[2]",
						nsmgr).As<XmlElement>()
				}
			};

			AddTestData("CipherReference1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadCipherReference()
		{
			ReadTest("CipherReference1", (serializer, reader) =>
				serializer.TestReadCipherReference(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteCipherReference()
		{
			WriteTest<CipherReference>("CipherReference1",
				(serializer, writer, obj) =>
					serializer.TestWriteCipherReference(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadCipherReferenceNull()
		{
			ReadNullTest((serializer) => serializer.TestReadCipherReference(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteCipherReferenceNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteCipherReference(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteCipherReferenceWriterNull()
		{
			WriteNullWriterTest<CipherReference>("CipherReference1",
				(serializer, obj) =>
					serializer.TestWriteCipherReference(null, obj));
		}

		[TestData]
		public static void AddCipherDataTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			  <CipherData xmlns='http://www.w3.org/2001/04/xmlenc#'>
				<CipherValue>DEADBEEF</CipherValue>
			    <CipherReference URI='http://www.example.com/CipherValues.xml'/>
			  </CipherData>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new CipherData
			{
				CipherValue = "DEADBEEF",
				CipherReference = new CipherReference()
				{
					Uri = new Uri("http://www.example.com/CipherValues.xml"),
				}
			};

			AddTestData("CipherData1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadCipherData()
		{
			ReadTest("CipherData1", (serializer, reader) =>
				serializer.TestReadCipherData(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteCipherData()
		{
			WriteTest<CipherData>("CipherData1",
				(serializer, writer, obj) =>
					serializer.TestWriteCipherData(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadCipherDataNull()
		{
			ReadNullTest((serializer) => serializer.TestReadCipherData(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteCipherDataNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteCipherData(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteCipherDataWriterNull()
		{
			WriteNullWriterTest<CipherData>("CipherData1",
				(serializer, obj) =>
					serializer.TestWriteCipherData(null, obj));
		}

		[TestData]
		public static void AddEncryptionPropertyTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			  <EncryptionProperty xmlns='http://www.w3.org/2001/04/xmlenc#'
				Target='http://enc.org/target'
				Id='someId'>
				<AnythingYouLike/>
			  </EncryptionProperty>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new EncryptionProperty
			{
				Target = new Uri("http://enc.org/target"),
				Id = "someId"
			};

			string writtenXml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			  <EncryptionProperty xmlns='http://www.w3.org/2001/04/xmlenc#'
				Target='http://enc.org/target'
				Id='someId'/>";

			AddTestData("EncryptionProperty1", xml, writtenXml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEncryptionProperty()
		{
			ReadTest("EncryptionProperty1", (serializer, reader) =>
				serializer.TestReadEncryptionProperty(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEncryptionProperty()
		{
			WriteTest<EncryptionProperty>("EncryptionProperty1",
				(serializer, writer, obj) =>
					serializer.TestWriteEncryptionProperty(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEncryptionPropertyNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadEncryptionProperty(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEncryptionPropertyNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteEncryptionProperty(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEncryptionPropertyWriterNull()
		{
			WriteNullWriterTest<EncryptionProperty>("EncryptionProperty1",
				(serializer, obj) =>
					serializer.TestWriteEncryptionProperty(null, obj));
		}

		[TestData]
		public static void AddEncryptionPropertiesTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<EncryptionProperties Id='anId' xmlns='http://www.w3.org/2001/04/xmlenc#'>
				  <EncryptionProperty
					Target='http://enc.org/target'
					Id='someId'>
					<AnythingYouLike/>
				  </EncryptionProperty>
			</EncryptionProperties>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new EncryptionProperties
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

			string writtenXml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<EncryptionProperties Id='anId' xmlns='http://www.w3.org/2001/04/xmlenc#'>
				  <EncryptionProperty
					Target='http://enc.org/target'
					Id='someId' />
			</EncryptionProperties>";

			AddTestData("EncryptionProperties1", xml, writtenXml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEncryptionProperties()
		{
			ReadTest("EncryptionProperties1", (serializer, reader) =>
				serializer.TestReadEncryptionProperties(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEncryptionProperties()
		{
			WriteTest<EncryptionProperties>("EncryptionProperties1",
				(serializer, writer, obj) =>
					serializer.TestWriteEncryptionProperties(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEncryptionPropertiesNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadEncryptionProperties(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEncryptionPropertiesNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteEncryptionProperties(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEncryptionPropertiesWriterNull()
		{
			WriteNullWriterTest<EncryptionProperties>("EncryptionProperties1",
				(serializer, obj) =>
					serializer.TestWriteEncryptionProperties(null, obj));
		}

		[TestData]
		public static void AddEncryptedDataTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<EncryptedData
				xmlns='http://www.w3.org/2001/04/xmlenc#'
				xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
				Id='ENCDATAID'
				MimeType='text/plain'
				Encoding='https://encoding.org/'
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
					  </EncryptionProperty>
				</EncryptionProperties>
			</EncryptedData>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new EncryptedData
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
				EncryptionMethod = new XEncEncryptionMethod
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

			string writtenXml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<EncryptedData
				xmlns='http://www.w3.org/2001/04/xmlenc#'
				xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
				Id='ENCDATAID'
				MimeType='text/plain'
				Encoding='https://encoding.org/'
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
						Id='someId2' />
				</EncryptionProperties>
			</EncryptedData>";

			AddTestData("EncryptedData1", xml, writtenXml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEncryptedData()
		{
			ReadTest("EncryptedData1", (serializer, reader) =>
				serializer.TestReadEncryptedData(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEncryptedData()
		{
			WriteTest<EncryptedData>("EncryptedData1",
				(serializer, writer, obj) =>
					serializer.TestWriteEncryptedData(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEncryptedDataNull()
		{
			ReadNullTest((serializer) => serializer.TestReadEncryptedData(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEncryptedDataNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteEncryptedData(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEncryptedDataWriterNull()
		{
			WriteNullWriterTest<EncryptedData>("EncryptedData1",
				(serializer, obj) =>
					serializer.TestWriteEncryptedData(null, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEncryptedDataNoCipherData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<EncryptedData
				xmlns='http://www.w3.org/2001/04/xmlenc#'
				xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
				Id='ENCDATAID'
				MimeType='text/plain'
				Encoding='https://encoding.org/'
				Type='http://www.w3.org/2001/04/xmlenc#Element'>
				<EncryptionMethod
					Algorithm='http://www.w3.org/2001/04/xmlenc#tripledes-cbc'/>
				<ds:KeyInfo xmlns:ds='http://www.w3.org/2000/09/xmldsig#'>
					<ds:KeyName>John Smith</ds:KeyName>
				</ds:KeyInfo>
				<EncryptionProperties Id='anId2'>
					  <EncryptionProperty xmlns='http://www.w3.org/2001/04/xmlenc#'
						Target='http://enc.org/target2'
						Id='someId2' />
				</EncryptionProperties>
			</EncryptedData>";

			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadEncryptedData(reader));
		}

		[TestData]
		public static void AddEncryptedValueTestData()
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

			var obj = new EncryptedValue
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
					EncryptionMethod = new XEncEncryptionMethod
					{
						Algorithm = new Uri("http://www.w3.org/2001/04/xmlenc#tripledes-cbc")
					}
				}
			};

			AddTestData("EncryptedValue1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEncryptedValue()
		{
			ReadTest("EncryptedValue1", (serializer, reader) =>
				serializer.TestReadEncryptedValue(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEncryptedValue()
		{
			WriteTest<EncryptedValue>("EncryptedValue1",
				(serializer, writer, obj) =>
					serializer.TestWriteEncryptedValue(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEncryptedValueNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadEncryptedValue(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEncryptedValueNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteEncryptedValue(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEncryptedValueWriterNull()
		{
			WriteNullWriterTest<EncryptedValue>("EncryptedValue1",
				(serializer, obj) =>
					serializer.TestWriteEncryptedValue(null, obj));
		}

		[TestData]
		public static void AddClaimValueTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<auth:ValueLessThan
				xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
				<auth:Value>Some Value</auth:Value>
			</auth:ValueLessThan>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new ClaimValue
			{
				Value = "Some Value"
			};

			string writtenXml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<auth:Value xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>Some Value</auth:Value>";

			AddTestData("ClaimValue1", xml, writtenXml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadClaimValue_Value()
		{
			ReadTest("ClaimValue1", (serializer, reader) =>
				serializer.TestReadClaimValue(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteClaimValue()
		{
			WriteTest<ClaimValue>("ClaimValue1",
				(serializer, writer, obj) =>
					serializer.TestWriteClaimValue(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadClaimValueNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadClaimValue(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteClaimValueNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteClaimValue(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteClaimValueWriterNull()
		{
			WriteNullWriterTest<ClaimValue>("ClaimValue1",
				(serializer, obj) =>
					serializer.TestWriteClaimValue(null, obj));
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

		[TestData]
		public static void AddConstrainedValueTestData()
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

			var obj = new ConstrainedValue
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

			AddTestData("ConstrainedValue1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadConstainedValue()
		{
			ReadTest("ConstrainedValue1", (serializer, reader) =>
				serializer.TestReadConstrainedValue(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteConstainedValue()
		{
			WriteTest<ConstrainedValue>("ConstrainedValue1",
				(serializer, writer, obj) =>
					serializer.TestWriteConstrainedValue(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadConstainedValueNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadConstrainedValue(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteConstainedValueNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteConstrainedValue(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteConstainedValueWriterNull()
		{
			WriteNullWriterTest<ConstrainedValue>("ConstrainedValue1",
				(serializer, obj) =>
					serializer.TestWriteConstrainedValue(null, obj));
		}

		[TestData]
		public static void AddDisplayClaimTestData()
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

			var obj = new DisplayClaim("https://saml.claim.type/")
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
						EncryptionMethod = new XEncEncryptionMethod
						{
							Algorithm = new Uri("http://www.w3.org/2001/04/xmlenc#tripledes-cbc")
						}
					}
				}
			};

			AddTestData("DisplayClaim1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadDisplayClaim()
		{
			ReadTest("DisplayClaim1", (serializer, reader) =>
				serializer.TestReadDisplayClaim(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteDisplayClaim()
		{
			WriteTest<DisplayClaim>("DisplayClaim1",
				(serializer, writer, obj) =>
					serializer.TestWriteDisplayClaim(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadDisplayClaimNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadDisplayClaim(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteDisplayClaimNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteDisplayClaim(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteDisplayClaimWriterNull()
		{
			WriteNullWriterTest<DisplayClaim>("DisplayClaim1",
				(serializer, obj) =>
					serializer.TestWriteDisplayClaim(null, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadDisplayClaimMissingUri()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<auth:ClaimType Optional='false'
				xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706' />";

			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadDisplayClaim(reader));
		}

		[TestData]
		public static void AddEntitiesDescriptorTestData()
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

			var obj = new EntitiesDescriptor
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

			AddTestData("EntitiesDescriptor1", xml, obj, true);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEntitiesDescriptor()
		{
			ReadTest("EntitiesDescriptor1", (serializer, reader) =>
				serializer.TestReadEntitiesDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEntitiesDescriptor()
		{
			WriteTest<EntitiesDescriptor>("EntitiesDescriptor1",
				(serializer, writer, obj) =>
					serializer.TestWriteEntitiesDescriptor(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEntitiesDescriptorNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadEntitiesDescriptor(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEntitiesDescriptorNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteEntitiesDescriptor(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEntitiesDescriptorWriterNull()
		{
			WriteNullWriterTest<EntitiesDescriptor>("EntitiesDescriptor1",
				(serializer, obj) =>
					serializer.TestWriteEntitiesDescriptor(null, obj));
		}

		[TestData]
		public static void AddSaml2AttributeTestData()
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

			var obj = new Saml2Attribute("testAtt", "attValue")
			{
				NameFormat = new Uri("http://idp.example.com/nameformat"),
				FriendlyName = "friendlyAtt"
			};

			AddTestData("Saml2AttributeTest1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadSaml2Attribute()
		{
			ReadTest("Saml2AttributeTest1", (serializer, reader) =>
				serializer.TestReadSaml2Attribute(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteSaml2Attribute()
		{
			WriteTest<Saml2Attribute>("Saml2AttributeTest1",
				(serializer, writer, obj) =>
					serializer.TestWriteAttribute(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadSaml2AttributeNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadSaml2Attribute(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteSaml2AttributeNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteAttribute(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteSaml2AttributeWriterNull()
		{
			WriteNullWriterTest<Saml2Attribute>("Saml2AttributeTest1",
				(serializer, obj) =>
					serializer.TestWriteAttribute(null, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadSaml2AttributeNoName()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<saml:Attribute 
					xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion'
					NameFormat='http://idp.example.com/nameformat'
					FriendlyName='friendlyAtt'>
					<saml:AttributeValue>attValue</saml:AttributeValue>
					<CustomValue />
				</saml:Attribute>";

			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadSaml2Attribute(reader));
		}

		[TestData]
		public static void AddKeyDescriptorTestData()
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

			var obj = new KeyDescriptor
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

			string writtenXml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<md:KeyDescriptor
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
					xmlns:xenc='http://www.w3.org/2001/04/xmlenc#'
					use='encryption'>
					<ds:KeyInfo>
						<ds:KeyName>string</ds:KeyName>
					</ds:KeyInfo>
					<md:EncryptionMethod Algorithm='http://www.example.com/'>
						<xenc:KeySize>1</xenc:KeySize>
						<xenc:OAEPparams>GpM7</xenc:OAEPparams>
					</md:EncryptionMethod>
				</md:KeyDescriptor>";

			AddTestData("KeyDescriptor1", xml, writtenXml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadKeyDescriptor()
		{
			ReadTest("KeyDescriptor1", (serializer, reader) =>
				serializer.TestReadKeyDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteKeyDescriptor()
		{
			WriteTest<KeyDescriptor>("KeyDescriptor1",
				(serializer, writer, obj) =>
					serializer.TestWriteKeyDescriptor(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadKeyDescriptorNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadKeyDescriptor(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteKeyDescriptorNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteKeyDescriptor(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteKeyDescriptorWriterNull()
		{
			WriteNullWriterTest<KeyDescriptor>("KeyDescriptor1",
				(serializer, obj) =>
					serializer.TestWriteKeyDescriptor(null, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadKeyDescriptorInvalidUseAttribute()
		{
			string xml =
				@"<?xml version='1.0' encoding='UTF-8'?>
				<md:KeyDescriptor
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					use='INVALID'/>";
			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadKeyDescriptor(reader));
		}

		[TestData]
		public static void AddAffiliationDescriptorTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<md:AffiliationDescriptor 
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
					xmlns:xenc='http://www.w3.org/2001/04/xmlenc#'
					cacheDuration='P3M2DT9H12M'
					validUntil='2019-02-02T15:16:17Z'
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

			var obj = new AffiliationDescriptor
			{
				CacheDuration = new XsdDuration(months: 3, days: 2, hours: 9, minutes: 12),
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

			string writtenXml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<md:AffiliationDescriptor 
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
					xmlns:xenc='http://www.w3.org/2001/04/xmlenc#'
					cacheDuration='P3M2DT9H12M'
					validUntil='2019-02-02T15:16:17Z'
					affiliationOwnerID='mr owner'
					ID='yourGUIDhere'>
					<md:Extensions>
						<ext-elt/>
					</md:Extensions>
					<md:AffiliateMember>http://idp.example.org</md:AffiliateMember>
					<md:KeyDescriptor>
						<ds:KeyInfo>
							<ds:KeyName>string</ds:KeyName>
						</ds:KeyInfo>
						<md:EncryptionMethod Algorithm='http://www.example.com/'>
							<xenc:KeySize>1</xenc:KeySize>
							<xenc:OAEPparams>GpM7</xenc:OAEPparams>
							<!--any element-->
						</md:EncryptionMethod>
					</md:KeyDescriptor>
				</md:AffiliationDescriptor>";

			AddTestData("AffiliationDescriptor1", xml, writtenXml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAffiliationDescriptor()
		{
			ReadTest("AffiliationDescriptor1", (serializer, reader) =>
				serializer.TestReadAffiliationDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteAffiliationDescriptor()
		{
			WriteTest<AffiliationDescriptor>("AffiliationDescriptor1",
				(serializer, writer, obj) =>
					serializer.TestWriteAffiliationDescriptor(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAffiliationDescriptorNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadAffiliationDescriptor(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteAffiliationDescriptorNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteAffiliationDescriptor(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteAffiliationDescriptorWriterNull()
		{
			WriteNullWriterTest<AffiliationDescriptor>("AffiliationDescriptor1",
				(serializer, obj) =>
					serializer.TestWriteAffiliationDescriptor(null, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAffiliationDescriptorNoOwnerId()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<md:AffiliationDescriptor 
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					ID='yourGUIDhere' />";
			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadAffiliationDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAffiliationDescriptorInvalidDate()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<md:AffiliationDescriptor 
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					cacheDuration='P3M2DT9H12M'
					validUntil='2019-13-02T15:16:17Z'
					affiliationOwnerID='mr owner'
					ID='yourGUIDhere'/>";
			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadAffiliationDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAffiliationDescriptorInvalidDuration()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<md:AffiliationDescriptor 
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					cacheDuration='P3M2DT9H12'
					validUntil='2019-13-02T15:16:17Z'
					affiliationOwnerID='mr owner'
					ID='yourGUIDhere'/>";
			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadAffiliationDescriptor(reader));
		}

		[TestData]
		public static void AddAdditionalMetadataLocationTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<md:AdditionalMetadataLocation
				xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
				namespace='http://oasis.org/saml-more#'>
				http://www.example.com/
			</md:AdditionalMetadataLocation>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new AdditionalMetadataLocation
			{
				Namespace = "http://oasis.org/saml-more#",
				Uri = new Uri("http://www.example.com/")
			};

			AddTestData("AdditionalMetadataLocation", xml, obj, true);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAdditionalMetadataLocation()
		{
			ReadTest("AdditionalMetadataLocation", (serializer, reader) =>
				serializer.TestReadAdditionalMetadataLocation(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteAdditionalMetadataLocation()
		{
			WriteTest<AdditionalMetadataLocation>("AdditionalMetadataLocation",
				(serializer, writer, obj) =>
					serializer.TestWriteAdditionalMetadataLocation(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAdditionalMetadataLocationNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadAdditionalMetadataLocation(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteAdditionalMetadataLocationNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteAdditionalMetadataLocation(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteAdditionalMetadataLocationWriterNull()
		{
			WriteNullWriterTest<AdditionalMetadataLocation>("AdditionalMetadataLocation",
				(serializer, obj) =>
					serializer.TestWriteAdditionalMetadataLocation(null, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAdditionalMetadataLocationNoNamespace()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<md:AdditionalMetadataLocation
				xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'>
				http://www.example.com/
			</md:AdditionalMetadataLocation>";
			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadAdditionalMetadataLocation(reader));
		}

		[TestData]
		public static void AddPDPDescriptorTestData()
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

			var obj = new PDPDescriptor
			{
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

			string writtenXml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<md:PDPDescriptor
					xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					xmlns:xenc='http://www.w3.org/2001/04/xmlenc#'
					ID='ID' 
					protocolSupportEnumeration='http://www.example.com/ http://www.example.com/'>
				   <md:KeyDescriptor>
					  <ds:KeyInfo>
						 <ds:KeyName>string</ds:KeyName>
					  </ds:KeyInfo>
					  <md:EncryptionMethod Algorithm='http://www.example.com/'>
						 <xenc:KeySize>1</xenc:KeySize>
						 <xenc:OAEPparams>GpM7</xenc:OAEPparams>
					  </md:EncryptionMethod>
				   </md:KeyDescriptor>
				   <md:Organization>
					  <md:OrganizationName xml:lang='en-US'>string</md:OrganizationName>
					  <md:OrganizationDisplayName xml:lang='en-US'>string</md:OrganizationDisplayName>
					  <md:OrganizationURL xml:lang='en-US'>http://www.example.com/</md:OrganizationURL>
				   </md:Organization>
				   <md:ContactPerson contactType='technical'>
					  <md:Company>string</md:Company>
					  <md:GivenName>string</md:GivenName>
					  <md:SurName>string</md:SurName>
					  <md:EmailAddress>http://www.example.com/</md:EmailAddress>
					  <md:TelephoneNumber>string</md:TelephoneNumber>
				   </md:ContactPerson>
				   <md:AuthzService Binding='http://www.example.com/' Location='http://www.example.com/'/>
				   <md:AssertionIDRequestService Binding='http://www.example.com/' Location='http://www.example.com/'/>
				   <md:NameIDFormat>http://www.example.com/</md:NameIDFormat>
				</md:PDPDescriptor>";

			AddTestData("PDPDescriptor1", xml, writtenXml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadPDPDescriptor()
		{
			ReadTest("PDPDescriptor1", (serializer, reader) =>
				serializer.TestReadPDPDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WritePDPDescriptor()
		{
			WriteTest<PDPDescriptor>("PDPDescriptor1",
				(serializer, writer, obj) =>
					serializer.TestWritePDPDescriptor(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadPDPDescriptorNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadPDPDescriptor(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WritePDPDescriptorNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWritePDPDescriptor(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WritePDPDescriptorWriterNull()
		{
			WriteNullWriterTest<PDPDescriptor>("PDPDescriptor1",
				(serializer, obj) =>
					serializer.TestWritePDPDescriptor(null, obj));
		}

		[TestData]
		public static void AddAuthnAuthorityDescriptorTestData()
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

			var obj = new AuthnAuthorityDescriptor
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

			string writtenXml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<md:AuthnAuthorityDescriptor 
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					xmlns:xenc='http://www.w3.org/2001/04/xmlenc#'
					xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
					ID='ID' protocolSupportEnumeration='http://www.example.com/ http://www.example.com/'>
				   <md:KeyDescriptor>
					  <ds:KeyInfo>
						 <ds:KeyName>string</ds:KeyName>
					  </ds:KeyInfo>
					  <md:EncryptionMethod Algorithm='http://www.example.com/'>
						 <xenc:KeySize>1</xenc:KeySize>
						 <xenc:OAEPparams>GpM7</xenc:OAEPparams>
						 <!--any element-->
					  </md:EncryptionMethod>
				   </md:KeyDescriptor>
				   <md:Organization>
					  <md:OrganizationName xml:lang='en-US'>string</md:OrganizationName>
					  <md:OrganizationDisplayName xml:lang='en-US'>string</md:OrganizationDisplayName>
					  <md:OrganizationURL xml:lang='en-US'>http://www.example.com/</md:OrganizationURL>
				   </md:Organization>
				   <md:ContactPerson contactType='technical'>
					  <md:Company>string</md:Company>
					  <md:GivenName>string</md:GivenName>
					  <md:SurName>string</md:SurName>
					  <md:EmailAddress>http://www.example.com/</md:EmailAddress>
					  <md:TelephoneNumber>string</md:TelephoneNumber>
				   </md:ContactPerson>
				   <md:AuthnQueryService Binding='http://www.example.com/' Location='http://www.example.com/' />
				   <md:AssertionIDRequestService Binding='http://www.example.com/' Location='http://www.example.com/' />
				   <md:NameIDFormat>http://www.example.com/</md:NameIDFormat>
				</md:AuthnAuthorityDescriptor>";

			AddTestData("AuthnAuthorityDescriptor1", xml, writtenXml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAuthnAuthorityDescriptor()
		{
			ReadTest("AuthnAuthorityDescriptor1", (serializer, reader) =>
				serializer.TestReadAuthnAuthorityDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteAuthnAuthorityDescriptor()
		{
			WriteTest<AuthnAuthorityDescriptor>("AuthnAuthorityDescriptor1",
				(serializer, writer, obj) =>
					serializer.TestWriteAuthnAuthorityDescriptor(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAuthnAuthorityDescriptorNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadAuthnAuthorityDescriptor(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteAuthnAuthorityDescriptorNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteAuthnAuthorityDescriptor(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteAuthnAuthorityDescriptorWriterNull()
		{
			WriteNullWriterTest<AuthnAuthorityDescriptor>("AuthnAuthorityDescriptor1",
				(serializer, obj) =>
					serializer.TestWriteAuthnAuthorityDescriptor(null, obj));
		}

		[TestData]
		public static void AddAttributeAuthorityDescriptorTestData()
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

			var obj = new AttributeAuthorityDescriptor
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

			string writtenXml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<md:AttributeAuthorityDescriptor 
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					xmlns:xenc='http://www.w3.org/2001/04/xmlenc#'
					xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
					xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion'
					ID='ID'
					protocolSupportEnumeration='http://www.example.com/ http://www.example.com/'>
				   <md:KeyDescriptor>
					  <ds:KeyInfo>
						 <ds:KeyName>string</ds:KeyName>
					  </ds:KeyInfo>
					  <md:EncryptionMethod Algorithm='http://www.example.com/'>
						 <xenc:KeySize>1</xenc:KeySize>
						 <xenc:OAEPparams>GpM7</xenc:OAEPparams>
						 <!--any element-->
					  </md:EncryptionMethod>
				   </md:KeyDescriptor>
				   <md:Organization>
					  <md:OrganizationName xml:lang='en-US'>string</md:OrganizationName>
					  <md:OrganizationDisplayName xml:lang='en-US'>string</md:OrganizationDisplayName>
					  <md:OrganizationURL xml:lang='en-US'>http://www.example.com/</md:OrganizationURL>
				   </md:Organization>
				   <md:ContactPerson contactType='technical'>
					  <md:Company>string</md:Company>
					  <md:GivenName>string</md:GivenName>
					  <md:SurName>string</md:SurName>
					  <md:EmailAddress>http://www.example.com/</md:EmailAddress>
					  <md:TelephoneNumber>string</md:TelephoneNumber>
				   </md:ContactPerson>
				   <md:AttributeService Binding='http://www.example.com/' Location='http://www.example.com/'/>
				   <md:AssertionIDRequestService Binding='http://www.example.com/' Location='http://www.example.com/'/>
				   <md:NameIDFormat>http://www.example.com/</md:NameIDFormat>
				   <md:AttributeProfile>http://www.example.com/</md:AttributeProfile>
				   <saml:Attribute Name='string'>
					  <saml:AttributeValue>any content</saml:AttributeValue>
				   </saml:Attribute>
				</md:AttributeAuthorityDescriptor>";

			AddTestData("AttributeAuthorityDescriptor1", xml, writtenXml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAttributeAuthorityDescriptor()
		{
			ReadTest("AttributeAuthorityDescriptor1", (serializer, reader) =>
				serializer.TestReadAttributeAuthorityDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteAttributeAuthorityDescriptor()
		{
			WriteTest<AttributeAuthorityDescriptor>("AttributeAuthorityDescriptor1",
				(serializer, writer, obj) =>
					serializer.TestWriteAttributeAuthorityDescriptor(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAttributeAuthorityDescriptorNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadAttributeAuthorityDescriptor(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteAttributeAuthorityDescriptorNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteAttributeAuthorityDescriptor(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteAttributeAuthorityDescriptorWriterNull()
		{
			WriteNullWriterTest<AttributeAuthorityDescriptor>("AttributeAuthorityDescriptor1",
				(serializer, obj) =>
					serializer.TestWriteAttributeAuthorityDescriptor(null, obj));
		}

		[TestData]
		public static void AddNameIDFormatTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<md:NameIDFormat
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'>
					http://www.example.org/
				</md:NameIDFormat>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new NameIDFormat
			{
				Uri = new Uri("http://www.example.org/"),
			};

			AddTestData("NameIDFormat1", xml, obj, true);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadNameIDFormat()
		{
			ReadTest("NameIDFormat1", (serializer, reader) =>
				serializer.TestReadNameIDFormat(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteNameIDFormat()
		{
			WriteTest<NameIDFormat>("NameIDFormat1",
				(serializer, writer, obj) =>
					serializer.TestWriteNameIDFormat(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadNameIDFormatNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadNameIDFormat(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteNameIDFormatNull()
		{
			WriteNullTest((serializer, writer) =>
					serializer.TestWriteNameIDFormat(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteNameIDFormatWriterNull()
		{
			WriteNullWriterTest<NameIDFormat>("NameIDFormat1",
				(serializer, obj) =>
					serializer.TestWriteNameIDFormat(null, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadNameIDFormatNoUrl()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<md:NameIDFormat
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'>
				</md:NameIDFormat>";
			ReadTestThrow<MetadataSerializationException>(xml, (serializer, reader) =>
				serializer.TestReadNameIDFormat(reader));
		}

		[TestData]
		public static void AddEntityDescriptorTestData()
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

			var obj = new EntityDescriptor(new EntityId("https://idp.example.org/idp/shibboleth"))
			{
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
				Organization = new Organization
				{
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

			string writtenXml =
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
								<ds:X509Certificate>" + certData + @"</ds:X509Certificate>
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
						<ds:KeyInfo>
							<ds:KeyName>string</ds:KeyName>
						</ds:KeyInfo>
						<EncryptionMethod Algorithm='http://www.example.com/'>
							<xenc:KeySize>1</xenc:KeySize>
							<xenc:OAEPparams>GpM7</xenc:OAEPparams>
						</EncryptionMethod>
					</KeyDescriptor>
					<AuthnQueryService Binding='http://www.example.com/' Location='http://www.example.com/'/>
					<AssertionIDRequestService Binding='http://www.example.com/' Location='http://www.example.com/'/>
					<NameIDFormat>http://www.example.com/</NameIDFormat>
				</AuthnAuthorityDescriptor>

				<PDPDescriptor
					ID='ID' 
					protocolSupportEnumeration='http://www.example.com/ http://www.example.com/'>
				   <KeyDescriptor>
					  <ds:KeyInfo>
						 <ds:KeyName>string</ds:KeyName>
					  </ds:KeyInfo>
					  <EncryptionMethod Algorithm='http://www.example.com/'>
						 <xenc:KeySize>1</xenc:KeySize>
						 <xenc:OAEPparams>GpM7</xenc:OAEPparams>
						 <!--any element-->
					  </EncryptionMethod>
				   </KeyDescriptor>
				   <AuthzService Binding='http://www.example.com/' Location='http://www.example.com/'/>
				   <AssertionIDRequestService Binding='http://www.example.com/' Location='http://www.example.com/'/>
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

			AddTestData("EntityDescriptor1", xml, writtenXml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEntityDescriptor()
		{
			ReadTest("EntityDescriptor1", (serializer, reader) =>
				serializer.TestReadEntityDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEntityDescriptor()
		{
			WriteTest<EntityDescriptor>("EntityDescriptor1",
				(serializer, writer, obj) =>
					serializer.TestWriteEntityDescriptor(writer, obj));
		}
		[TestData]
		public static void AddEntityDescriptorTestData2()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<EntityDescriptor
				xmlns='urn:oasis:names:tc:SAML:2.0:metadata'
				xmlns:ds='http://www.w3.org/2000/09/xmldsig#'
				xmlns:shibmd='urn:mace:shibboleth:metadata:1.0'
				xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'
				entityID='https://idp.example.org/idp/shibboleth'>
				<RoleDescriptor xmlns:fed='http://docs.oasis-open.org/wsfed/federation/200706' xsi:type='fed:SecurityTokenServiceType' protocolSupportEnumeration='http://docs.oasis-open.org/wsfed/federation/200706'>
				</RoleDescriptor><IDPSSODescriptor protocolSupportEnumeration='urn:mace:shibboleth:1.0 urn:oasis:names:tc:SAML:2.0:protocol'>
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
			</EntityDescriptor>";
			
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new EntityDescriptor(new EntityId("https://idp.example.org/idp/shibboleth"))
			{
				RoleDescriptors = {
					new SecurityTokenServiceDescriptor
					{
						 ProtocolsSupported = {
							new Uri("http://docs.oasis-open.org/wsfed/federation/200706")
						}
					},
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
					}
				}
			};

			AddTestData("EntityDescriptor2", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEntityDescriptor2()
		{
			ReadTest("EntityDescriptor2", (serializer, reader) =>
				serializer.TestReadEntityDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEntityDescriptorNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadEntityDescriptor(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEntityDescriptorNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteEntityDescriptor(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEntityDescriptorWriterNull()
		{
			WriteNullWriterTest<EntityDescriptor>("EntityDescriptor1",
				(serializer, obj) =>
					serializer.TestWriteEntityDescriptor(null, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadEntityDescriptorNoEntityId()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<EntityDescriptor xmlns='urn:oasis:names:tc:SAML:2.0:metadata' />";
			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadEntityDescriptor(reader));
		}

		[TestData]
		public static void AddLocalizedNameTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<LocalName xmlns='urn:oasis:names:tc:SAML:2.0:metadata'
					xml:lang='en'>NameValue</LocalName>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new LocalizedName("NameValue", "en");

			AddTestData("LocalizedName1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadLocalizedName()
		{
			ReadTest("LocalizedName1", (serializer, reader) =>
				serializer.TestReadLocalizedName(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteLocalizedName()
		{
			WriteTest<LocalizedName>("LocalizedName1",
				(serializer, writer, obj) =>
					serializer.TestWriteLocalizedName(writer, obj, "LocalName",
						"urn:oasis:names:tc:SAML:2.0:metadata"));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadLocalizedNameNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadLocalizedName(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteLocalizedNameNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteLocalizedName(writer, null, "LocalName",
					"urn:oasis:names:tc:SAML:2.0:metadata"));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteLocalizedNameWriterNull()
		{
			WriteNullWriterTest<LocalizedName>("LocalizedName1",
				(serializer, obj) =>
					serializer.TestWriteLocalizedName(null, obj, "LocalName",
						"urn:oasis:names:tc:SAML:2.0:metadata"));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteLocalizedNameNameNull()
		{
			WriteTestThrow<LocalizedName, ArgumentNullException>("LocalizedName1",
				(serializer, writer, obj) =>
					serializer.TestWriteLocalizedName(writer, obj, null,
						"urn:oasis:names:tc:SAML:2.0:metadata"));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteLocalizedNameNamespaceNull()
		{
			WriteTestThrow<LocalizedName, ArgumentNullException>("LocalizedName1",
				(serializer, writer, obj) =>
					serializer.TestWriteLocalizedName(writer, obj, "LocalName", null));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadLocalizedNameNoLang()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<LocalName xmlns='urn:oasis:names:tc:SAML:2.0:metadata'>NameValue</LocalName>";
			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadLocalizedName(reader));
		}

		[TestData]
		public static void AddLocalizedUriTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
				<LocalUri xmlns='urn:oasis:names:tc:SAML:2.0:metadata'
					xml:lang='en'>http://www.foo.org/</LocalUri>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new LocalizedUri(new Uri("http://www.foo.org/"), "en");

			AddTestData("LocalizedUri1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadLocalizedUri()
		{
			ReadTest("LocalizedUri1", (serializer, reader) =>
				serializer.TestReadLocalizedUri(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteLocalizedUri()
		{
			WriteTest<LocalizedUri>("LocalizedUri1",
				(serializer, writer, obj) =>
					serializer.TestWriteLocalizedUri(writer, obj, "LocalUri",
						"urn:oasis:names:tc:SAML:2.0:metadata"));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadLocalizedUriNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadLocalizedUri(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteLocalizedUriNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteLocalizedUri(writer, null, "LocalUri",
					"urn:oasis:names:tc:SAML:2.0:metadata"));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteLocalizedUriWriterNull()
		{
			WriteNullWriterTest<LocalizedUri>("LocalizedUri1",
				(serializer, obj) =>
					serializer.TestWriteLocalizedUri(null, obj, "LocalUri",
						"urn:oasis:names:tc:SAML:2.0:metadata"));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteLocalizedUriNameNull()
		{
			WriteTestThrow<LocalizedUri, ArgumentNullException>("LocalizedUri1",
				(serializer, writer, obj) =>
					serializer.TestWriteLocalizedUri(writer, obj, null,
						"urn:oasis:names:tc:SAML:2.0:metadata"));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteLocalizedUriNamespaceNull()
		{
			WriteTestThrow<LocalizedUri, ArgumentNullException>("LocalizedUri1",
				(serializer, writer, obj) =>
					serializer.TestWriteLocalizedUri(writer, obj, "LocalUri",
						null));
		}

		[TestData]
		public static void AddOrganizationTestData()
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

			var obj = new Organization
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

			AddTestData("Organization1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadOrganization()
		{
			ReadTest("Organization1", (serializer, reader) =>
				serializer.TestReadOrganization(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteOrganization()
		{
			WriteTest<Organization>("Organization1",
				(serializer, writer, obj) =>
					serializer.TestWriteOrganization(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadOrganizationNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadOrganization(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteOrganizationNull()
		{
			WriteNullTest((serializer, writer) =>
					serializer.TestWriteOrganization(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteOrganizationWriterNull()
		{
			WriteNullWriterTest<Organization>("Organization1",
				(serializer, obj) =>
					serializer.TestWriteOrganization(null, obj));
		}

		[TestData]
		public static void AddMetadataTestData()
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

			var obj = new EntitiesDescriptor
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

			AddTestData("Metadata1", xml, obj, true);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadMetadata()
		{
			var testData = GetTestData("Metadata1");
			byte[] xmlBytes = Encoding.UTF8.GetBytes(testData.Xml);
			var expected = testData.ObjectModel.As<MetadataBase>();
			var serializer = new TestMetadataSerializer();

			using (var ms = new MemoryStream(xmlBytes))
			{
				var result = serializer.ReadMetadata(ms);
				result.Should().BeEquivalentTo(expected);
			}
			using (var ms = new MemoryStream(xmlBytes))
			using (var xr = XmlReader.Create(ms))
			{
				var result = serializer.ReadMetadata(xr);
				result.Should().BeEquivalentTo(expected);
			}
			using (var ms = new MemoryStream(xmlBytes))
			using (var xr = XmlReader.Create(ms))
			{
				var result = serializer.ReadMetadata(xr, NullSecurityTokenResolver.Instance);
				result.Should().BeEquivalentTo(expected);
			}
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteMetadata()
		{
			var testData = GetTestData("Metadata1");
			XmlDocument expectedDoc = new XmlDocument();
			expectedDoc.LoadXml(testData.WrittenXml);

			var obj = testData.ObjectModel.As<MetadataBase>();
			var serializer = new TestMetadataSerializer();

			Action<MemoryStream> checkStream = (MemoryStream ms) =>
			{
				ms.Position = 0;
				XmlDocument resultDoc = new XmlDocument();
				resultDoc.Load(ms);
				resultDoc.Should().BeEquivalentTo(expectedDoc);
			};

			using (var ms = new MemoryStream())
			{
				serializer.WriteMetadata(ms, obj);
				checkStream(ms);
			}

			using (var ms = new MemoryStream())
			{
				using (var xw = XmlWriter.Create(ms))
				{
					serializer.WriteMetadata(xw, obj);
				}
				checkStream(ms);
			}
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadMetadataStreamNull()
		{
			var serializer = new TestMetadataSerializer();
			Action a = () => serializer.ReadMetadata((Stream)null);
			a.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadMetadataXmlReaderNull()
		{
			var serializer = new TestMetadataSerializer();
			Action a = () => serializer.ReadMetadata((XmlReader)null);
			a.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadMetadataXmlReaderNullWithResolver()
		{
			var serializer = new TestMetadataSerializer();
			Action a = () => serializer.ReadMetadata((XmlReader)null,
				NullSecurityTokenResolver.Instance);
			a.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadMetadataStreamTokenResolverNull()
		{
			using (var ms = new MemoryStream())
			using (var xr = XmlReader.Create(ms))
			{
				var serializer = new TestMetadataSerializer();
				Action a = () => serializer.ReadMetadata(xr, null);
				a.Should().Throw<ArgumentNullException>();
			}
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteMetadataStreamObjNull()
		{
			var serializer = new TestMetadataSerializer();
			using (var ms = new MemoryStream())
			{
				Action a = () => serializer.WriteMetadata(ms, null);
				a.Should().Throw<ArgumentNullException>();
			}
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteMetadataStreamNull()
		{
			var obj = GetTestData("Metadata1").ObjectModel.As<MetadataBase>();
			var serializer = new TestMetadataSerializer();
			Action a = () => serializer.WriteMetadata((Stream)null, obj);
			a.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteMetadataXmlWriterNull()
		{
			var obj = GetTestData("Metadata1").ObjectModel.As<MetadataBase>();
			var serializer = new TestMetadataSerializer();
			Action a = () => serializer.WriteMetadata((XmlWriter)null, obj);
			a.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteMetadataXmlWriterObjNull()
		{
			using (var ms = new MemoryStream())
			using (var xw = XmlWriter.Create(ms))
			{
				var serializer = new TestMetadataSerializer();
				Action a = () => serializer.WriteMetadata(xw, null);
				a.Should().Throw<ArgumentNullException>();
			}
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadMetadataUnknownRoot()
		{
			string xml = @"<?xml version='1.0' encoding='UTF-8'?><INVALID/>";
			using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml), false))
			{
				Action a = () => new TestMetadataSerializer().ReadMetadata(ms);
				a.Should().Throw<MetadataSerializationException>();
			}
		}

		[TestData]
		public static void AddSecurityTokenServiceDescriptorTestData1()
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
					<fed:LogicalServiceNamesOffered>
						<fed:IssuerName Uri='https://idp.example.com/lsn1'/>
						<fed:IssuerName Uri='https://idp.example.com/lsn2'/>
					</fed:LogicalServiceNamesOffered>
					<fed:TokenTypesOffered>
						<fed:TokenType Uri='urn:oasis:names:tc:SAML:2.0:assertion'/>
						<fed:TokenType Uri='urn:oasis:names:tc:SAML:1.0:assertion'/>
					</fed:TokenTypesOffered>
					<fed:ClaimDialectsOffered>
						<fed:ClaimDialect Uri='http://idp.example.com/dialect1'/>
						<fed:ClaimDialect Uri='http://idp.example.com/dialect2'/>
					</fed:ClaimDialectsOffered>
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
					<fed:ClaimTypesRequested>
						<auth:ClaimType Uri='http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsaccountname' 
							xmlns:auth='http://docs.oasis-open.org/wsfed/authorization/200706'>
							<auth:DisplayName>Windows account name</auth:DisplayName>
							<auth:Description>The domain account name of the user in the form of &lt;domain&gt;\&lt;user&gt;</auth:Description>
						</auth:ClaimType>
					</fed:ClaimTypesRequested>
					<fed:AutomaticPseudonyms>
						false
					</fed:AutomaticPseudonyms>
					<fed:TargetScopes>
						<wsa:EndpointReference>
							<wsa:Address>https://www.netiq.com/nidp/wsfed/ep</wsa:Address>
						</wsa:EndpointReference>
					</fed:TargetScopes>
					<fed:SecurityTokenServiceEndpoint>
						<wsa:EndpointReference>
							<wsa:Address>https://www.netiq.com/nidp/wsfed/ep</wsa:Address>
						</wsa:EndpointReference>
					</fed:SecurityTokenServiceEndpoint>
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
					<fed:PassiveRequestorEndpoint>
						<wsa:EndpointReference>
							<wsa:Address>https://www.netiq.com/nidp/wsfed/ep</wsa:Address>
						</wsa:EndpointReference>
					</fed:PassiveRequestorEndpoint>
				</RoleDescriptor>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj= new SecurityTokenServiceDescriptor
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
				LogicalServiceNamesOffered = {
					new Uri("https://idp.example.com/lsn1"),
					new Uri("https://idp.example.com/lsn2")
				},
				ClaimDialectsOffered = {
					new Uri("http://idp.example.com/dialect1"),
					new Uri("http://idp.example.com/dialect2")
				},
				ClaimTypesRequested = {
					new DisplayClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsaccountname")
					{
						DisplayName = "Windows account name",
						Description = "The domain account name of the user in the form of <domain>\\<user>"
					}
				},
				AutomaticPseudonyms = false,
				TargetScopes = {
					new EndpointReference("https://www.netiq.com/nidp/wsfed/ep")
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

			AddTestData("SecurityTokenServiceDescriptor1", xml, obj, true);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadSecurityTokenServiceDescriptor()
		{
			ReadTest("SecurityTokenServiceDescriptor1", (serializer, reader) =>
				serializer.TestReadSecurityTokenServiceDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteSecurityTokenServiceDescriptor()
		{
			WriteTest<SecurityTokenServiceDescriptor>("SecurityTokenServiceDescriptor1",
				(serializer, writer, obj) =>
					serializer.TestWriteSecurityTokenServiceDescriptor(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadSecurityTokenServiceDescriptorNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadSecurityTokenServiceDescriptor(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteSecurityTokenServiceDescriptorNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteSecurityTokenServiceDescriptor(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteSecurityTokenServiceDescriptorWriterNull()
		{
			WriteNullWriterTest<SecurityTokenServiceDescriptor>("SecurityTokenServiceDescriptor1",
				(serializer, obj) =>
					serializer.TestWriteSecurityTokenServiceDescriptor(null, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadSecurityTokenServiceDescriptorNoProtocolsSupported()
		{
			string xml =
				@"<?xml version='1.0' encoding='UTF-8' ?>
				<RoleDescriptor 
					xsi:type='fed:SecurityTokenServiceType'
					ServiceDisplayName='www.netiq.com' 
					ServiceDescription='test'
					xmlns='urn:oasis:names:tc:SAML:2.0:metadata'
					xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' />";
			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
					serializer.TestReadSecurityTokenServiceDescriptor(reader));
		}

		[TestData]
		public static void AddAttributeConsumingServiceTestData()
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

			var obj = new AttributeConsumingService
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

			AddTestData("AttributeConsumingService1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAttributeConsumingService()
		{
			ReadTest("AttributeConsumingService1", (serializer, reader) =>
				serializer.TestReadAttributeConsumingService(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteAttributeConsumingService()
		{
			WriteTest<AttributeConsumingService>("AttributeConsumingService1",
				(serializer, writer, obj) =>
					serializer.TestWriteAttributeConsumingService(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadAttributeConsumingServiceNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadAttributeConsumingService(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteAttributeConsumingServiceNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteAttributeConsumingService(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteAttributeConsumingServiceWriterNull()
		{
			WriteNullWriterTest<AttributeConsumingService>("AttributeConsumingService1",
				(serializer, obj) =>
					serializer.TestWriteAttributeConsumingService(null, obj));
		}

		[TestData]
		public static void AddRequestedAttributeTestData()
		{
			string xml =
				@"<?xml version='1.0' encoding='UTF-8' ?>
				<md:RequestedAttribute 
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion'
					isRequired='true'
					FriendlyName='eduPersonPrincipalName' 
					Name='urn:mace:dir:attribute-def:eduPersonPrincipalName'
					NameFormat='urn:mace:shibboleth:1.0:attributeNamespace:uri'>
					<saml:AttributeValue>VALUE</saml:AttributeValue>
				</md:RequestedAttribute>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new RequestedAttribute("urn:mace:dir:attribute-def:eduPersonPrincipalName")
			{
				IsRequired = true,
				FriendlyName = "eduPersonPrincipalName",
				NameFormat = new Uri("urn:mace:shibboleth:1.0:attributeNamespace:uri"),
				Values = { "VALUE" }
			};

			AddTestData("RequestedAttribute1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadRequestedAttribute()
		{
			ReadTest("RequestedAttribute1", (serializer, reader) =>
				serializer.TestReadRequestedAttribute(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteRequestedAttribute()
		{
			WriteTest<RequestedAttribute>("RequestedAttribute1",
				(serializer, writer, obj) =>
					serializer.TestWriteRequestedAttribute(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadRequestedAttributeNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadRequestedAttribute(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteRequestedAttributeNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteRequestedAttribute(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteRequestedAttributeWriterNull()
		{
			WriteNullWriterTest<RequestedAttribute>("RequestedAttribute1",
				(serializer, obj) =>
					serializer.TestWriteRequestedAttribute(null, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadRequestedAttributeInvalidBoolean()
		{
			string xml =
				@"<?xml version='1.0' encoding='UTF-8' ?>
				<md:RequestedAttribute 
					xmlns:md='urn:oasis:names:tc:SAML:2.0:metadata'
					xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion'
					isRequired='WRONG'
					FriendlyName='eduPersonPrincipalName' 
					Name='urn:mace:dir:attribute-def:eduPersonPrincipalName'
					NameFormat='urn:mace:shibboleth:1.0:attributeNamespace:uri'>
					<saml:AttributeValue>VALUE</saml:AttributeValue>
				</md:RequestedAttribute>";
			ReadTestThrow<MetadataSerializationException>(xml,
				(serializer, reader) =>
						serializer.TestReadRequestedAttribute(reader));
		}

		[TestData]
		public static void AddSpSsoDescriptorTestData()
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

			var obj = new SpSsoDescriptor
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

			AddTestData("SpSsoDescriptor1", xml, obj, true);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadSpSsoDescriptor()
		{
			ReadTest("SpSsoDescriptor1", (serializer, reader) =>
				serializer.TestReadSpSsoDescriptor(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteSpSsoDescriptor()
		{
			WriteTest<SpSsoDescriptor>("SpSsoDescriptor1",
				(serializer, writer, obj) =>
					serializer.TestWriteSpSsoDescriptor(writer, obj));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadSpSsoDescriptorNull()
		{
			ReadNullTest((serializer) =>
				serializer.TestReadSpSsoDescriptor(null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteSpSsoDescriptorNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteSpSsoDescriptor(writer, null));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteSpSsoDescriptorWriterNull()
		{
			WriteNullWriterTest<SpSsoDescriptor>("SpSsoDescriptor1",
				(serializer, obj) =>
					serializer.TestWriteSpSsoDescriptor(null, obj));
		}

		[TestData]
		public static void AddEndpointTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<x:TestEndpoint xmlns:x='test:namespace:01'
				Binding = 'https://idp.example.com/binding'
				Location = 'https://idp.example.com/location'
				ResponseLocation = 'https://idp.example.com/responseLocation'/>";

			var obj = new Endpoint
			{
				Binding = new Uri("https://idp.example.com/binding"),
				Location = new Uri("https://idp.example.com/location"),
				ResponseLocation = new Uri("https://idp.example.com/responseLocation")
			};

			AddTestData("Endpoint1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEndpoint()
		{
			WriteTest<Endpoint>("Endpoint1", (serializer, writer, obj) =>
				serializer.TestWriteEndpoint(writer, obj,
					"TestEndpoint", "test:namespace:01"));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEndpointNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteEndpoint(writer, null,
					"TestEndpoint", "test:namespace:01"));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEndpointWriterNull()
		{
			WriteNullWriterTest<Endpoint>("Endpoint1", (serializer, obj) =>
				serializer.TestWriteEndpoint(null, obj,
					"TestEndpoint", "test:namespace:01"));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEndpointNameNull()
		{
			WriteTestThrow<Endpoint, ArgumentNullException>("Endpoint1",
				(serializer, writer, obj) =>
					serializer.TestWriteEndpoint(writer, obj,
						null, "test:namespace:01"));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteEndpointNamespaceNull()
		{
			WriteTestThrow<Endpoint, ArgumentNullException>("Endpoint1",
				(serializer, writer, obj) =>
					serializer.TestWriteEndpoint(writer, obj,
						"TestEndpoint", null));
		}

		[TestData]
		public static void AddIndexedEndpointData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<x:TestIndexedEndpoint xmlns:x='test:namespace:02'
				isDefault = 'true'
				index = '4'
				Binding = 'https://idp.example.com/binding'
				Location = 'https://idp.example.com/location'
				ResponseLocation = 'https://idp.example.com/responseLocation'/>";

			var obj = new IndexedEndpoint
			{
				Index = 4,
				IsDefault = true,
				Binding = new Uri("https://idp.example.com/binding"),
				Location = new Uri("https://idp.example.com/location"),
				ResponseLocation = new Uri("https://idp.example.com/responseLocation")
			};

			AddTestData("IndexedEndpoint1", xml, obj);
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteIndexedEndpoint()
		{
			WriteTest<IndexedEndpoint>("IndexedEndpoint1", (serializer, writer, obj) =>
				serializer.TestWriteIndexedEndpoint(
					writer, obj, "TestIndexedEndpoint", "test:namespace:02"));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteIndexedEndpointNull()
		{
			WriteNullTest((serializer, writer) =>
				serializer.TestWriteIndexedEndpoint(
					writer, null, "TestIndexedEndpoint", "test:namespace:02"));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteIndexedEndpointWriterNull()
		{
			WriteNullWriterTest<IndexedEndpoint>("IndexedEndpoint1",
				(serializer, obj) =>
					serializer.TestWriteIndexedEndpoint(
						null, obj, "TestIndexedEndpoint", "test:namespace:02"));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteIndexedEndpointNameNull()
		{
			WriteTestThrow<IndexedEndpoint, ArgumentNullException>("IndexedEndpoint1",
				(serializer, writer, obj) =>
					serializer.TestWriteIndexedEndpoint(
						writer, obj, null, "test:namespace:02"));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteIndexedEndpointNamespaceNull()
		{
			WriteTestThrow<IndexedEndpoint, ArgumentNullException>("IndexedEndpoint1",
				(serializer, writer, obj) =>
					serializer.TestWriteIndexedEndpoint(
						writer, obj, "TestIndexedEndpoint", null));
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadWebServiceDescriptorAttributesReaderNull()
		{
			var descriptor = new ApplicationServiceDescriptor();
			Action a = () => new TestMetadataSerializer()
				.TestReadWebServiceDescriptorAttributes(null, descriptor);
			a.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadWebServiceDescriptorAttributesDescriptorNull()
		{
			using (var sr = new StringReader("<root/>"))
			using (var xr = XmlReader.Create(sr))
			{
				Action a = () => new TestMetadataSerializer()
					.TestReadWebServiceDescriptorAttributes(xr, null);
				a.Should().Throw<ArgumentNullException>();
			}
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadWebServiceDescriptorElementReaderNull()
		{
			var descriptor = new ApplicationServiceDescriptor();
			Action a = () => new TestMetadataSerializer()
				.TestReadWebServiceDescriptorElement(null, descriptor);
			a.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadWebServiceDescriptorElementDescriptorNull()
		{
			using (var sr = new StringReader("<root/>"))
			using (var xr = XmlReader.Create(sr))
			{
				Action a = () => new TestMetadataSerializer()
					.TestReadWebServiceDescriptorElement(xr, null);
				a.Should().Throw<ArgumentNullException>();
			}
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteWebServiceDescriptorAttributesReaderNull()
		{
			var descriptor = new ApplicationServiceDescriptor();
			Action a = () => new TestMetadataSerializer()
				.TestWriteWebServiceDescriptorAttributes(null, descriptor);
			a.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteWebServiceDescriptorAttributesDescriptorNull()
		{
			using (var ms = new MemoryStream())
			using (var xw = XmlWriter.Create(ms))
			{
				Action a = () => new TestMetadataSerializer()
					.TestWriteWebServiceDescriptorAttributes(xw, null);
				a.Should().Throw<ArgumentNullException>();
			}
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteWebServiceDescriptorElementReaderNull()
		{
			var descriptor = new ApplicationServiceDescriptor();
			Action a = () => new TestMetadataSerializer()
				.TestWriteWebServiceDescriptorElements(null, descriptor);
			a.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteWebServiceDescriptorElementDescriptorNull()
		{
			using (var ms = new MemoryStream())
			using (var xw = XmlWriter.Create(ms))
			{
				Action a = () => new TestMetadataSerializer()
					.TestWriteWebServiceDescriptorElements(xw, null);
				a.Should().Throw<ArgumentNullException>();
			}
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteSsoDescriptorAttributesReaderNull()
		{
			var descriptor = new IdpSsoDescriptor();
			Action a = () => new TestMetadataSerializer()
				.TestWriteSsoDescriptorAttributes(null, descriptor);
			a.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteSsoDescriptorAttributesDescriptorNull()
		{
			using (var ms = new MemoryStream())
			using (var xw = XmlWriter.Create(ms))
			{
				Action a = () => new TestMetadataSerializer()
					.TestWriteSsoDescriptorAttributes(xw, null);
				a.Should().Throw<ArgumentNullException>();
			}
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteSsoDescriptorElementReaderNull()
		{
			var descriptor = new IdpSsoDescriptor();
			Action a = () => new TestMetadataSerializer()
				.TestWriteSsoDescriptorElements(null, descriptor);
			a.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteSsoDescriptorElementDescriptorNull()
		{
			using (var ms = new MemoryStream())
			using (var xw = XmlWriter.Create(ms))
			{
				Action a = () => new TestMetadataSerializer()
					.TestWriteSsoDescriptorElements(xw, null);
				a.Should().Throw<ArgumentNullException>();
			}
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteRoleDescriptorAttributesReaderNull()
		{
			var descriptor = new IdpSsoDescriptor();
			Action a = () => new TestMetadataSerializer()
				.TestWriteRoleDescriptorAttributes(null, descriptor);
			a.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteRoleDescriptorAttributesDescriptorNull()
		{
			using (var ms = new MemoryStream())
			using (var xw = XmlWriter.Create(ms))
			{
				Action a = () => new TestMetadataSerializer()
					.TestWriteRoleDescriptorAttributes(xw, null);
				a.Should().Throw<ArgumentNullException>();
			}
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteRoleDescriptorElementReaderNull()
		{
			var descriptor = new IdpSsoDescriptor();
			Action a = () => new TestMetadataSerializer()
				.TestWriteRoleDescriptorElements(null, descriptor);
			a.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteRoleDescriptorElementDescriptorNull()
		{
			using (var ms = new MemoryStream())
			using (var xw = XmlWriter.Create(ms))
			{
				Action a = () => new TestMetadataSerializer()
					.TestWriteRoleDescriptorElements(xw, null);
				a.Should().Throw<ArgumentNullException>();
			}
		}

		[TestData]
		public static void AddServiceNameTestData()
		{
			string xml =
			@"<?xml version='1.0' encoding='UTF-8'?>
			<wsa:ServiceName
				xmlns:wsa='http://www.w3.org/2005/08/addressing'
				PortName='MrPort'>
				nameofservice
			</wsa:ServiceName>";
			(XmlDocument doc, XmlNamespaceManager nsmgr) = LoadXml(xml);

			var obj = new ServiceName
			{
				Name = "nameofservice",
				PortName = "MrPort"
			};

			AddTestData("ServiceName1", xml, obj, true);
		}

		[TestMethod]
		public void MetadataSerializerTests_ReadServiceName()
		{
			ReadTest("ServiceName1", (serializer, reader) =>
				serializer.TestReadServiceName(reader));
		}

		[TestMethod]
		public void MetadataSerializerTests_WriteServiceName()
		{
			WriteTest<ServiceName>("ServiceName1",
				(serializer, writer, obj) =>
					serializer.TestWriteServiceName(writer, obj));
		}
	}
}
