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

        public static string Unprotect(string protectedData)
        {
            var unescaped = protectedData.Replace('_', '/').Replace('-', '+').Replace('.', '=');

            return Encoding.UTF8.GetString(Unprotect(Convert.FromBase64String(unescaped)));
        }

        public static string Protect(string userData, bool checkEscaping = true)
        {
            var base64Data = Convert.ToBase64String(
                Protect(Encoding.UTF8.GetBytes(userData)));

            if (checkEscaping)
            {
                base64Data.Any(c => c == '+').Should().BeTrue("resulting Base64 data should contain + to test escaping");
                base64Data.Any(c => c == '/').Should().BeTrue("resulting Base64 data should contain / to test escaping");
                base64Data.Any(c => c == '=').Should().BeTrue("resulting Base64 data should contain = to test escaping");
            }

            return base64Data.Replace('/', '_').Replace('+', '-').Replace('=', '.');
        }
    }
}
