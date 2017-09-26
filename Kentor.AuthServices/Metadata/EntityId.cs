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