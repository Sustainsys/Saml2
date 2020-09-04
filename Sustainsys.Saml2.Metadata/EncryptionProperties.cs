using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sustainsys.Saml2.Metadata
{
    public class EncryptionProperties
    {
        public string Id { get; set; }

        public ICollection<EncryptionProperty> Properties { get; private set; } =
            new Collection<EncryptionProperty>();
    }
}