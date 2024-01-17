using Sustainsys.Saml2.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Sustainsys.Saml2.Metadata;

namespace Sustainsys.Saml2.WebSso
{
    /// <summary>
    /// Represents the service provider metadata command behaviour.
    /// Instances of this class can be created directly or by using the factory method
    /// CommandFactory.GetCommand(CommandFactory.MetadataCommandName).
    /// </summary>
    public class MetadataCommand : ICommand
    {
        /// <summary>
        /// Run the command, creating and returning the service provider metadata.
        /// </summary>
        /// <param name="request">Request data.</param>
        /// <param name="options">Options</param>
        /// <returns>CommandResult</returns>
        public CommandResult Run(HttpRequestData request, IOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var urls = new Saml2Urls(request, options);

            /* Dynamically fetch the identity provider for the entityid.
             * This will ensure any settings controlled with a feature flag or dynamically refreshed values will be updated after Identity Server startup.
             * We use these identity provider specific settings instead of the startup settings for the rest of the flow.
             */
            IdentityProvider identityProvider = options.Notifications.GetIdentityProvider(options.SPOptions.EntityId, null, options);

			var metadata = identityProvider.spOptions.CreateMetadata(urls);
            options.Notifications.MetadataCreated(metadata, urls);

            var result = new CommandResult()
            {
                Content = metadata.ToXmlString(
					identityProvider.spOptions.SigningServiceCertificate,
                    identityProvider.spOptions.OutboundSigningAlgorithm),
                ContentType = "application/samlmetadata+xml"
            };

            var fileName = CreateFileName(identityProvider.spOptions.EntityId.Id);

            result.Headers.Add("Content-Disposition", "attachment; filename=\"" + fileName + "\"");

            options.Notifications.MetadataCommandResultCreated(result);

            options.SPOptions.Logger.WriteInformation("Created metadata");

            return result;
        }

        private object CreateFileName(string id)
        {
            return id
                .Replace("http://", "")
                .Replace("https://", "")
                .Replace(':', '.')
                .Replace('/', '_')
                + ".xml";
        }
    }
}
