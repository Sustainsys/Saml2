using System.Xml.Linq;

namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// The IdPEntry specifies a single identity provider trusted by the requester to authenticate the presenter    
    /// </summary>
    public class Saml2IdPEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Saml2IdPEntry"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="name">The name.</param>
        /// <param name="providerId">The provider identifier.</param>
        public Saml2IdPEntry(string location, string name, string providerId)
        {
            Location = location;
            Name = name;
            ProviderId = providerId;
        }
        /// <summary>
        /// Gets or sets the a URI reference representing the location of a profile-specific endpoint supporting 
        /// the authentication request protocol.The binding to be used must be understood from the profile of use.
        /// </summary>
        /// <value>The location.</value>
        public string Location { get; set; }
        /// <summary>
        /// Gets or sets a human-readable name for the identity provider
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the unique identifier of the identity provider.
        /// </summary>
        /// <value>The provider identifier.</value>
        public string ProviderId { get; set; }

        /// <summary>
        /// Create XElement for the Saml2IdPEntry.
        /// </summary>
        public XElement ToXElement()
        {
            var idpEntryElement = new XElement(Saml2Namespaces.Saml2P + "IDPEntry");

            idpEntryElement.AddAttributeIfNotNullOrEmpty("ProviderID", ProviderId);
            idpEntryElement.AddAttributeIfNotNullOrEmpty("Name", Name);
            idpEntryElement.AddAttributeIfNotNullOrEmpty("Loc", Location);

            return idpEntryElement;
        }
    }
}