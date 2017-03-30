using Kentor.AuthServices.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.WebSso
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
        /// <param name="options">Options to get module path and (optional) notification hooks from.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SignInUrl")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "LogoutUrl")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "AuthServicesUrl")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "AssertionConsumerServiceUrl")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ApplicationUrl")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "sp")]
        public AuthServicesUrls(HttpRequestData request, IOptions options)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var publicOrigin = options.Notifications.GetPublicOrigin(request) ?? options.SPOptions.PublicOrigin ?? request.ApplicationUrl;
            Init(publicOrigin, options.SPOptions.ModulePath);

            options.SPOptions.Logger.WriteVerbose("Expanded AuthServicesUrl"
                + "\n  AssertionConsumerServiceUrl: " + AssertionConsumerServiceUrl
                + "\n  SignInUrl: " + SignInUrl
                + "\n  LogoutUrl: " + LogoutUrl
                + "\n  ApplicationUrl: " + ApplicationUrl);
        }

        /// <summary>
        /// Creates the urls for AuthServices based on the complete base Url
        /// the application and the AuthServices base module path.
        /// </summary>
        /// <param name="applicationUrl">The full Url to the root of the application.</param>
        /// <param name="modulePath">Path of module, starting with / and ending without.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads"
            , Justification = "Incorrect warning. modulePath isn't a string representation of a Uri" )]
        public AuthServicesUrls(Uri applicationUrl, string modulePath)
        {
            if (applicationUrl == null)
            {
                throw new ArgumentNullException(nameof(applicationUrl));
            }

            if (modulePath == null)
            {
                throw new ArgumentNullException(nameof(modulePath));
            }

            Init(applicationUrl, modulePath);
        }

        /// <summary>
        /// Creates the urls for AuthServices based on the given full urls
        /// for assertion consumer service and sign-in
        /// </summary>
        /// <param name="assertionConsumerServiceUrl">The full Url for the Assertion Consumer Service.</param>
        /// <param name="signInUrl">The full Url for sign-in.</param>
        /// <param name="applicationUrl">The full Url for the application root.</param>
        public AuthServicesUrls(Uri assertionConsumerServiceUrl, Uri signInUrl, Uri applicationUrl)
        {
            if (signInUrl == null)
            {
                throw new ArgumentNullException(nameof(signInUrl));
            }

            AssertionConsumerServiceUrl = assertionConsumerServiceUrl;
            SignInUrl = signInUrl;
            ApplicationUrl = applicationUrl;
        }

        void Init(Uri publicOrigin, string modulePath)
        {
            if (!modulePath.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("modulePath should start with /.");
            }

            if(!publicOrigin.AbsoluteUri.EndsWith("/", StringComparison.Ordinal))
            {
                publicOrigin = new Uri(publicOrigin.AbsoluteUri + "/");
            }

            var authServicesRoot = publicOrigin.AbsoluteUri.TrimEnd('/') + modulePath + "/";

            AssertionConsumerServiceUrl = new Uri(authServicesRoot + CommandFactory.AcsCommandName);
            SignInUrl = new Uri(authServicesRoot + CommandFactory.SignInCommandName);
            ApplicationUrl = publicOrigin;
            LogoutUrl = new Uri(authServicesRoot + CommandFactory.LogoutCommandName);
        }

        /// <summary>
        /// The full url of the assertion consumer service.
        /// </summary>
        public Uri AssertionConsumerServiceUrl { get; private set; }

        /// <summary>
        /// The full url of the signin command, which is also the response 
        /// location for idp discovery.
        /// </summary>
        public Uri SignInUrl { get; private set; }
        
        /// <summary>
        /// The full url of the application root. Used as default redirect
        /// location after logout.
        /// </summary>
        public Uri ApplicationUrl { get; internal set; }

        /// <summary>
        /// The full url of the logout command.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout")]
        public Uri LogoutUrl { get; internal set; }
    }
}