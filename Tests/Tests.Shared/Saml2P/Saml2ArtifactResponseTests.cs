using Sustainsys.Saml2.Saml2P;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FluentAssertions;
using System.Xml;
using Sustainsys.Saml2.Exceptions;

namespace Sustainsys.Saml2.Tests.Saml2P
{
    [TestClass]
    public class Saml2ArtifactResponseTests
    {
        [TestMethod]
        public void Saml2ArtifactResponse_Ctor_Nullcheck()
        {
            Action a = () => new Saml2ArtifactResponse(null);

            a.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("xml");
        }

        [TestMethod]
        public void Saml2ArtifactResponse_GetMessage()
        {
            var xml =
@"<samlp:ArtifactResponse
  xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol""
  xmlns=""urn:oasis:names:tc:SAML:2.0:assertion""
  ID=""_FQvGknDfws2Z"" Version=""2.0""
  InResponseTo=""_6c3a4f8b9c2d""
  IssueInstant=""2004-01-21T19:00:49Z"">
  <Issuer>https://IdentityProvider.com/SAML</Issuer>
  <ds:Signature xmlns:ds=""http://www.w3.org/2000/09/xmldsig#"">Not parsed...</ds:Signature>
  <samlp:Extensions>Extended data</samlp:Extensions>
  <samlp:Status>
    <samlp:StatusCode
      Value=""urn:oasis:names:tc:SAML:2.0:status:Success""/>
  </samlp:Status>
  <samlp:LogoutRequest ID=""d2b7c388cec36fa7c39c28fd298644a8""
    IssueInstant=""2004-01-21T19:00:49Z""
    Version=""2.0"">
    <Issuer>https://IdentityProvider.com/SAML</Issuer>
    <NameID Format=""urn:oasis:names:tc:SAML:2.0:nameidformat:persistent"">005a06e0-ad82-110d-a556-004005b13a2b</NameID>
    <samlp:SessionIndex>1</samlp:SessionIndex>
  </samlp:LogoutRequest>
</samlp:ArtifactResponse>";

            var xmlDocument = XmlHelpers.XmlDocumentFromString(xml);
            var xmlElement = xmlDocument.DocumentElement;

            var subject = new Saml2ArtifactResponse(xmlElement);

            subject.GetMessage().LocalName.Should().Be("LogoutRequest");
        }

        [TestMethod]
        public void Saml2ArtifactResponse_Status()
        {
            var xml =
@"<samlp:ArtifactResponse
  xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol""
  xmlns=""urn:oasis:names:tc:SAML:2.0:assertion""
  ID=""_FQvGknDfws2Z"" Version=""2.0""
  InResponseTo=""_6c3a4f8b9c2d""
  IssueInstant=""2004-01-21T19:00:49Z"">
  <Issuer>https://IdentityProvider.com/SAML</Issuer>
  <ds:Signature xmlns:ds=""http://www.w3.org/2000/09/xmldsig#"">Not parsed...</ds:Signature>
  <samlp:Extensions>Extended data</samlp:Extensions>
  <samlp:Status>
    <samlp:StatusCode
      Value=""urn:oasis:names:tc:SAML:2.0:status:Success""/>
  </samlp:Status>
  <samlp:LogoutRequest ID=""d2b7c388cec36fa7c39c28fd298644a8""
    IssueInstant=""2004-01-21T19:00:49Z""
    Version=""2.0"">
    <Issuer>https://IdentityProvider.com/SAML</Issuer>
    <NameID Format=""urn:oasis:names:tc:SAML:2.0:nameidformat:persistent"">005a06e0-ad82-110d-a556-004005b13a2b</NameID>
    <samlp:SessionIndex>1</samlp:SessionIndex>
  </samlp:LogoutRequest>
</samlp:ArtifactResponse>";

            var xmlDoc = XmlHelpers.XmlDocumentFromString(xml);

            var subject = new Saml2ArtifactResponse(xmlDoc.DocumentElement);

            subject.Status.Should().Be(Saml2StatusCode.Success);
        }

        [TestMethod]
        public void Saml2ArtifactResponse_GetMessageThrowsOnStatusNotSuccess()
        {
            var xml =
@"<samlp:ArtifactResponse
  xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol""
  xmlns=""urn:oasis:names:tc:SAML:2.0:assertion""
  ID=""_FQvGknDfws2Z"" Version=""2.0""
  InResponseTo=""_6c3a4f8b9c2d""
  IssueInstant=""2004-01-21T19:00:49Z"">
  <Issuer>https://IdentityProvider.com/SAML</Issuer>
  <ds:Signature xmlns:ds=""http://www.w3.org/2000/09/xmldsig#"">Not parsed...</ds:Signature>
  <samlp:Extensions>Extended data</samlp:Extensions>
  <samlp:Status>
    <samlp:StatusCode
      Value=""urn:oasis:names:tc:SAML:2.0:status:Responder""/>
  </samlp:Status>
  <samlp:LogoutRequest ID=""d2b7c388cec36fa7c39c28fd298644a8""
    IssueInstant=""2004-01-21T19:00:49Z""
    Version=""2.0"">
    <Issuer>https://IdentityProvider.com/SAML</Issuer>
    <NameID Format=""urn:oasis:names:tc:SAML:2.0:nameidformat:persistent"">005a06e0-ad82-110d-a556-004005b13a2b</NameID>
    <samlp:SessionIndex>1</samlp:SessionIndex>
  </samlp:LogoutRequest>
</samlp:ArtifactResponse>";

            var xmlDoc = XmlHelpers.XmlDocumentFromString(xml);

            var subject = new Saml2ArtifactResponse(xmlDoc.DocumentElement);

            subject.Status.Should().Be(Saml2StatusCode.Responder);
            subject.Invoking(s => s.GetMessage())
                .Should().Throw<UnsuccessfulSamlOperationException>()
                .And.Message.Should().Be("Artifact resolution returned status Responder, can't extract message.");
        }

        [TestMethod]
        public void Saml2ArtifactResponse_GetMessage_WithoutOptionalElements()
        {
            var xml =
@"<samlp:ArtifactResponse
  xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol""
  xmlns=""urn:oasis:names:tc:SAML:2.0:assertion""
  ID=""_FQvGknDfws2Z"" Version=""2.0""
  InResponseTo=""_6c3a4f8b9c2d""
  IssueInstant=""2004-01-21T19:00:49Z"">
  <samlp:Status>
    <samlp:StatusCode
      Value=""urn:oasis:names:tc:SAML:2.0:status:Success""/>
  </samlp:Status>
  <samlp:LogoutRequest ID=""d2b7c388cec36fa7c39c28fd298644a8""
    IssueInstant=""2004-01-21T19:00:49Z""
    Version=""2.0"">
    <Issuer>https://IdentityProvider.com/SAML</Issuer>
    <NameID Format=""urn:oasis:names:tc:SAML:2.0:nameidformat:persistent"">005a06e0-ad82-110d-a556-004005b13a2b</NameID>
    <samlp:SessionIndex>1</samlp:SessionIndex>
  </samlp:LogoutRequest>
</samlp:ArtifactResponse>";

            var xmlDocument = XmlHelpers.XmlDocumentFromString(xml);

            var xmlElement = xmlDocument.DocumentElement;

            var subject = new Saml2ArtifactResponse(xmlElement);

            subject.GetMessage().LocalName.Should().Be("LogoutRequest");
        }

    }
}
