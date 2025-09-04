using Microsoft.AspNetCore.DataProtection;

namespace Sustainsys.Saml2.Tests.Helpers;
internal class FakeDataProtectionProvider : IDataProtectionProvider
{
    public IDataProtector CreateProtector(string _) => new FakeDataProtector();

    public class FakeDataProtector : IDataProtector
    {
        private readonly byte[] prefix = [0xc0, 0xde, 0xda, 0x1a];

        public IDataProtector CreateProtector(string _) => new FakeDataProtector();
        public byte[] Protect(byte[] plaintext) => [.. prefix, .. plaintext];

        public byte[] Unprotect(byte[] protectedData)
        {
            if (!protectedData[..4].SequenceEqual(prefix))
            {
                throw new InvalidOperationException("Not fake data protected");
            }

            return protectedData[4..];
        }
    }
}
