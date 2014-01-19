using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Tests
{
    /// <summary>
    /// Helper class for testing standard exception constructors.
    /// </summary>
    static class ExceptionTestHelpers
    {
        public static void TestDefaultCtor<TException>()
            where TException: Exception, new()
        {
            new TException();

            // Deliberately no asserts - just testing that default ctor can
            // be run and exists.
        }

    }
}
