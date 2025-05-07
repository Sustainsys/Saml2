using Microsoft.AspNetCore.Http;
using NSubstitute;
using Sustainsys.Saml2.Bindings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sustainsys.Saml2.Tests.Bindings;
public class HttpPostBindingTests
{
    [Fact]
    public void Uri()
    {
        new HttpPostBinding().Identifier.Should().Be("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST");
    }

    [Fact]
    public void CanUnbind_RejectsGet()
    {
        var request = Substitute.For<HttpRequest>();
        request.Method = "GET";

        // Shouldn't happen on GET, but it's a test so let's be weird.
        request.Form = new FormCollection(new()
        {
            { "SAMLRequest", "foo" }
        });

        var subject = new HttpPostBinding();
        subject.CanUnbind(request).Should().BeFalse();
    }

    [Theory]
    [InlineData("SAMLRequest", true)]
    [InlineData("SAMLResponse", true)]
    [InlineData("id_token", false)] // Naahh... that would be OIDC
    public void CanUnbind_Accepts(string formKey, bool expected)
    {
        var request = Substitute.For<HttpRequest>();
        request.Method = "POST";
        request.Form = new FormCollection(new()
        {
            { formKey, "foo" }
        });

        var subject = new HttpPostBinding();
        subject.CanUnbind(request).Should().Be(expected);
    }

    [Theory]
    [InlineData("SAMLRequest")]
    [InlineData("SAMLResponse")]
    public async Task Unbind(string messageName)
    {
        var request = Substitute.For<HttpRequest>();
        request.PathBase = "/subdir";
        request.Path = "/Saml2/Acs";
        request.Method = "POST";
        request.Form = new FormCollection(new()
        {
            { messageName, "PHhtbD48YS8+PC94bWw+" },
            { "RelayState", "ABC123" }
        });

        var subject = new HttpPostBinding();

        Func<string, Task<Saml2Entity>> getEntity = str =>
            Task.FromResult<Saml2Entity>(new IdentityProvider());

        var actual = await subject.UnbindAsync(request, getEntity);

        var xd = new XmlDocument();
        xd.LoadXml("<xml><a/></xml>");

        var expected = new Saml2Message
        {
            Destination = "/subdir/Saml2/Acs",
            Name = messageName,
            RelayState = "ABC123",
            Xml = xd.DocumentElement!
        };

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Unbind_BothResponseAndRequest()
    {
        var request = Substitute.For<HttpRequest>();
        request.Path = "/Saml2/Acs";
        request.Method = "POST";
        request.Form = new FormCollection(new()
        {
            { "SAMLResponse", "PHhtbD48Yi8+PC94bWw+" },
            { "SAMLRequest", "PHhtbD48YS8+PC94bWw+" },
            { "RelayState", "ABC123" }
        });

        var subject = new HttpPostBinding();

        Func<string, Task<Saml2Entity>> getEntity = str =>
            Task.FromResult<Saml2Entity>(new IdentityProvider());

        await subject.Invoking(s => s.UnbindAsync(request, getEntity))
            .Should().ThrowAsync<ArgumentException>().WithMessage("*both*");
    }
}