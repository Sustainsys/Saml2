using Kentor.AuthServices.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Kentor.AuthServices
{
    class 
        MetadataCommand : ICommand
    {
        public CommandResult Run(HttpRequestData request)
        {
            var xmlData = ServiceProvider.Metadata.ToXElement();

            // Add the cacheduration of the metadata before outputting. This is done here
            // as it is not actually part of the metadata, but rather part of how it is presented.
            xmlData.Add(new XAttribute("cacheDuration",
                KentorAuthServicesSection.Current.MetadataCacheDuration));

            return new CommandResult()
            {
                Content = xmlData.ToString(),
                ContentType = "application/samlmetadata+xml"
            };
        }
    }
}
