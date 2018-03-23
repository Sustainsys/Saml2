using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Security.Cryptography.X509Certificates;
using Sustainsys.Saml2.Metadata;

namespace Sustainsys.Saml2.Saml2P
{
    /// <summary>
    /// Abstract Saml2 StatusResponseType class.
    /// </summary>
    public abstract class Saml2StatusResponseType : ISaml2Message
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="status">Status of the response</param>
        protected Saml2StatusResponseType(Saml2StatusCode status)
        {
            Status = status;
        }

        /// <summary>
        /// The destination of the message.
        /// </summary>
        public Uri DestinationUrl { get; set; }

        /// <summary>
        /// Issuer of the message.
        /// </summary>
        public EntityId Issuer { get; set; }

        /// <summary>
        /// Id of request message, if this message is a response to a previous
        /// request.
        /// </summary>
        public Saml2Id InResponseTo { get; set; }

        /// <summary>
        /// The name of the message to use in a query string or form input
        /// field. Typically "SAMLRequest" or "SAMLResponse".
        /// </summary>
        public string MessageName { get; } = "SAMLResponse";

        /// <summary>
        /// RelayState attached to the message.
        /// </summary>
        /// <remarks>Strictly speaking, this is not part of the message,
        /// but it is delivered together with the message so we need to keep
        /// track of it together with a message.</remarks>
        public string RelayState { get; set; }

        /// <summary>
        /// Certificate used to sign the message with during binding, according
        /// to the signature processing rules of each binding.
        /// </summary>
        public X509Certificate2 SigningCertificate { get; set; }

        /// <summary>
        /// The signing algorithm to use when signing the message during binding, 
        /// according to the signature processing rules of each binding.
        /// </summary>
        /// <value>The signing algorithm.</value>
        public string SigningAlgorithm { get; set; }

        /// <summary>
        /// Status code of the message.
        /// </summary>
        public Saml2StatusCode Status { get; }

        /// <summary>
        /// Id of the message.
        /// </summary>
        public Saml2Id Id { get; } = new Saml2Id();

        /// <summary>
        /// Issue instant.
        /// </summary>
        public DateTime IssueInstant { get; } = DateTime.UtcNow;

        /// <summary>
        /// Serializes the message into wellformed Xml.
        /// </summary>
        /// <returns>string containing the Xml data.</returns>
        public abstract string ToXml();
    }
}