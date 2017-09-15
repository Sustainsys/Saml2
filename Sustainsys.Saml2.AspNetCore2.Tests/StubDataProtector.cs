using Microsoft.AspNetCore.DataProtection;
using System.Linq;

namespace Sustainsys.Saml2.AspNetCore2.Tests
{
    class StubDataProtector : IDataProtector
    {
        byte[] IDataProtector.Protect(byte[] userData)
        {
            return Protect(userData);
        }

        public static byte[] Protect(byte[] userData)
        {
            return userData.Select((b, i) => 
                i < 6 ? b : (byte)(b ^ 42)
                ).ToArray();
        }

        byte[] IDataProtector.Unprotect(byte[] protectedData)
        {
            return Unprotect(protectedData);
        }

        public static byte[] Unprotect(byte[] protectedData)
        {
            return protectedData.Select((b, i) =>
                i < 6 ? b : (byte)(b ^ 42)
                ).ToArray();
        }

        public IDataProtector CreateProtector(string purpose)
        {
            return this;
        }
    }
}
