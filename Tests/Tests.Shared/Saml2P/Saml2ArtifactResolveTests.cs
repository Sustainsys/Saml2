using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Saml2P;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Sustainsys.Saml2.WebSso;
using System.Xml.Linq;

namespace Sustainsys.Saml2.Tests.Saml2P
{
    [TestClass]
    public class Saml2ArtifactResolveTests
    {
        [TestMethod]
        public void Saml2ArtifactResolve_ToXml()
        {
            var artifact = "MyArtifact";
            var subject = new Saml2ArtifactResolve()
            {
                Issuer = new EntityId("http://sp.example.com"),
                Artifact = artifact
            };

            var actual = XElement.Parse(subject.ToXml());

            var expected = XElement.Parse(
@"<saml2p:ArtifactResolve
    xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
    xmlns:saml2 = ""urn:oasis:names:tc:SAML:2.0:assertion""
    ID = ""_6c3a4f8b9c2d"" Version = ""2.0""
    IssueInstant = ""2004-01-21T19:00:49Z"" >
    <saml2:Issuer>http://sp.example.com</saml2:Issuer>
    <saml2p:Artifact>MyArtifact</saml2p:Artifact>
 </saml2p:ArtifactResolve>");

            // Set generated expected values to the actual.
            expected.Attribute("ID").Value = actual.Attribute("ID").Value;
            expected.Attribute("IssueInstant").Value = actual.Attribute("IssueInstant").Value;

			actual.Should().BeEquivalentTo(expected);
        }
    }
}
