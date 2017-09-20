using FluentAssertions;
using Kentor.AuthServices.WebSso;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Tests.Owin
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
    }
}
