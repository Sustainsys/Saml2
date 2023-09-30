using Microsoft.AspNetCore.Http;
using NSubstitute;
using Sustainsys.Saml2.Bindings;
using System;
using System.Threading.Tasks;
using System.Xml;

namespace Sustainsys.Saml2.Tests.Bindings;
public class FrontChannelBindingTests
{
    private class Subject : FrontChannelBinding
    {
        public override string Identification => throw new NotImplementedException();

        public override Task<Saml2Message> UnbindAsync(
            HttpRequest httpRequest,
            Func<string, Task<IdentityProvider>> getIdentityProviderAsync) 
            => throw new NotImplementedException();
        
        protected override Task DoBindAsync(HttpResponse httpResponse, Saml2Message message) 
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
            Xml = xmlDocument!.DocumentElement!
        };

        var subject = new Subject();

        var httpResponse = Substitute.For<HttpResponse>();

        subject.Invoking(s => s.BindAsync(httpResponse, message))
            .Should().ThrowAsync<ArgumentException>().WithParameterName("message").WithMessage(expectedMessage);
    }
}
