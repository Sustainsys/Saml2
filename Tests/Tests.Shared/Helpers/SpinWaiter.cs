using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Tests.Helpers
{
    public static class SpinWaiter
    {
        [DebuggerDisplay("TestResult: {result}, {errorMessage}")]
        private struct TestResult
        {
            public readonly bool result;
            public readonly string errorMessage;
            public TestResult(bool result, string errorMessage)
            {
                this.result = result;
                this.errorMessage = errorMessage;
            }
        }
        public readonly static TimeSpan MaxWait = new TimeSpan(0, 0, 0, 5);
        public static void While(Func<bool> condition, string failMessage = "Timeout passed without condition becoming false.")
        {
            While(() => new TestResult(condition(), failMessage), true);
        }

        public static void WhileEqual<T>(Func<T> v1, Func<T> v2)
        {
            While(v1, v2, true);
        }

        public static void WhileNotEqual<T>(Func<T> v1, Func<T> v2)
        {
            While(v1, v2, false);
        }

        private static void While<T>(Func<T> v1, Func<T> v2, bool spinWhileValue)
        {
            While(() =>
            {
                var value2 = v2();
                var value1 = v1();
                bool testResult;

                if (value2 == null && value1 == null)
                {
                    testResult = true;
                }
                else if (value2 != null && value1 == null)
                {
                    testResult = false;
                }
                else if (value2 == null && value1 != null)
                {
                    testResult = false;
                }
                else
                {
                    testResult = value2.Equals(value1);
                }
                return new TestResult(testResult, string.Format("Timeout passed without condition becoming false, expected {0}, last actual value was {1}", value2, value1));
            }, spinWhileValue);
        }

        private static void While(Func<TestResult> testFunction, bool spinWhileValue)
        {
            var waitStart = DateTime.UtcNow;
            var result = testFunction();
            while (result.result == spinWhileValue)
            {
                if (DateTime.UtcNow - waitStart > MaxWait)
                {
                    Assert.Fail(result.errorMessage);
                }
                result = testFunction();
                
                // Short sleep to yield the CPU, giving the background tasks
                // a chance to run. Not a problem on multi-core CPUs, but for
                // single (which the build server at appveyour appears to be)
                // the tests fail when the main thread occupies the CPU.
                Thread.Sleep(1);
            }
        }
    }
}
