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
        /// The name of the Assertion Consumer Service, relative to the base module path.
        /// </summary>
        public const string AcsCommandName = "Acs";

        /// <summary>
        /// Resolve the urls for AuthServices from an http request and options.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="spOptions"></param>
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
        /// <param name="applicationUrl"></param>
        /// <param name="modulePath"></param>
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
            AssertionConsumerServiceUrl = new Uri(applicationUrl, modulePath + "/" + AcsCommandName);
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