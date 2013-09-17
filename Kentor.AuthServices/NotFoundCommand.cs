using System;
using System.Linq;
using System.Net;

namespace Kentor.AuthServices
{
    class NotFoundCommand : Command
    {
        public override CommandResult Run()
        {
            return new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.NotFound
            };
        }
    }
}
