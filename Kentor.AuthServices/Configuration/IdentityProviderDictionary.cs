using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// A thread safe wrapper around a dictionary for the identity providers.
    /// </summary>
    /// <remarks>
    /// First I thought about using a ConcurrentDictionary, but that does not maintain
    /// any order of the added objects. Since the first idp added becomes the default idp,
    /// the order must be preserved. And there has to be queuing semantics if the first idp
    /// is dynamically loaded from a federation and later removed. Locks are simple and
    /// this part of the code shouldn't be that performance sensitive.
    /// </remarks>
    public class IdentityProviderDictionary
    {
        private Dictionary<EntityId, IdentityProvider> dictionary =
            new Dictionary<EntityId, IdentityProvider>(EntityIdEqualityComparer.Instance);

        /// <summary>
        /// Gets an idp from the entity id.
        /// </summary>
        /// <param name="entityID">entity Id to look up.</param>
        /// <returns>IdentityProvider</returns>
        public IdentityProvider this[EntityId entityID]
        {
            get
            {
                lock(dictionary)
                {
                    return dictionary[entityID];
                }
            }
            set
            {
                lock (dictionary)
                {
                    dictionary[entityID] = value;
                }
            }
        }

        /// <summary>
        /// The default identity provider; i.e. the first registered of the currently known.
        /// </summary>
        public IdentityProvider Default
        {
            get
            {
                lock (dictionary)
                {
                    return dictionary.Values.First();
                }
            }
        }

        /// <summary>
        /// Try to get the value of an idp with a given entity id.
        /// </summary>
        /// <param name="idpEntityId">Entity id to search for.</param>
        /// <param name="idp">The idp, if found.</param>
        /// <returns>True if an idp with the given entity id was found.</returns>
        public bool TryGetValue(EntityId idpEntityId, out IdentityProvider idp)
        {
            lock (dictionary)
            {
                return dictionary.TryGetValue(idpEntityId, out idp);
            }
        }

        /// <summary>
        /// Checks if there are no known identity providers.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                lock (dictionary)
                {
                    return dictionary.Count == 0;
                }
            }
        }
    }
}
