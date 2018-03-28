using FluentAssertions;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.WebSso;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Owin.Tests
{
    [TestClass]
    public class HttpRequestDataTests
    {
        // Not really a test specific to Owin, but it uses the StubDataProtector so
        // I just put it in the owin tests package out of laziness.
        [TestMethod]
        public void HttpRequestData_Ctor_Deserialize_StoredRequestState()
        {
            var url = new Uri("http://example.com:42/ApplicationPath/Path?RelayState=Foo");
            string appPath = "/ApplicationPath";

            var storedRequestData = new StoredRequestState(
                    new EntityId("http://idp.example.com"),
                    new Uri("http://sp.example.com/loggedout"),
                    new Saml2Id("id123"),
                    null);

            var cookies = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>(
                    StoredRequestState.CookieNameBase +"Foo",
                    HttpRequestData.ConvertBinaryData(
                            StubDataProtector.Protect(storedRequestData.Serialize())))
            };

            var subject = new HttpRequestData(
                 "GET",
                 url,
                 appPath,
                 Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>(),
                 cookies,
                 StubDataProtector.Unprotect);

            subject.StoredRequestState.Should().BeEquivalentTo(storedRequestData);
        }
    }
}
