using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// Saml2Scoping specifies a set of identity providers trusted by the requester to authenticate the presenter, as well as 
    /// limitations and context related to proxying of the authentication request message to subsequent identity 
    /// providers by the responder.
    /// </summary>
    public class Saml2Scoping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Saml2Scoping"/> class.
        /// </summary>
        /// <param name="idPEntries">The advisory list of identity providers.</param>
        /// <param name="proxyCount">The proxy count.</param>
        /// <param name="requesterIds">The requester ids.</param>
        public Saml2Scoping(IList<Saml2IdPEntry> idPEntries, int? proxyCount, IList<Saml2RequesterId> requesterIds)
        {
            IdPEntries = idPEntries;
            ProxyCount = proxyCount;
            RequesterIds = requesterIds;
        }
        /// <summary>
        /// Gets or sets advisory list of identity providers and associated information that 
        /// the requester deems acceptable to respond to the request.
        /// </summary>
        /// <value>The idp entries.</value>
        public IList<Saml2IdPEntry> IdPEntries { get; }
        /// <summary>
        /// Specifies the number of proxying indirections permissible between the identity provider that receives
        /// the authentication request and the identity provider who ultimately authenticates the principal.
        /// A count of zero permits no proxying, while omitting (null) this attribute expresses no such restriction.
        /// </summary>
        /// <value>The proxy count.</value>
        public int? ProxyCount { get; }
        /// <summary>
        /// Gets or sets the set of requesting entities on whose behalf the requester is acting. 
        /// Used to communicate the chain of requesters when proxying occurs.
        /// </summary>
        /// <value>The requester ids.</value>
        public IList<Saml2RequesterId> RequesterIds { get; }

        /// <summary>
        /// Create XElement for the Saml2Scoping.
        /// </summary>
        public XElement ToXElement()
        {
            var scopingElement = new XElement(Saml2Namespaces.Saml2P + "Scoping");

            if (ProxyCount > 0)
            {
                scopingElement.AddAttributeIfNotNullOrEmpty("ProxyCount", ProxyCount?.ToString(CultureInfo.InvariantCulture));
            }

            if (IdPEntries != null && IdPEntries.Count > 0)
            {
                scopingElement.Add(new XElement(Saml2Namespaces.Saml2P + "IDPList", IdPEntries.Select(x => x.ToXElement())));
            }

            if (RequesterIds != null && RequesterIds.Count > 0)
            {
                scopingElement.Add(RequesterIds.Select(x => x.ToXElement()));
            }

            return scopingElement;
        }
    }
}

