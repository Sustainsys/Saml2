using Sustainsys.Saml2.Metadata;
using System.Collections.Generic;

namespace Sustainsys.Saml2.Configuration
{
    /// <summary>
    /// The interface for identity provider dictionary
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "It works like dictionary, even though it doesn't implement the full interface.")]
    public interface IIdentityProviderDictionary
    {
        /// <summary>
        /// Gets an idp from the entity id.
        /// </summary>
        /// <param name="entityId">entity Id to look up.</param>
        /// <returns>IdentityProvider</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
        IdentityProvider this[EntityId entityId]
        {
            get;
            set;
        }

        /// <summary>
        /// Add an identity provider to the collection..
        /// </summary>
        /// <param name="idp">Identity provider to add.</param>
        void Add(IdentityProvider idp);

        /// <summary>
        /// The default identity provider; i.e. the first registered of the currently known.
        /// </summary>
        IdentityProvider Default
        {
            get;
        }

        /// <summary>
        /// Gets all currently known identity providers. Note that the returned
        /// enumeration is a copy to avoid race conditions.
        /// </summary>
        IEnumerable<IdentityProvider> KnownIdentityProviders
        {
            get;
        }

        /// <summary>
        /// Try to get the value of an idp with a given entity id.
        /// </summary>
        /// <param name="idpEntityId">Entity id to search for.</param>
        /// <param name="idp">The idp, if found.</param>
        /// <returns>True if an idp with the given entity id was found.</returns>
        bool TryGetValue(EntityId idpEntityId, out IdentityProvider idp);

        /// <summary>
        /// Checks if there are no known identity providers.
        /// </summary>
        bool IsEmpty
        {
            get;
        }

        /// <summary>
        /// Removes the idp with the given entity id, if present. If no such
        /// entity is found, nothing is done.
        /// </summary>
        /// <param name="idp">EntityId of idp to remove.</param>
        void Remove(EntityId idp);


        /// <summary>
        // Used by tests.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IdentityProvider this[int index]
        {
            get;
        }
    }
}
