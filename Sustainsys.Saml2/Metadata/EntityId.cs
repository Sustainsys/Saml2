// For the full framework, we're using System.IdentityModel for all metadata
// related stuff. But that doesn't exist in the Microsoft.IdentityModel
// packages (yet). So stub up the most important parts directly in the library
// here if we're targeting NetStandard.

#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Text;

namespace Kentor.AuthServices.Metadata
{
    public class EntityId
    {
        public EntityId(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}
#endif