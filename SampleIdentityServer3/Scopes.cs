using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentityServer3.Core.Models;

namespace SampleIdentityServer3
{
    static class Scopes
    {
        public static IList<Scope> Get()
        {
            return new List<Scope>
            {
                new Scope
                {
                    Name="api1"
                }
            };
        }
    }
}