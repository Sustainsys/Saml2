using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Interface for an adapter around the logging framework used on each
    /// platform.
    /// </summary>
    public interface ILoggerAdapter
    {
        /// <summary>
        /// Write informational message.
        /// </summary>
        /// <param name="message">Message to write.</param>
        void WriteInformation(string message);

        /// <summary>
        /// Write an error message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="ex">Exception to include in error message.</param>
        void WriteError(string message, Exception ex);

        /// <summary>
        /// Write an informational message on the verbose level.
        /// </summary>
        /// <param name="message">Message to write</param>
        void WriteVerbose(string message);
    }
}
