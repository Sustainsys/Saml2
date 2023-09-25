using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Text.Encodings.Web;

namespace Sustainsys.Saml2.AspNetCore.Tests;
public class Saml2HandlerTests
{
    private static async Task<(Saml2Handler subject, HttpContext httpContext)> CreateSubject(Saml2Options options)
    {
        var optionsMonitor = Substitute.For<IOptionsMonitor<Saml2Options>>();
        optionsMonitor.Get(Arg.Any<string>()).Returns(options);

        var loggerFactory = Substitute.For<ILoggerFactory>();

        var systemClock = Substitute.For<ISystemClock>();

        var handler = new Saml2Handler(
            optionsMonitor,
            loggerFactory,
            UrlEncoder.Default,
            systemClock);

        var scheme = new AuthenticationScheme("Saml2", "Saml2", typeof(Saml2Handler));

        var httpContext = Substitute.For<HttpContext>();

        await handler.InitializeAsync(scheme, httpContext);

        return (handler, httpContext);
    }

    private static Saml2Options CreateOptions() => new()
    {
    };

    //[Fact]
    //public async Task ChallengeReturnsRedirect()
    //{
    //    var options = Saml2HandlerTests.CreateOptions();

    //    (var subject, var httpContext) = await Saml2HandlerTests.CreateSubject(options);

    //    var props = new AuthenticationProperties();

    //    await subject.ChallengeAsync(props);

    //    static void validateLocation(string location)
    //    {
    //        1.Should().Be(2);
    //    }

    //    httpContext.Response.Received().Redirect(Arg.Do<string>(validateLocation));
    //}
}
