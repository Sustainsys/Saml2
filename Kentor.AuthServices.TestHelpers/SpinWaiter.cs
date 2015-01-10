using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.TestHelpers
{
    public static class SpinWaiter
    {
        private class TestResult
        {
            public readonly bool result;
            public readonly string errorMessage;
            public TestResult(bool result, string errorMessage)
            {
                this.result = result;
                this.errorMessage = errorMessage;
            }
        }
        public readonly static TimeSpan MaxWait = new TimeSpan(0, 0, 0, 0, 500);
        public static void While(Func<bool> condition, string failMessage = "Timeout passed without condition becoming false.")
        {
            While(() => new TestResult(condition(), failMessage));
        }

        public static void WhileEqual<T>(Func<T> actual, Func<T> expected, bool invertTest = false)
        {
            While(() =>
            {
                var expectedValue = expected();
                var actualValue = actual();
                bool testResult;
                if (expectedValue == null && actualValue == null)
                {
                    testResult = true;
                }
                else if (expectedValue != null && actualValue == null)
                {
                    testResult = false;
                }
                else if (expectedValue == null && actualValue != null)
                {
                    testResult = false;
                }
                else
                {
                    testResult = expectedValue.Equals(actualValue);
                }
                if (invertTest)
                {
                    testResult = !testResult;
                }
                return new TestResult(testResult, string.Format("Timeout passed without condition becoming false, expected {0}, last actual value was {1}", expectedValue, actualValue));
            });
        }

        private static void While(Func<TestResult> testFunction)
        {
            var waitStart = DateTime.UtcNow;
            var result = testFunction();
            while (result.result)
            {
                if (DateTime.UtcNow - waitStart > MaxWait)
                {
                    Assert.Fail(result.errorMessage);
                }
                result = testFunction();
            }
        }
    }
}
