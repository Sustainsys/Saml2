using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace Kentor.AuthServices
{
    class AcsCommand : ICommand
    {
        public CommandResult Run(HttpRequestBase request)
        {
            var binding = Saml2Binding.Get(request);

            if (binding != null)
            {
                try
                {
                    binding.Unbind(request);
                }
                catch (Exception ex)
                {
                    if (ex is XmlException || ex is FormatException)
                    {
                        return new CommandResult()
                        {
                            ErrorCode = CommandResultErrorCode.BadFormatSamlResponse,
                            HttpStatusCode = HttpStatusCode.InternalServerError
                        };
                    }
                    throw;
                }
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
