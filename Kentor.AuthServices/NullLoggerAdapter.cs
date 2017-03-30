using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Logger adapter that does nothing.
    /// </summary>
    public class NullLoggerAdapter : ILoggerAdapter
    {
        /// <summary>
        /// Write an error message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="ex">Exception to include in error message.</param>
        public void WriteError(string message, Exception ex)
        {
        }

        /// <summary>
        /// Write informational message.
        /// </summary>
        /// <param name="message">Message to write.</param>
        public void WriteInformation(string message)
        {
        }

        /// <summary>
        /// Write an informational message on the verbose level.
        /// </summary>
        /// <param name="message">Message to write</param>
        public void WriteVerbose(string message)
        {
        }
    }
}
