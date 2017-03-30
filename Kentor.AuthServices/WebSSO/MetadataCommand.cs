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
using Kentor.AuthServices.Metadata;

namespace Kentor.AuthServices.WebSso
{
    class MetadataCommand : ICommand
    {
        public CommandResult Run(HttpRequestData request, IOptions options)
        {
            if(options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var urls = new AuthServicesUrls(request, options);

            var metadata = options.SPOptions.CreateMetadata(urls);
            options.Notifications.MetadataCreated(metadata, urls);

            var result = new CommandResult()
            {
                Content = metadata.ToXmlString(
                    options.SPOptions.SigningServiceCertificate,
                    options.SPOptions.OutboundSigningAlgorithm),
                ContentType = "application/samlmetadata+xml"
            };

            options.Notifications.MetadataCommandResultCreated(result);

            options.SPOptions.Logger.WriteInformation("Created metadata");

            return result;
        }
    }
}
