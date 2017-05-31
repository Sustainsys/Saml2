using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;

namespace Kentor.AuthServices.AspNetCore
{
    public static class KentorAuthServicesAppBuilderExtensions
    {
        public static IApplicationBuilder UseKentorAuthServices(this IApplicationBuilder app)
        {
            if(app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<KentorAuthServicesMiddleware>();
        }

        public static IApplicationBuilder UseKentorAuthServices(this IApplicationBuilder app, KentorAuthServicesOptions options)
        {
            if(app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if(options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return app.UseMiddleware<KentorAuthServicesMiddleware>(Options.Create(options));
        }
    }
}
