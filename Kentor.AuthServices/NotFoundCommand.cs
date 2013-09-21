using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;

namespace Kentor.AuthServices
{
    class NotFoundCommand : ICommand
    {
        public CommandResult Run(NameValueCollection formData = null)
        {
            return new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.NotFound
            };
        }
    }
}
