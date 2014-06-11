using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kentor.AuthServices
{
    class 
        MetadataCommand : ICommand
    {
        public CommandResult Run(HttpRequestBase request)
        {
            return new CommandResult()
            {
                Content = ServiceProvider.Metadata.ToXElement().ToString(),
                ContentType = "application/samlmetadata+xml"
            };
        }
    }
}
