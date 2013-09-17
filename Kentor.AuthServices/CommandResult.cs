using System;
using System.Linq;
using System.Net;
using System.Web;

namespace Kentor.AuthServices
{
    class CommandResult
    {
        public HttpStatusCode HttpStatusCode { get; set; }

        public CommandResult()
        {
            HttpStatusCode = HttpStatusCode.OK;
        }

        public void Apply(HttpResponse response)
        {
            response.StatusCode = (int)HttpStatusCode;      
        }
    }
}
