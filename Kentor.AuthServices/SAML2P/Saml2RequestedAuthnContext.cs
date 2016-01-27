using Kentor.AuthServices.Configuration;
using System;

namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// Configuration of RequestedAuthnContext
    /// </summary>
    public class Saml2RequestedAuthnContext
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="requestedAuthnContextElement">Config element to load.</param>
        public Saml2RequestedAuthnContext(RequestedAuthnContextElement requestedAuthnContextElement)
        {
            if(requestedAuthnContextElement == null)
            {
                throw new ArgumentNullException(nameof(requestedAuthnContextElement));
            }
            
            if(!string.IsNullOrEmpty(requestedAuthnContextElement.AuthnContextClassRef))
            {
                ClassRef = new Uri(
                    !requestedAuthnContextElement.AuthnContextClassRef.Contains(":")
                    ? "urn:oasis:names:tc:SAML:2.0:ac:classes:" + requestedAuthnContextElement.AuthnContextClassRef
                    : requestedAuthnContextElement.AuthnContextClassRef);
            }

            Comparison = requestedAuthnContextElement.Comparison;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="classRef">AuthnContextClassRef</param>
        /// <param name="comparison">Comparison</param>
        public Saml2RequestedAuthnContext(Uri classRef, AuthnContextComparisonType comparison)
        {
            ClassRef = classRef;
            Comparison = comparison;
        }

        /// <summary>
        /// Authentication context class reference.
        /// </summary>
        public Uri ClassRef { get; }

        /// <summary>
        /// Comparison method.
        /// </summary>
        public AuthnContextComparisonType Comparison { get; }
    }
}