using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Kentor.AuthServices.Mvc
{
    /// <summary>
    /// Extension methods for CommandResult for integrating CommandResults in
    /// the MVC architecture.
    /// </summary>
    public static class CommandResultExtensions
    {
        /// <summary>
        /// Converts a command result to an action result.
        /// </summary>
        /// <param name="commandResult">The source command result.</param>
        /// <returns>Action result</returns>
        /// <remarks>The reason to use a separate command result at all, instead
        /// of simply using ActionResult is that the core library should not
        /// be Mvc dependant.</remarks>
        public static ActionResult ToActionResult(this CommandResult commandResult)
        {
            if (commandResult == null)
            {
                throw new ArgumentNullException("commandResult");
            }

            switch (commandResult.HttpStatusCode)
            {
                case HttpStatusCode.SeeOther:
                    return new RedirectResult(commandResult.Location.ToString());
                case HttpStatusCode.OK:
                    return new ContentResult()
                    {
                        Content = commandResult.Content
                    };
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
