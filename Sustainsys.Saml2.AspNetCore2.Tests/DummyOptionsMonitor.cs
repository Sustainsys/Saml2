using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.AspNetCore2.Tests
{
    class DummyOptionsMonitor : IOptionsMonitor<Saml2Options>
    {
        public DummyOptionsMonitor(Saml2Options options)
        {
            new PostConfigureSaml2Options().PostConfigure(null, options);
            CurrentValue = options;
        }

        public Saml2Options CurrentValue { get; }

        public Saml2Options Get(string name)
        {
            return CurrentValue;
        }

        public IDisposable OnChange(Action<Saml2Options, string> listener)
        {
            throw new NotImplementedException();
        }
    }
}
