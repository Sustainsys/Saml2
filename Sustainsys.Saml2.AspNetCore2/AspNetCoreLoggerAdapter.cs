using Sustainsys.Saml2;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.AspNetCore2
{
    /// <summary>
    /// Logger adapter for ASP.NET Core
    /// </summary>
    public class AspNetCoreLoggerAdapter : ILoggerAdapter
    {
        private ILogger logger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="logger">Logger to write to</param>
        public AspNetCoreLoggerAdapter(ILogger logger)
        {
            this.logger = logger;
        }

        /// <InheritDoc />
        public void WriteError(string message, Exception ex)
        {
            logger.LogError(ex, message);
        }

        /// <InheritDoc />
        public void WriteInformation(string message)
        {
            logger.LogInformation(message);
        }

        /// <InheritDoc />
        public void WriteVerbose(string message)
        {
            logger.LogDebug(message);
        }
    }
}
