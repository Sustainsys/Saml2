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
    }
}