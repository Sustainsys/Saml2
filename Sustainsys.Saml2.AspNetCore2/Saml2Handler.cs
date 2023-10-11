﻿using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Sustainsys.Saml2.WebSso;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using Sustainsys.Saml2.Metadata;

namespace Sustainsys.Saml2.AspNetCore2
{
    /// <summary>
    /// Authentication handler for Saml2
    /// </summary>
    public class Saml2Handler : IAuthenticationRequestHandler, IAuthenticationSignOutHandler
    {
        private readonly IOptionsMonitor<Saml2Options> optionsMonitor;
        private readonly IDataProtectionProvider dataProtectorProvider;

        // Internal to be visible to tests.
        internal Saml2Options options;
        HttpContext context;
        private IDataProtector dataProtector;
        bool emitSameSiteNone;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="optionsMonitor">Options monitor</param>
        /// <param name="dataProtectorProvider">Data Protector Provider</param>
        public Saml2Handler(
            IOptionsMonitor<Saml2Options> optionsMonitor,
            IDataProtectionProvider dataProtectorProvider)
        {
            if (dataProtectorProvider == null)
            {
                throw new ArgumentNullException(nameof(dataProtectorProvider));
            }

            this.optionsMonitor = optionsMonitor;
            this.dataProtectorProvider = dataProtectorProvider;
        }

        /// <InheritDoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "scheme")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "context")]
        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));

            options = optionsMonitor.Get(scheme.Name);

            dataProtector = dataProtectorProvider.CreateProtector(GetType().FullName, options.SPOptions.ModulePath);

            emitSameSiteNone = options.Notifications.EmitSameSiteNone(context.Request.GetUserAgent());

            return Task.CompletedTask;
        }

        /// <InheritDoc />
        [ExcludeFromCodeCoverage]
        public Task<AuthenticateResult> AuthenticateAsync()
        {
            throw new NotImplementedException();
        }

        private string CurrentUri
        {
            get => context.Request.Scheme + "://"
                + context.Request.Host
                + context.Request.PathBase
                + context.Request.Path
                + context.Request.QueryString;
        }

        /// <InheritDoc />
        public async Task ChallengeAsync(AuthenticationProperties properties)
        {
            properties = properties ?? new AuthenticationProperties();

            // Don't serialize the return url twice, move it to our location.
            var redirectUri = properties.RedirectUri ?? CurrentUri;
            properties.RedirectUri = null;

            var requestData = context.ToHttpRequestData(options.CookieManager, null);

            EntityId entityId = null;

            if (properties.Items.TryGetValue("idp", out var entityIdString))
            {
                entityId = new EntityId(entityIdString);
            }

            var result = SignInCommand.Run(
                entityId,
                redirectUri,
                requestData,
                options,
                properties.Items);

            await result.Apply(context, dataProtector, options.CookieManager, null, null, emitSameSiteNone);
        }

        /// <InheritDoc />
        [ExcludeFromCodeCoverage]
        public Task ForbidAsync(AuthenticationProperties properties)
        {
            throw new NotImplementedException();
        }

        /// <InheritDoc />
        public async Task<bool> HandleRequestAsync()
        {
            if (context.Request.Path.StartsWithSegments(options.SPOptions.ModulePath, StringComparison.Ordinal))
            {
                var commandName = context.Request.Path.Value.Substring(
                    options.SPOptions.ModulePath.Length).TrimStart('/');

                var commandResult = CommandFactory.GetCommand(commandName).Run(
                    context.ToHttpRequestData(options.CookieManager, dataProtector.Unprotect), options);

                await commandResult.Apply(
                    context, dataProtector, options.CookieManager, options.SignInScheme, options.SignOutScheme, emitSameSiteNone);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Initiate a federated sign out if supported (Idp supports it and sp has a configured
        /// signing certificate)
        /// </summary>
        /// <param name="properties">Authentication props, containing a return url.</param>
        /// <returns>Task</returns>
        public async Task SignOutAsync(AuthenticationProperties properties)
        {
            var request = context.ToHttpRequestData(options.CookieManager, dataProtector.Unprotect);

            // This is not the right behaviour for Asp.Net Core - we should do nothing if
            // there was not a configured ReturnUrl. But the LogoutCommand is designed
            // to always redirect so this is the best we can do to accept null AuthProps without
            // changing other stuff
            var returnUrl = properties?.RedirectUri ?? (context.Request.PathBase + "/");

            await LogoutCommand.InitiateLogout(
                request,
                new Uri(returnUrl, UriKind.RelativeOrAbsolute),
                options,
                // In the Asp.Net Core2 model, it's the caller's responsibility to terminate the
                // local session on an SP-initiated logout.
                terminateLocalSession: false)
                .Apply(context, dataProtector, options.CookieManager, null, null, emitSameSiteNone);
        }
    }
}
