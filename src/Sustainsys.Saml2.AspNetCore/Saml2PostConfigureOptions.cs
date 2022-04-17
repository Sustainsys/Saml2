using Microsoft.Extensions.Options;
using Sustainsys.Saml2.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection;

public class Saml2PostConfigureOptions : IPostConfigureOptions<Saml2Options>
{
    public void PostConfigure(string name, Saml2Options options)
    {

    }
}