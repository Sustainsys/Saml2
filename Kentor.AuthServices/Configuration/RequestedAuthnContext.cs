using System;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Configuration of RequestedAuthnContext
    /// </summary>
    public class RequestedAuthnContext
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="requestedAuthnContextElement">Config element to load.</param>
        public RequestedAuthnContext(RequestedAuthnContextElement requestedAuthnContextElement)
        {
            if(requestedAuthnContextElement == null)
            {
                throw new ArgumentNullException(nameof(requestedAuthnContextElement));
            }
            
            ClassRef = new Uri(
                !requestedAuthnContextElement.AuthnContextClassRef.Contains(":")
                ? "urn:oasis:names:tc:SAML:2.0:ac:classes:" + requestedAuthnContextElement.AuthnContextClassRef
                : requestedAuthnContextElement.AuthnContextClassRef);

            Comparison = requestedAuthnContextElement.Comparison;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="classRef">AuthnContextClassRef</param>
        /// <param name="comparison">Comparison</param>
        public RequestedAuthnContext(Uri classRef, AuthnContextComparisonType comparison)
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