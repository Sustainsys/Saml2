using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Tests
{
    /// <summary>
    /// Dummy class for referencing at least one type in each dll file that needs to be deployed when running mstest.exe
    /// This is needed as mstest does not deploy dll files from the bin folder to the mstest\out folder when no type from
    /// the file is actually referenced from code
    /// </summary>
    public static class _TestReferencesForMsbuild
    {
        /// <summary>
        /// Just return any type from Microsoft.Owin.Diagnostics.dll to make sure dll is deployed to test folder by mstest.exe
        /// </summary>
        public static Microsoft.Owin.Diagnostics.ErrorPageMiddleware ref1 { get { throw new NotImplementedException(); } }

        /// <summary>
        /// Just return any type from Microsoft.Owin.Host.HttpListener.dll to make sure dll is deployed to test folder by mstest.exe
        /// </summary>
        public static Microsoft.Owin.Host.HttpListener.OwinHttpListener ref2 { get { throw new NotImplementedException(); } }

        /// <summary>
        /// Just return any type from System.Web.Helpers.dll to make sure dll is deployed to test folder by mstest.exe
        /// </summary>
        public static System.Web.Helpers.Chart ref3 { get { throw new NotImplementedException(); } }

    }
}
