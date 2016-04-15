using System.Xml.Linq;

namespace Kentor.AuthServices.Saml2P
{
    public class Saml2IdPEntry
    {
        public Saml2IdPEntry(string location, string name, string providerId)
        {
            Location = location;
            Name = name;
            ProviderId = providerId;
        }
        public string Location { get; set; }
        public string Name { get; set; }
        public string ProviderId { get; set; }

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