using IdentityServer3.Core.Models;
using System.Collections.Generic;

namespace SampleIdentityServer3
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
                    ClientName = "Server Side client",
                    ClientId = "serverside",
                    Flow = Flows.Hybrid,
                    ClientSecrets = new List<Secret>()
                    {
                        new Secret("somesecret")
                    },

                    RedirectUris = new List<string>
                    {
                        "http://localhost:4589/"
                    },

                    AllowAccessToAllScopes = true,

                    LogoutUri = "http://localhost:4589/ServerSide-Logout"
                },
                new Client
                {
                    Enabled = true,
                    ClientName = "Client Side Client",
                    ClientId = "clientside",
                    Flow = Flows.Implicit,

                    RedirectUris = new List<string>
                    {
                        "http://localhost:4589/"
                    },

                    AllowAccessToAllScopes = true,
                }
            };
        }
    }
}