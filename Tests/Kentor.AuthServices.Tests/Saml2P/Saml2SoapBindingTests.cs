using FluentAssertions;
using Kentor.AuthServices.Saml2P;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
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

        [TestMethod]
        public void Saml2SoapBinding_SendSoapRequest_NullCheckDestination()
        {
            string payload = "Doesn't matter";

            Action a = () => Saml2SoapBinding.SendSoapRequest(payload, null, null);

            a.ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("destination");
        }

        [TestMethod]
        public void Saml2SoapBinding_SendSoapRequest_VerifiesUriIsRemote()
        {
            File.Exists("c:\\Kentor-Unit-Test.txt").Should().BeFalse(
                "a file c:\\Kentor-Unit-Test.txt already exists, preventing test from running");

            var payload = "Doesn't matter";
            var destination = new Uri("file://c:/Kentor-Unit-Test.txt");

            Action a = () => Saml2SoapBinding.SendSoapRequest(payload, destination, null);

            a.ShouldThrow<ArgumentException>()
                .WithMessage("*file*");
        }

        [TestMethod]
        public void Saml2SoapBinding_SendSoapRequest_AllowsHttp()
        {
            var payload = "Doesn't matter";
            var destination = new Uri("http://localhost/Endpoint");

            Action a = () => Saml2SoapBinding.SendSoapRequest(payload, destination, null);

            // Destination is not listening, but we should get an exception that shows it
            // at least tried to connect there.
            a.ShouldThrow<WebException>();
        }

        [TestMethod]
        public void Saml2SoapBinding_SendSoapRequest_AllowsHttps()
        {
            var payload = "Doesn't matter";
            var destination = new Uri("https://localhost/Endpoint");

            Action a = () => Saml2SoapBinding.SendSoapRequest(payload, destination, null);

            // Destination is not listening, but we should get an exception that shows it
            // at least tried to connect there.
            a.ShouldThrow<WebException>();
        }
    }
}
