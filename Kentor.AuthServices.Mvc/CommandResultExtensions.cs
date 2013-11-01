using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Kentor.AuthServices.Mvc
{
    static class CommandResultExtensions
    {
        /// <summary>
        /// Converts a command result to an action result.
        /// </summary>
        /// <param name="commandResult">The source command result.</param>
        /// <returns>Action result</returns>
        /// <remarks>The reason to use a separate comamnd result at all, instead
        /// of simply using ActionResult is that the core library should not
        /// be Mvc dependant.</remarks>
        public static ActionResult ToActionResult(this CommandResult commandResult)
        {
            switch (commandResult.HttpStatusCode)
            {
                case HttpStatusCode.SeeOther:
                    return new RedirectResult(commandResult.Location.ToString());
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
