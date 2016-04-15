using System;
using System.Xml.Linq;

namespace Kentor.AuthServices.Saml2P
{

    /// <summary>
    /// Saml2RequesterId - represents a requesting entity on whose behalf the requester is acting.
    /// </summary>
    public class Saml2RequesterId
    {
        private readonly Uri _id;

        /// <summary>
        /// Initializes a new instance of the <see cref="Saml2RequesterId"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public Saml2RequesterId(Uri id)
        {
            _id = id;
        }

        /// <summary>
        /// Create XElement for the <see cref="Saml2RequesterId"/> class.
        /// </summary>
        public XElement ToXElement()
        {
            return new XElement(Saml2Namespaces.Saml2P + "RequesterID", _id);
        }
    }
}