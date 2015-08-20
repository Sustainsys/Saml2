using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Linq;
using System.Threading;
using System.Globalization;

namespace Kentor.AuthServices.Tests
{
    public class NotReRunnableAttribute : Attribute { }

    [TestClass]
    public class AllTestOtherCultures
    {
        /// <summary>
        /// Some test have been written in a way that they fail on non english UI culture (if the
        /// languages are installed). This function runs *all* tests with a number of different
        /// UI cultures. It won't pass if any of the other tests fail.
        /// </summary>
        [TestMethod]
        [NotReRunnable]
        // This test takes a lot of time, as it effectively reruns the entire
        // test suite twice. There has been no localization bugs for long and
        // we've abandoned the coding style that caused the bug that initially 
        // prompted this test to be created. Leaving the test here though to
        // make it possible to enable again if needed.
        [Ignore]
        public void RunAllTestsWithOtherCultures()
        {
            var testClasses = (from t in Assembly.GetExecutingAssembly().DefinedTypes
                               where t.GetCustomAttribute<TestClassAttribute>() != null
                               select new
                               {
                                   Constructor = t.GetConstructor(new Type[] { }),
                                   ClassInit = t.GetMethods().Where(
                                   m => m.GetCustomAttribute<ClassInitializeAttribute>() != null).SingleOrDefault(),
                                   TestInit = t.GetMethods().Where(
                                   m => m.GetCustomAttribute<TestInitializeAttribute>() != null).SingleOrDefault(),
                                   TestCleanup = t.GetMethods().Where(
                                   m => m.GetCustomAttribute<TestCleanupAttribute>() != null).SingleOrDefault(),
                                   ClassCleanup = t.GetMethods().Where(
                                   m => m.GetCustomAttribute<ClassCleanupAttribute>() != null).SingleOrDefault(),
                                   TestMethods = t.GetMethods().Where(
                                   m => m.GetCustomAttribute<TestMethodAttribute>() != null
                                   && m.GetCustomAttribute<NotReRunnableAttribute>() == null
                                   && m.GetCustomAttribute<IgnoreAttribute>() == null).ToList()
                               }).ToList();

            // These are the environments I have access to. Please feel free to add and checking whatever
            // locales you have available.
            var cultures = new string[] { "en-US", "sv-SE" };

            var originalUICulture = Thread.CurrentThread.CurrentUICulture;

            var emtpyObjArray = new object[] { };

            try
            {
                foreach (var culture in cultures)
                {
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

                    foreach (var c in testClasses)
                    {
                        var instance = c.Constructor.Invoke(emtpyObjArray);
                        if (c.ClassInit != null)
                        {
                            c.ClassInit.Invoke(instance, emtpyObjArray);
                        }
                        foreach (var m in c.TestMethods)
                        {
                            if (c.TestInit != null)
                            {
                                c.TestInit.Invoke(instance, emtpyObjArray);
                            }
                            try
                            {
                                m.Invoke(instance, emtpyObjArray);
                            }
                            catch(TargetInvocationException ex)
                            {
                                if (!(ex.InnerException is AssertInconclusiveException))
                                {
                                    throw;
                                }
                            }
                            finally
                            {
                                if (c.TestCleanup != null)
                                {
                                    c.TestCleanup.Invoke(instance, emtpyObjArray);
                                }
                            }
                        }
                        if (c.ClassCleanup != null)
                        {
                            c.ClassCleanup.Invoke(instance, emtpyObjArray);
                        }
                    }
                }
            }
            finally
            {
                Thread.CurrentThread.CurrentUICulture = originalUICulture;
            }
        }
    }
}
