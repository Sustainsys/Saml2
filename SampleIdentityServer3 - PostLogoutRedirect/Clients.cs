using IdentityServer3.Core.Models;
using System.Collections.Generic;

namespace SampleIdentityServer3.PostLogoutRedirect
{
    static class Clients
    {
        public static IList<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    Enabled = true,
                    ClientName = "OpenidClient",
                    ClientId = "OpenidClient",
                    Flow = Flows.Implicit,

                    RedirectUris = new List<string>
                    {
                        Startup.domain
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        Startup.domain
                    },
                    AllowAccessToAllScopes = true,

                    
                }
            };
        }
    }
}