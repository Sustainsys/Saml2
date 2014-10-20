using Kentor.AuthServices.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    /// <summary>
    /// The urls of AuthServices that are used in various messages.
    /// </summary>
    public class AuthServicesUrls
    {
        /// <summary>
        /// Resolve the urls for AuthServices from an http request and options.
        /// </summary>
        /// <param name="request">Request to get application root url from.</param>
        /// <param name="spOptions">SP Options to get module path from.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "sp")]
        public AuthServicesUrls(HttpRequestData request, ISPOptions spOptions)
        {
            if(request == null)
            {
                throw new ArgumentNullException("request");
            }

            if(spOptions == null)
            {
                throw new ArgumentNullException("spOptions");
            }

            Init(request.ApplicationUrl, spOptions.ModulePath);
        }

        /// <summary>
        /// Creates the urls for AuthServices based on the complete base Url
        /// the application and the AuthServices base module path.
        /// </summary>
        /// <param name="applicationUrl">The full Url to the root of the application.</param>
        /// <param name="modulePath">Path of module, starting with / and ending without.</param>
        public AuthServicesUrls(Uri applicationUrl, string modulePath)
        {
            if(applicationUrl == null)
            {
                throw new ArgumentNullException("applicationUrl");
            }

            if(modulePath == null)
            {
                throw new ArgumentNullException("modulePath");
            }

            Init(applicationUrl, modulePath);
        }

        void Init(Uri applicationUrl, string modulePath)
        {
            if(!modulePath.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("modulePath should start with /.");
            }

            var authServicesRoot = applicationUrl.AbsoluteUri.TrimEnd('/') + modulePath + "/";

            AssertionConsumerServiceUrl = new Uri(authServicesRoot + CommandFactory.AcsCommandName);
        }

        /// <summary>
        /// The full uri of the assertion consumer service.
        /// </summary>
        public Uri AssertionConsumerServiceUrl
        {
            get; private set;
        }
    }
}