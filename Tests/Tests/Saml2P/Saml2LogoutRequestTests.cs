using FluentAssertions;
using Sustainsys.Saml2.Internal;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Saml2P;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sustainsys.Saml2.Tests.Helpers;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace Sustainsys.Saml2.Tests.Saml2P
{
    [TestClass]
    public class Saml2LogoutRequestTests
    {
        [TestMethod]
        public void Saml2LogoutRequest_ToXml()
        {
            var subject = new Saml2LogoutRequest()
            {
                DestinationUrl = new Uri("http://idp.example.com/logout"),
                Issuer = new EntityId("http://sp.example.com/"),
                NameId = new Saml2NameIdentifier("005a06e0-ad82-110d-a556-004005b13a2b")
                {
                    Format = new Uri(NameIdFormat.Persistent.GetUri().AbsoluteUri),
                    NameQualifier = "qualifier",
                    SPNameQualifier = "spQualifier",
                    SPProvidedId = "spId"
                },
                SessionIndex = "SessionId"
            };

            var actual = XmlHelpers.XmlDocumentFromString(subject.ToXml());

            var expected = XmlHelpers.XmlDocumentFromString(
@"<saml2p:LogoutRequest xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
 ID=""d2b7c388cec36fa7c39c28fd298644a8"" Version=""2.0"" IssueInstant=""2004-01-21T19:00:49Z""
 Destination=""http://idp.example.com/logout"">
 <saml2:Issuer>http://sp.example.com/</saml2:Issuer>
 <saml2:NameID Format=""urn:oasis:names:tc:SAML:2.0:nameid-format:persistent""
    NameQualifier = ""qualifier""
    SPNameQualifier = ""spQualifier""
    SPProvidedID = ""spId"">005a06e0-ad82-110d-a556-004005b13a2b</saml2:NameID>
 <saml2p:SessionIndex>SessionId</saml2p:SessionIndex>
</saml2p:LogoutRequest>");

            // Set generated expected values to the actual.
            expected.DocumentElement.Attributes["ID"].Value =
                actual.DocumentElement.Attributes["ID"].Value;
            expected.DocumentElement.Attributes["IssueInstant"].Value =
                actual.DocumentElement.Attributes["IssueInstant"].Value;

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2LogoutRequest_FromXml()
        {
            var xmlData =
            @"<saml2p:LogoutRequest xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
 ID=""d2b7c388cec36fa7c39c28fd298644a8"" Version=""2.0"" IssueInstant=""2004-01-21T19:00:49Z""
 Destination=""http://idp.example.com/logout"">
 <saml2:Issuer>http://sp.example.com/</saml2:Issuer>
 <saml2:NameID Format=""urn:oasis:names:tc:SAML:2.0:nameid-format:persistent"">005a06e0-ad82-110d-a556-004005b13a2b</saml2:NameID>
 <saml2p:SessionIndex>SessionId</saml2p:SessionIndex>
</saml2p:LogoutRequest>";

            var xmlDoc = XmlHelpers.XmlDocumentFromString(xmlData);

            var subject = Saml2LogoutRequest.FromXml(xmlDoc.DocumentElement);

            var expected = new Saml2LogoutRequest(new Saml2Id("d2b7c388cec36fa7c39c28fd298644a8"))
            {
                DestinationUrl = new Uri("http://idp.example.com/logout"),
                Issuer = new EntityId("http://sp.example.com/"),
                NameId = new Saml2NameIdentifier(
                    "005a06e0-ad82-110d-a556-004005b13a2b",
                    new Uri(NameIdFormat.Persistent.GetUri().AbsoluteUri)),
                SessionIndex = "SessionId",
            };

            subject.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2LogoutRequest_FromXml_OnlyRequiredData()
        {
            var xmlData =
            @"<saml2p:LogoutRequest xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
 ID=""d2b7c388cec36fa7c39c28fd298644a8"" Version=""2.0"" IssueInstant=""2004-01-21T19:00:49Z"">
 <saml2:NameID>005a06e0-ad82-110d-a556-004005b13a2b</saml2:NameID>
</saml2p:LogoutRequest>";

            var xmlDoc = XmlHelpers.XmlDocumentFromString(xmlData);

            var subject = Saml2LogoutRequest.FromXml(xmlDoc.DocumentElement);

            var expected = new Saml2LogoutRequest(new Saml2Id("d2b7c388cec36fa7c39c28fd298644a8"))
            {
                NameId = new Saml2NameIdentifier("005a06e0-ad82-110d-a556-004005b13a2b"),
            };

            subject.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2LogoutRequest_FromXml_Nullcheck()
        {
            Action a = () => Saml2LogoutRequest.FromXml(null);

            a.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("xml");
        }
    }
}
