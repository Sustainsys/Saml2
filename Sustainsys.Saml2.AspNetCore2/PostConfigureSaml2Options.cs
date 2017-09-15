using Kentor.AuthServices;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.AspNetCore2
{
    class PostConfigureSaml2Options : IPostConfigureOptions<Saml2Options>
    {
        public void PostConfigure(string name, Saml2Options options)
        {
            options.SPOptions.Logger = options.SPOptions.Logger ?? new NullLoggerAdapter();
        }
    }
}
