using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Net;
using NSubstitute;
using System.Web;
using System.Security.Claims;

namespace Kentor.AuthServices.Tests
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IdentityModel.Tokens;
    using System.Linq;
    using System.Threading;
    using System.Xml;

    [TestClass]
    public class CommandResultTests
    {
        [TestInitialize]
        public void MyTestInitialize()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        [TestMethod]
        public void CommandResult_Defaults()
        {
            var expected = new
            {
                HttpStatusCode = HttpStatusCode.OK,
                Cacheability = HttpCacheability.NoCache,
                Location = (Uri)null,
                Principal = (ClaimsPrincipal)null,
                Content = (string)null,
                SecurityTokens = new List<SecurityToken>()
            };

            new CommandResult().ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void CommandResult_Apply_ChecksResponseNull()
        {
            Action a = () => new CommandResult().Apply(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("response");
        }

        [TestMethod]
        public void CommandResult_Apply_HttpStatusCode()
        {
            var response = Substitute.For<HttpResponseBase>();

            new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.PaymentRequired
            }.Apply(response);

            response.Received().StatusCode = (int)HttpStatusCode.PaymentRequired;
        }

        [TestMethod]
        public void CommandResult_Apply_Cacheability()
        {
            var cache = Substitute.For<HttpCachePolicyBase>();
            var response = Substitute.For<HttpResponseBase>();
            response.Cache.Returns(cache);

            new CommandResult()
            {
                Cacheability = HttpCacheability.ServerAndNoCache
            }.Apply(response);

            cache.Received().SetCacheability(HttpCacheability.ServerAndNoCache);
        }

        [TestMethod]
        public void CommandResult_Apply_HandleRedirect()
        {
            var response = Substitute.For<HttpResponseBase>();
            var redirectTarget = "http://example.com/redirect/target/";

            new CommandResult()
            {
                Location = new Uri(redirectTarget),
                HttpStatusCode = HttpStatusCode.SeeOther
            }.Apply(response);

            response.Received().Redirect(redirectTarget);
            response.StatusCode.Should().Be((int)HttpStatusCode.SeeOther);
        }

        [TestMethod]
        public void CommandResult_Apply_ThrowsOnMissingLocation()
        {
            var response = Substitute.For<HttpResponseBase>();

            Action a = () =>
                new CommandResult()
                {
                    HttpStatusCode = HttpStatusCode.SeeOther
                }.Apply(response);

            a.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void CommandResult_Apply_ThrowsOnLocationWithoutRedirect()
        {
            var response = Substitute.For<HttpResponseBase>();
        
            Action a = () =>
                new CommandResult()
                {
                    Location = new Uri("http://example.com")
                }.Apply(response);

            a.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void CommandResult_CreateSessionSecurityToken_RespectsValidFromAndValidTo()
        {
            var xmlUnversalDateFomat = "yyyy-MM-ddTHH:mm:ssZ";
            var notBefore = DateTime.UtcNow;
            var notOnOrAfter = DateTime.UtcNow.AddHours(1);
            
            var response = string.Format(
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = ""AcsCommand_Run_SuccessfulResult"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>
                    https://idp.example.com
                </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion
                Version=""2.0"" ID=""Saml2Response_GetClaims_CreateIdentity_Assertion1""
                IssueInstant=""{0}"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotBefore=""{0}"" NotOnOrAfter=""{1}"" />
                </saml2:Assertion>
            </saml2p:Response>", notBefore.ToString(xmlUnversalDateFomat), notOnOrAfter.ToString(xmlUnversalDateFomat));
            
            var ids = new [] { new ClaimsIdentity("Federation"), new ClaimsIdentity("ClaimsAuthenticationManager") };
            ids[0].AddClaim(new Claim(ClaimTypes.NameIdentifier, "SomeUser", null, "https://idp.example.com"));
            ids[1].AddClaim(new Claim(ClaimTypes.Role, "RoleFromClaimsAuthManager", null, "ClaimsAuthenticationManagerMock"));

            SecurityToken token;
            var document = new XmlDocument();
            document.LoadXml(response);
            using (var reader = new XmlNodeReader(document.DocumentElement["saml2:Assertion"]))
            {
                token = MorePublicSaml2SecurityTokenHandler.DefaultInstance.ReadToken(reader);
            }

            var commandResult = new CommandResult()
            {
                Principal = new ClaimsPrincipal(ids),
                SecurityTokens = new[] { token }
            };

            var result = commandResult.CreateSessionSecurityToken();

            result.ValidFrom.Should().BeCloseTo(notBefore, 1000);
            result.ValidTo.Should().BeCloseTo(notOnOrAfter, 1000);
        }

        [TestMethod]
        public void CommandResult_CreateSessionSecurityToken_RespectsMaxValidFromAndMinValidTo()
        {
            var xmlUnversalDateFomat = "yyyy-MM-ddTHH:mm:ssZ";
            var notBefore = DateTime.UtcNow.AddMinutes(-5);
            var notOnOrAfter = notBefore.AddHours(1);

            var notBefore2 = DateTime.UtcNow;
            var notOnOrAfter2 = notBefore2.AddHours(2);

            var response = string.Format(
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = ""AcsCommand_Run_SuccessfulResult"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>
                    https://idp.example.com
                </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion
                Version=""2.0"" ID=""Saml2Response_GetClaims_CreateIdentity_Assertion1""
                IssueInstant=""{0}"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotBefore=""{0}"" NotOnOrAfter=""{1}"" />
                </saml2:Assertion>
                <saml2:Assertion
                Version=""2.0"" ID=""Saml2Response_GetClaims_CreateIdentity_Assertion2""
                IssueInstant=""{0}"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser2</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotBefore=""{2}"" NotOnOrAfter=""{3}"" />
                </saml2:Assertion>
            </saml2p:Response>", notBefore.ToString(xmlUnversalDateFomat), notOnOrAfter.ToString(xmlUnversalDateFomat), notBefore2.ToString(xmlUnversalDateFomat), notOnOrAfter2.ToString(xmlUnversalDateFomat));

            var ids = new [] { new ClaimsIdentity("Federation"), new ClaimsIdentity("ClaimsAuthenticationManager") };
            ids[0].AddClaim(new Claim(ClaimTypes.NameIdentifier, "SomeUser", null, "https://idp.example.com"));
            ids[1].AddClaim(new Claim(ClaimTypes.Role, "RoleFromClaimsAuthManager", null, "ClaimsAuthenticationManagerMock"));

            SecurityToken token1;
            SecurityToken token2;
            var document = new XmlDocument();
            document.LoadXml(response);

            var assertionElementNodes =
                        document.DocumentElement.ChildNodes.Cast<XmlNode>()
                        .Where(node => node.NodeType == XmlNodeType.Element).Cast<XmlElement>()
                        .Where(xe => xe.LocalName == "Assertion" && xe.NamespaceURI == Saml2Namespaces.Saml2Name);

            using (var reader = new XmlNodeReader(assertionElementNodes.First()))
            {
                token1 = MorePublicSaml2SecurityTokenHandler.DefaultInstance.ReadToken(reader);
            }

            using (var reader = new XmlNodeReader(assertionElementNodes.Last()))
            {
                token2 = MorePublicSaml2SecurityTokenHandler.DefaultInstance.ReadToken(reader);
            }

            var commandResult = new CommandResult()
            {
                Principal = new ClaimsPrincipal(ids),
                SecurityTokens = new[] { token1, token2 }
            };

            var result = commandResult.CreateSessionSecurityToken();
            
            result.ValidFrom.Should().BeCloseTo(notBefore2, 1000);
            result.ValidTo.Should().BeCloseTo(notOnOrAfter, 1000);
        }

        [TestMethod]
        public void CommandResult_CreateSessionSecurityToken_RespectsValidFromAndValidToIfNotDefinedInCondition()
        {
            var xmlUnversalDateFomat = "yyyy-MM-ddTHH:mm:ssZ";
            var issueInstant = DateTime.UtcNow;
            var expectedNotOnOrAfter = issueInstant.Add(SessionSecurityTokenHandler.DefaultTokenLifetime);

            var response = string.Format(
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = ""AcsCommand_Run_SuccessfulResult"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>
                    https://idp.example.com
                </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion
                Version=""2.0"" ID=""Saml2Response_GetClaims_CreateIdentity_Assertion1""
                IssueInstant=""{0}"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                </saml2:Assertion>
            </saml2p:Response>", issueInstant.ToString(xmlUnversalDateFomat));

            var ids = new[] { new ClaimsIdentity("Federation"), new ClaimsIdentity("ClaimsAuthenticationManager") };
            ids[0].AddClaim(new Claim(ClaimTypes.NameIdentifier, "SomeUser", null, "https://idp.example.com"));
            ids[1].AddClaim(new Claim(ClaimTypes.Role, "RoleFromClaimsAuthManager", null, "ClaimsAuthenticationManagerMock"));

            SecurityToken token;
            var document = new XmlDocument();
            document.LoadXml(response);
            using (var reader = new XmlNodeReader(document.DocumentElement["saml2:Assertion"]))
            {
                token = MorePublicSaml2SecurityTokenHandler.DefaultInstance.ReadToken(reader);
            }

            var commandResult = new CommandResult()
            {
                Principal = new ClaimsPrincipal(ids),
                SecurityTokens = new[] { token }
            };

            var result = commandResult.CreateSessionSecurityToken();

            result.ValidFrom.Should().BeCloseTo(issueInstant, 1000);
            result.ValidTo.Should().BeCloseTo(expectedNotOnOrAfter, 1000);
        }
    }
}
