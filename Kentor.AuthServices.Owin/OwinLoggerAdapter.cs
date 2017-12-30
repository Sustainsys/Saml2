using Microsoft.Owin.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Owin
{
    /// <summary>
    /// Adapter for Saml2 logging around owin logging system.
    /// </summary>
    public class OwinLoggerAdapter : ILoggerAdapter
    {
        ILogger logger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="logger">Owin logger to wrap</param>
        public OwinLoggerAdapter(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Write an error message and include an exception
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="ex">Exception with details</param>
        public void WriteError(string message, Exception ex)
        {
            logger.WriteError(message, ex);
        }

        /// <summary>
        /// Write an informational message.
        /// </summary>
        /// <param name="message">Message to write.</param>
        public void WriteInformation(string message)
        {
            logger.WriteInformation(message);
        }

        /// <summary>
        /// Write a verbose informational message.
        /// </summary>
        /// <param name="message">Message to write</param>
        public void WriteVerbose(string message)
        {
            logger.WriteVerbose(message);
        }
    }
}
