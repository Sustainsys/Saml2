using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Net;
using NSubstitute;
using System.Web;
using System.Security.Claims;

namespace Kentor.AuthServices.Tests
{
    using System.Globalization;
    using System.IdentityModel.Services;
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
                Saml2Response = (Saml2Response)null
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
                ID = ""AcsCommand_Run_SuccessfulResult"" Version=""2.0"" IssueInstant=""{0}"">
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

            var ids = new[] { new ClaimsIdentity("Federation") };
            ids[0].AddClaim(new Claim(ClaimTypes.NameIdentifier, "SomeUser", null, "https://idp.example.com"));

            Saml2SecurityToken token;
            var document = new XmlDocument();
            document.LoadXml(response);
            using (var reader = new XmlNodeReader(document.DocumentElement["saml2:Assertion"]))
            {
                token = (Saml2SecurityToken)MorePublicSaml2SecurityTokenHandler.DefaultInstance.ReadToken(reader);
            }

            var issuer = "https://idp.example.com";
            var destination = "http://destination.example.com/";
            var saml2Response = Substitute.For<Saml2Response>(issuer, SignedXmlHelper.TestCert, new Uri(destination), ids);
            saml2Response.Saml2SecurityTokens.Returns(new[] { token }.AsEnumerable());

            var commandResult = new CommandResult()
            {
                Principal = new ClaimsPrincipal(ids),
                Saml2Response = saml2Response
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
                ID = ""AcsCommand_Run_SuccessfulResult"" Version=""2.0"" IssueInstant=""{0}"">
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

            var ids = new[] { new ClaimsIdentity("Federation"), new ClaimsIdentity("Federation") };
            ids[0].AddClaim(new Claim(ClaimTypes.NameIdentifier, "SomeUser", null, "https://idp.example.com"));
            ids[1].AddClaim(new Claim(ClaimTypes.NameIdentifier, "SomeUser2", null, "https://idp.example.com"));

            Saml2SecurityToken token1;
            Saml2SecurityToken token2;
            var document = new XmlDocument();
            document.LoadXml(response);

            var assertionElementNodes =
                        document.DocumentElement.ChildNodes.Cast<XmlNode>()
                        .Where(node => node.NodeType == XmlNodeType.Element).Cast<XmlElement>()
                        .Where(xe => xe.LocalName == "Assertion" && xe.NamespaceURI == Saml2Namespaces.Saml2Name);

            using (var reader = new XmlNodeReader(assertionElementNodes.First()))
            {
                token1 = (Saml2SecurityToken)MorePublicSaml2SecurityTokenHandler.DefaultInstance.ReadToken(reader);
            }

            using (var reader = new XmlNodeReader(assertionElementNodes.Last()))
            {
                token2 = (Saml2SecurityToken)MorePublicSaml2SecurityTokenHandler.DefaultInstance.ReadToken(reader);
            }

            var issuer = "https://idp.example.com";
            var destination = "http://destination.example.com/";
            var saml2Response = Substitute.For<Saml2Response>(issuer, SignedXmlHelper.TestCert, new Uri(destination), ids);
            saml2Response.Saml2SecurityTokens.Returns(new[] { token1, token2 }.AsEnumerable());

            var commandResult = new CommandResult()
            {
                Principal = new ClaimsPrincipal(ids),
                Saml2Response = saml2Response
            };

            var result = commandResult.CreateSessionSecurityToken();
            
            result.ValidFrom.Should().BeCloseTo(notBefore2, 1000);
            result.ValidTo.Should().BeCloseTo(notOnOrAfter, 1000);
        }

