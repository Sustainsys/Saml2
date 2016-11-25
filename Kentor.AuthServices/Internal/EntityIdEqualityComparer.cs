using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Internal
{
    class EntityIdEqualityComparer : IEqualityComparer<EntityId>
    {
        private static EntityIdEqualityComparer instance = new EntityIdEqualityComparer();
        public static EntityIdEqualityComparer Instance
        {
            get
            {
                return instance;
            }
        }

        public bool Equals(EntityId x, EntityId y)
        {
            if(x == null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            if(y == null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            return x.Id == y.Id;
        }

        public int GetHashCode(EntityId obj)
        {
            if(obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            if(obj.Id == null)
            {
                return 117; // Whatever value, as long as we return the same each time.
            }
            return obj.Id.GetHashCode();
        }
    }
}
