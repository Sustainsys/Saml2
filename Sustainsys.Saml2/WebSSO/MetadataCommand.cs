using System;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Metadata.Extensions;

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
            if(options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var urls = new Saml2Urls(request, options);

            var metadata = options.SPOptions.CreateMetadata(urls);
            options.Notifications.MetadataCreated(metadata, urls);

            var result = new CommandResult()
            {
                Content = metadata.ToXmlString(
                    options.SPOptions.SigningServiceCertificate,
                    options.SPOptions.OutboundSigningAlgorithm,
                    options.Notifications.MetadataDocumentCreated),
                ContentType = "application/samlmetadata+xml"
            };

            var fileName = CreateFileName(options.SPOptions.EntityId.Id);

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
