using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Sustainsys.Saml2.Bindings;
using System;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Sustainsys.Saml2.Tests.Bindings;
public class FrontChannelBindingTests
{
    private class Subject : FrontChannelBinding
    {
        public override string Identification => throw new NotImplementedException();

        protected override Task DoBind(HttpResponse httpResponse, Saml2Message message) 
            => throw new NotImplementedException();
    }

    [Theory]
    [InlineData(null, "<xml/>", "Name*")]
    [InlineData("Name", null, "Xml*")]

    public void Bind_ValidateMessage(string? name, string? xml, string expectedMessage)
    {
        XmlDocument? xmlDocument = null;
        if (xml != null)
        {
            xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
        }

        var message = new Saml2Message()
        {
            Name = name!,
            Xml = xmlDocument!
        };

        var subject = new Subject();

        var httpResponse = Substitute.For<HttpResponse>();

        subject.Invoking(s => s.Bind(httpResponse, message))
            .Should().ThrowAsync<ArgumentException>().WithParameterName("message").WithMessage(expectedMessage);
    }
}
