using System;

namespace Sustainsys.Saml2.Metadata
{
    public class XEncEncryptionMethod
    {
        public int KeySize { get; set; }
        public byte[] OAEPparams { get; set; }
        public Uri Algorithm { get; set; }
    }
}