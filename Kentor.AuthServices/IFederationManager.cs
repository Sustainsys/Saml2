using System.Collections.Generic;
using System.IdentityModel.Metadata;

namespace Kentor.AuthServices
{
    /// <summary>
    /// A federation manager - configuration entry point to allow extensibility
    /// into programmatic identity providers.
    /// </summary>
    public interface IFederationManager
    {
        /// <summary>
        /// Are unsolicited responses from the idps in the federation allowed?
        /// </summary>
        bool AllowUnsolicitedAuthnResponse { get; }

        /// <summary>
        /// Loads identity providers.
        /// </summary>
        /// <returns>A collection of identity providers</returns>
        EntitiesDescriptor Load();
    }
}
