using Microsoft.Extensions.Options;
using Sustainsys.Saml2.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Post configure options for Saml2. Validates config and sets the default 
/// for parameters that have not been set.
/// </summary>
public class Saml2PostConfigureOptions : IPostConfigureOptions<Saml2Options>
{
    /// <summary>
    /// Validates config and sets the default for parameters that have not
    /// been set
    /// </summary>
    /// <param name="name">Name of the scheme</param>
    /// <param name="options">Options to validate and set defaults on</param>
    public void PostConfigure(string? name, Saml2Options options)
    {

    }
}