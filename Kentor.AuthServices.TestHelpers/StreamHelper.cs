using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.TestHelpers
{
    /// <summary>
    /// Class that has utility functions for streams. 
    /// </summary>
    public static class StreamHelper
    {
        /// <summary>
        /// Converts a string to a stream.
        /// </summary>
        /// <param name="s">The input string that we want to convert to a stream.</param>
        /// <returns>A stream containing the string information.</returns>
        public static Stream ConvertString(String s)
        {
            MemoryStream memStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(memStream);
            writer.Write(s);
            writer.Flush();
            memStream.Seek(0, SeekOrigin.Begin);
            return memStream;
        }
    }
}
