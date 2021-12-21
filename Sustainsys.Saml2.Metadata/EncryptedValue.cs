using System;

namespace Sustainsys.Saml2.Metadata
{
    public class EncryptedValue
    {
        public Uri DecryptionCondition { get; set; }
        public EncryptedData EncryptedData { get; set; }
    }
}