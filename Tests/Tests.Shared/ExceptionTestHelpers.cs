using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
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

        public static void TestSerializationCtor<TException>()
            where TException : Exception, new()
        {
            var original = new TException();
            var msg = "Some Message";
            typeof(Exception).GetField("_message",
                BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(original, msg);

            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();

                formatter.Serialize(ms, original);

                ms.Seek(0, SeekOrigin.Begin);

                var deserialized = (TException)formatter.Deserialize(ms);

                deserialized.Message.Should().Be(msg);
                deserialized.Should().BeEquivalentTo(original);
            }

        }
    }
}
