using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kentor.AuthServices.Configuration;

namespace Kentor.AuthServices.WebSso
{
    class SignOutCommand : ICommand
    {
        public CommandResult Run(HttpRequestData request, IOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
