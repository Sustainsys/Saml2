using FluentAssertions;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Saml2P;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sustainsys.Saml2.Tests.Helpers;
using System.Xml;
using System;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace Sustainsys.Saml2.Tests.Saml2P
{
    [TestClass]
    public class Saml2LogoutResponseTests
    {
        [TestMethod]
        public void Saml2LogoutResponse_AppendTo_Minimal()
        {
            var subject = new Saml2LogoutResponse(Saml2StatusCode.Success);

            var expectedXml =
                $@"<samlp:LogoutResponse 
                        xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol""
                        xmlns=""urn:oasis:names:tc:SAML:2.0:assertion""
                        ID=""{subject.Id.Value}""
                        Version=""2.0""
                        IssueInstant=""{subject.IssueInstant.ToSaml2DateTimeString()}"">
                        <samlp:Status>
                            <samlp:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success""/>
                        </samlp:Status>
                    </samlp:LogoutResponse>";

            var expectedNode = XmlHelpers.XmlDocumentFromString(expectedXml).DocumentElement;

            var xmlDoc = XmlHelpers.CreateSafeXmlDocument();

            subject.AppendTo(xmlDoc);

            xmlDoc.DocumentElement.Should().BeEquivalentTo(expectedNode, "XML should be a valid LogoutResponse");
        }

        [TestMethod]
        public void Saml2LogoutResponse_AppendTo_AllSupported()
        {
            var subject = new Saml2LogoutResponse(Saml2StatusCode.Requester)
            {
                Issuer = new EntityId("https://ServiceProvider.com/SAML"),
                DestinationUrl = new Uri("https://IdentityProvider.com/Logout"),
                InResponseTo = new Saml2Id()
            };
            
            var expectedXml =
                $@"<samlp:LogoutResponse xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol""
                    xmlns=""urn:oasis:names:tc:SAML:2.0:assertion""
                    ID=""{subject.Id.Value}""
                    InResponseTo=""{subject.InResponseTo.Value}""
                    IssueInstant=""{subject.IssueInstant.ToSaml2DateTimeString()}"" Version=""2.0""
                    Destination=""https://IdentityProvider.com/Logout"">
                    <Issuer>https://ServiceProvider.com/SAML</Issuer>
                    <samlp:Status>
                        <samlp:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester""/>
                    </samlp:Status>
                </samlp:LogoutResponse>";

            var expectedElement = XmlHelpers.XmlDocumentFromString(expectedXml).DocumentElement;

            var xmlDoc = XmlHelpers.CreateSafeXmlDocument();
            subject.AppendTo(xmlDoc);

            xmlDoc.DocumentElement.Should().BeEquivalentTo(expectedElement, "XML should be full LogoutResponse");
        }

        [TestMethod]
        public void Saml2LogoutResponse_ToXml()
        {
            var subject = new Saml2LogoutResponse(Saml2StatusCode.Requester)
            {
                Issuer = new EntityId("https://ServiceProvider.com/SAML"),
                DestinationUrl = new Uri("https://IdentityProvider.com/Logout"),
                InResponseTo = new Saml2Id()
            };

            var xmlDoc = XmlHelpers.CreateSafeXmlDocument();
            subject.AppendTo(xmlDoc);
            var expected = xmlDoc.OuterXml;

            subject.ToXml().Should().Be(expected);
        }

        [TestMethod]
        public void Saml2LogoutResponse_FromXml_Nullcheck()
        {
            Action a = () => Saml2LogoutResponse.FromXml(null);

            a.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("xml");
        }
    }
}
