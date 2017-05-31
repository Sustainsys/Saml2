using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Configuration;
using System.Text.Encodings.Web;

namespace Kentor.AuthServices.AspNetCore
{
    public class KentorAuthServicesMiddleware : AuthenticationMiddleware<KentorAuthServicesOptions>
    {
        public KentorAuthServicesMiddleware(
            RequestDelegate next,
            IOptions<KentorAuthServicesOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            IDataProtectionProvider dataProtectionProvider)
            : base(next, options, loggerFactory, encoder)
        {
            if(dataProtectionProvider == null)
            {
                throw new ArgumentNullException(nameof(dataProtectionProvider));
            }

            if(next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if(loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if(encoder == null)
            {
                throw new ArgumentNullException(nameof(encoder));
            }

            if(options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if(Options.SPOptions == null)
            {
                throw new ConfigurationErrorsException("The options.SPOptions property cannot be null. There is an implementation class Kentor.AuthServices.Configuration.SPOptions that you can instantiate. The EntityId property of that class is mandatory. It must be set to the EntityId used to represent this system.");
            }

            if(Options.SPOptions.EntityId == null)
            {
                throw new ConfigurationErrorsException("The SPOptions.EntityId property cannot be null. It must be set to the EntityId used to represent this system.");
            }

            if(string.IsNullOrEmpty(Options.SignInAsAuthenticationType))
            {
                throw new ConfigurationErrorsException("The SignInAsAuthenticationType property cannot be null.");
            }

            Options.DataProtector = dataProtectionProvider.CreateProtector(typeof(KentorAuthServicesMiddleware).FullName, Options.AuthenticationScheme, "v2"); ;
        }

        protected override AuthenticationHandler<KentorAuthServicesOptions> CreateHandler()
        {
            return new KentorAuthServicesHandler();
        }
    }
}
