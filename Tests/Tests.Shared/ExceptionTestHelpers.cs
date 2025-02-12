using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FluentAssertions;

namespace Sustainsys.Saml2.Tests
{
    /// <summary>
    /// Helper class for testing standard exception constructors.
    /// </summary>
    static class ExceptionTestHelpers
    {
        public static void TestDefaultCtor<TException>()
            where TException : Exception, new()
        {
            new TException();

            // Deliberately no asserts - just testing that default ctor can
            // be run and exists.
        }
    }
}
