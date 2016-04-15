using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// Class Saml2Scoping.
    /// </summary>
    public class Saml2Scoping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Saml2Scoping"/> class.
        /// </summary>
        /// <param name="idPEntries">The idp entries.</param>
        /// <param name="proxyCount">The proxy count.</param>
        /// <param name="requesterIds">The requester ids.</param>
        public Saml2Scoping(IList<Saml2IdPEntry> idPEntries, int proxyCount, IList<Saml2RequesterId> requesterIds)
        {
            IdPEntries = idPEntries;
            ProxyCount = proxyCount;
            RequesterIds = requesterIds;
        }
        /// <summary>
        /// Gets or sets the idp entries.
        /// </summary>
        /// <value>The idp entries.</value>
        public IList<Saml2IdPEntry> IdPEntries { get; }
        /// <summary>
        /// Gets or sets the proxy count.
        /// </summary>
        /// <value>The proxy count.</value>
        public int ProxyCount { get; }
        /// <summary>
        /// Gets or sets the requester ids.
        /// </summary>
        /// <value>The requester ids.</value>
        public IList<Saml2RequesterId> RequesterIds { get; }

        /// <summary>
        /// Create XElement for the Saml2Scoping.
        /// </summary>
        /// <returns>XElement.</returns>
        public XElement ToXElement()
        {
            var scopingElement = new XElement(Saml2Namespaces.Saml2P + "Scoping");

            if (ProxyCount > 0)
            {
                scopingElement.AddAttributeIfNotNullOrEmpty("ProxyCount", ProxyCount.ToString(CultureInfo.InvariantCulture));
            }
            if (IdPEntries != null && IdPEntries.Count > 0)
            {
                var idpListElement = new XElement(Saml2Namespaces.Saml2P + "IDPList");
                foreach (var saml2IdPEntry in IdPEntries)
                {
                    var idpEntryElement = new XElement(Saml2Namespaces.Saml2P + "IDPEntry");

                    if (!string.IsNullOrEmpty(saml2IdPEntry.ProviderId))
                    {
                        idpEntryElement.AddAttributeIfNotNullOrEmpty("ProviderID", saml2IdPEntry.ProviderId);
                    }
                    if (!string.IsNullOrEmpty(saml2IdPEntry.Name))
                    {
                        idpEntryElement.AddAttributeIfNotNullOrEmpty("Name", saml2IdPEntry.Name);
                    }
                    if (!string.IsNullOrEmpty(saml2IdPEntry.Location))
                    {
                        idpEntryElement.AddAttributeIfNotNullOrEmpty("Loc", saml2IdPEntry.Location);
                    }
                    idpListElement.Add(idpEntryElement);
                }
                scopingElement.Add(idpListElement);
            }
            if (RequesterIds != null && RequesterIds.Count > 0)
            {
                foreach (var saml2RequesterId in RequesterIds)
                {
                    scopingElement.Add(new XElement(Saml2Namespaces.Saml2P + "RequesterID", saml2RequesterId.Id));
                }
            }
            return scopingElement;
        }
    }
}

