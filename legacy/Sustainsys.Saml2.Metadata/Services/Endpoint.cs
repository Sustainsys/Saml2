using System;

namespace Sustainsys.Saml2.Metadata.Services
{
    public class Endpoint
    {
        public Uri Binding { get; set; }
        public Uri Location { get; set; }
        public Uri ResponseLocation { get; set; }

        public Endpoint()
        {
        }

        public Endpoint(Uri binding, Uri location)
        {
            Binding = binding;
            Location = location;
        }
    }
}