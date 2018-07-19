using Sustainsys.Saml2.Configuration;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;

namespace Sustainsys.Saml2.WebSso
{
    /// <summary>
    /// Represents a missing command.
    /// Instances of this class are returned by CommandFactory.GetCommand(...)
    /// when the specified command name is not recognised.
    /// </summary>
    public class NotFoundCommand : ICommand
    {
        /// <summary>
        /// Run the command, returning a CommandResult specifying an HTTP 404 Not Found status code.
        /// </summary>
        /// <param name="request">Request data.</param>
        /// <param name="options">Options</param>
        /// <returns>CommandResult</returns>
        public CommandResult Run(HttpRequestData request, IOptions options)
        {
            return new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.NotFound
            };
        }
    }
}
