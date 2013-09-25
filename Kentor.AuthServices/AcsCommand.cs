using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Kentor.AuthServices
{
    class AcsCommand : ICommand
    {
        public CommandResult Run(HttpRequestBase request)
        {
            var binding = Saml2Binding.Get(request);

            if(binding != null)
            {
                return new CommandResult();
            }

            return noSamlResponseFoundResult;
        }

        static readonly CommandResult noSamlResponseFoundResult = new CommandResult()
        {
            HttpStatusCode = HttpStatusCode.InternalServerError,
            ErrorCode = CommandResultErrorCode.NoSamlResponseFound
        };
    }
}
