using Kentor.AuthServices.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Kentor.AuthServices
{
    class MetadataCommand : ICommand
    {
        public CommandResult Run(HttpRequestData request, IOptions options)
        {
            return new CommandResult()
            {
                Content = ServiceProvider.Metadata.ToXmlString(
                    KentorAuthServicesSection.Current.MetadataCacheDuration),
                ContentType = "application/samlmetadata+xml"
            };
        }
    }
}