        [TestMethod]
        public void CommandResult_CreateSessionSecurityToken_HandlesValidFromAndValidToIfNotDefinedInCondition()
        {
            var issueInstant = DateTime.UtcNow;
            var expectedNotOnOrAfter = issueInstant.Add(SessionSecurityTokenHandler.DefaultTokenLifetime);
            FederatedAuthentication.FederationConfiguration.IdentityConfiguration.DetectReplayedTokens = false;
            var response = string.Format(
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = ""AcsCommand_Run_SuccessfulResult"" Version=""2.0"" IssueInstant=""{0}"">
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
            </saml2p:Response>", issueInstant.ToSaml2DateTimeString());

            var ids = new[] { new ClaimsIdentity("Federation") };
            ids[0].AddClaim(new Claim(ClaimTypes.NameIdentifier, "SomeUser", null, "https://idp.example.com"));

            Saml2SecurityToken token;
            var document = new XmlDocument();
            document.LoadXml(response);
            using (var reader = new XmlNodeReader(document.DocumentElement["saml2:Assertion"]))
            {
                token = (Saml2SecurityToken)MorePublicSaml2SecurityTokenHandler.DefaultInstance.ReadToken(reader);
            }

            //var issuer = "https://idp.example.com";
            //var destination = "http://destination.example.com/";
            //var saml2Response = Substitute.For<Saml2Response>(issuer, SignedXmlHelper.TestCert, new Uri(destination), ids);
            //saml2Response.Saml2SecurityTokens.Returns(new[] { token }.AsEnumerable());

            var saml2Response = Saml2Response.Read(SignedXmlHelper.SignXml(response));
            saml2Response.Validate(SignedXmlHelper.TestCert);

            var commandResult = new CommandResult()
            {
                Principal = new ClaimsPrincipal(ids),
                Saml2Response = saml2Response
            };

            var result = commandResult.CreateSessionSecurityToken();

            result.ValidFrom.Should().BeCloseTo(issueInstant, 1000);
            result.ValidTo.Should().BeCloseTo(expectedNotOnOrAfter, 1000);
        }


        [TestMethod]
        public void CommandResult_CreateSessionSecurityToken_RespectsTheConfiguredLifetimeIfShorter()
        {
            var xmlUnversalDateFomat = "yyyy-MM-ddTHH:mm:ssZ";
            var notBefore = DateTime.UtcNow;
            var notOnOrAfter = DateTime.UtcNow.AddHours(1);

            var handler = (SessionSecurityTokenHandler)FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers[typeof(SessionSecurityToken)];
            var originalTokenLifetime = handler.TokenLifetime; // Memorize for later reset
            handler.TokenLifetime = TimeSpan.FromMinutes(20);
            var expectedValidTo = notBefore.Add(handler.TokenLifetime);

            var response = string.Format(
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = ""AcsCommand_Run_SuccessfulResult"" Version=""2.0"" IssueInstant=""{0}"">
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

            var ids = new[] { new ClaimsIdentity("Federation") };
            ids[0].AddClaim(new Claim(ClaimTypes.NameIdentifier, "SomeUser", null, "https://idp.example.com"));

            Saml2SecurityToken token;
            var document = new XmlDocument();
            document.LoadXml(response);
            using (var reader = new XmlNodeReader(document.DocumentElement["saml2:Assertion"]))
            {
                token = (Saml2SecurityToken)MorePublicSaml2SecurityTokenHandler.DefaultInstance.ReadToken(reader);
            }

            var issuer = "https://idp.example.com";
            var destination = "http://destination.example.com/";
            var saml2Response = Substitute.For<Saml2Response>(issuer, SignedXmlHelper.TestCert, new Uri(destination), ids);
            saml2Response.Saml2SecurityTokens.Returns(new[] { token }.AsEnumerable());

            var commandResult = new CommandResult()
            {
                Principal = new ClaimsPrincipal(ids),
                Saml2Response = saml2Response
            };

            var result = commandResult.CreateSessionSecurityToken();

            handler.TokenLifetime = originalTokenLifetime; // Reset to default -> otherwise other tests will fail
            
            result.ValidFrom.Should().BeCloseTo(notBefore, 1000);
            result.ValidTo.Should().BeCloseTo(expectedValidTo, 1000);
        }
    }
}
