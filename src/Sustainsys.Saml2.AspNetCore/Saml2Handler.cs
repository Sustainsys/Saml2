using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace Sustainsys.Saml2.AspNetCore;

/// <summary>
/// Saml2 authentication handler
/// </summary>
public class Saml2Handler : RemoteAuthenticationHandler<Saml2Options>
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="options">Options</param>
    /// <param name="logger">Logger factory</param>
    /// <param name="encoder">Url encoder</param>
    /// <param name="clock">System Clock</param>
    public Saml2Handler(
        IOptionsMonitor<Saml2Options> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) 
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
    {
        return Task.FromResult(new HandleRequestResult());
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        throw new NotImplementedException();
    }
}