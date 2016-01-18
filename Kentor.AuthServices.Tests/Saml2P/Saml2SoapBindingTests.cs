using FluentAssertions;
using Kentor.AuthServices.Saml2P;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace Kentor.AuthServices.Tests.Saml2P
{
    [TestClass]
    public class Saml2SoapBindingTests
    {
        [TestMethod]
        public void Saml2SoapBinding_CreateSoapBody()
        {
            var message = "<payload>data</payload>";

            var expected = XElement.Parse(
@"<SOAP-ENV:Envelope
    xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"">
    <SOAP-ENV:Body>
        <payload>data</payload>
    </SOAP-ENV:Body>
</SOAP-ENV:Envelope>");

            var actual = XElement.Parse(Saml2SoapBinding.CreateSoapBody(message));

            actual.ShouldBeEquivalentTo(expected, opt => opt.IgnoringCyclicReferences());
        }

        [TestMethod]
        public void Saml2SoapBinding_ExtractBody()
        {
            string payload =
            "    <payload>\n"
            + "      <color>\n"
            + "        red\n"
            + "      </color>\n"
            + "      <color>\n"
            + "        green\n"
            + "      </color>\n"
            + "    </payload>\n";

            string soapMessage =
            "<SOAP-ENV:Envelope\n"
            + "xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\">\n"
            + "  <SOAP-ENV:Body>\n"
            + payload
            + "  </SOAP-ENV:Body>\n"
            + "</SOAP-ENV:Envelope>";

            Saml2SoapBinding.ExtractBody(soapMessage)
                .OuterXml.Should().Be(payload.Trim());
        }
    }
}
