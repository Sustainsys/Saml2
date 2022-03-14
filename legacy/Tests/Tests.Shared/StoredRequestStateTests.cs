using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sustainsys.Saml2.Metadata;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace Sustainsys.Saml2.Tests
{
    [TestClass]
    public class StoredRequestStateTests
    {
        private void TestSerializeDeserialize(StoredRequestState subject)
        {
            var actual = new StoredRequestState(subject.Serialize());

            actual.Should().BeEquivalentTo(subject);
        }

        [TestMethod]
        public void StoredRequestState_Serialize_Deserialize_Idp()
        {
            TestSerializeDeserialize(new StoredRequestState(
                new EntityId("https://idp.example.com"),
                null, null, null));
        }

        [TestMethod]
        public void StoredRequestState_Serialize_Deserialize_ReturnUrl()
        {
            TestSerializeDeserialize(new StoredRequestState(
                null, new Uri("http://localhost/return"), null, null));
        }

        [TestMethod]
        public void StoredRequestState_Serialize_Deserialize_RelativeReturnUrl()
        {
            TestSerializeDeserialize(new StoredRequestState(
                null, new Uri("/return", UriKind.RelativeOrAbsolute), null, null));
        }

        [TestMethod]
        public void StoredRequestState_Serialize_Deserialize_MessageId()
        {
            TestSerializeDeserialize(new StoredRequestState(
                null, null, new Saml2Id("saml2Id"), null));
        }

        [TestMethod]
        public void StoredRequestState_Serialize_Deserialize_RelayData()
        {
            TestSerializeDeserialize(new StoredRequestState(
                null, null, null,
                new Dictionary<string, string>
                {
                    { "A", "B" },
                    { "C", "D" },
                    { "E", null }
                }));
        }
    }
}
