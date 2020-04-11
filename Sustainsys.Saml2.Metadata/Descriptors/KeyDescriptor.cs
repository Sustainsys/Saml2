using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sustainsys.Saml2.Metadata.Descriptors
{
    public class KeyDescriptor
    {
        public DSigKeyInfo KeyInfo { get; set; }
        public KeyType Use { get; set; } = KeyType.Unspecified;

        public ICollection<EncryptionMethod> EncryptionMethods { get; private set; } =
            new Collection<EncryptionMethod>();

        public KeyDescriptor()
        {
        }
    }
}