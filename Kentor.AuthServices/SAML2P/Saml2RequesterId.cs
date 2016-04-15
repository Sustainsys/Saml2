using System.Xml.Linq;

namespace Kentor.AuthServices.Saml2P
{
    public class Saml2RequesterId
    {
        public string Id { get; set; }

        public Saml2RequesterId(string id)
        {
            Id = id;
        }

        public XElement ToXElement()
        {
            return new XElement(Saml2Namespaces.Saml2P + "RequesterID", Id);
        }
    }
}