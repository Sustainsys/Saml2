using System.Collections.Generic;
using Kentor.AuthServices.Saml2P;
using System;
using Kentor.AuthServices.WebSso;
using System.IdentityModel.Metadata;
using Kentor.AuthServices.Metadata;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Set of callbacks that can be used as extension points for various
    /// events.
    /// </summary>
    public class KentorAuthServicesNotifications
    {
        /// <summary>
        /// Ctor, setting all callbacks to do-nothing versions.
        /// </summary>
        public KentorAuthServicesNotifications()
        {
            AuthenticationRequestCreated = (request, provider, dictionary) => { };
            SignInCommandResultCreated = (cr, r) => { };
            SelectIdentityProvider = (ei, r) => null;
            GetLogoutResponseState = ( httpRequestData ) => httpRequestData.StoredRequestState;
            GetPublicOrigin = ( httpRequestData ) => null;
            ProcessSingleLogoutResponseStatus = ( response, state ) => false;
            GetBinding = Saml2Binding.Get;
            MessageUnbound = ur => { };
            AcsCommandResultCreated = (cr, r) => { };
            LogoutCommandResultCreated = cr => { };
            MetadataCreated = (md, urls) => { };
            MetadataCommandResultCreated = cr => { };
            ValidateAbsoluteReturnUrl = url => false;
        }

        /// <summary>
        /// Notification called when a <see cref="Saml2AuthenticationRequest"/>
        /// has been created. The authenticationrequest can be amended and
        /// modified.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public Action<Saml2AuthenticationRequest, IdentityProvider, IDictionary<string, string>>
            AuthenticationRequestCreated { get; set; }

        /// <summary>
        /// Notification called when the SignIn command has produced a
        /// <see cref="CommandResult"/>, but before anything has been applied
        /// to the outgoing response. Set the <see cref="CommandResult.HandledResult"/>
        /// flag to suppress the library's built in apply functionality to the
        /// outgoing response.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public Action<CommandResult, IDictionary<string, string>>
            SignInCommandResultCreated { get; set; }

        /// <summary>
        /// Notification called when the SignIn command is about to select
        /// what Idp to use for the request. The EntityId is the one supplied
        /// (e.g. through query string). To select a specicic IdentityProvider
        /// simply return it. Return <code>null</code> to fall back to built
        /// in selection.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public Func<EntityId, IDictionary<string, string>, IdentityProvider>
            SelectIdentityProvider { get; set; }

        /// <summary>
        /// Notification called when the logout command is about to use the 
        /// <code>StoredRequestState</code> derived from the request's RelayState data.
        /// Return a different StoredRequestState if you would like to customize the 
        /// RelayState lookup. 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout")]
        public Func<HttpRequestData, StoredRequestState>
            GetLogoutResponseState { get; set; }

        /// <summary>
        /// Notification called when a command is about to construct a fully-qualified url
        /// Return a non-null Uri if you need to override this per request. Otherwise
        /// it will fall back to the normal logic that checks the request Uri 
        /// and the SPOptions.PublicOrigin setting
        /// </summary>
        public Func<HttpRequestData, Uri>
            GetPublicOrigin { get; set; }

        /// <summary>
        /// Notification called when single logout status is returned from IDP.
        /// Return true to indicate that your notification has handled this status. Otherwise
        /// it will fall back to the normal status processing logic.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout" )]
        public Func<Saml2LogoutResponse, StoredRequestState, bool>
            ProcessSingleLogoutResponseStatus { get; set; }

        /// <summary>
        /// Get a binding that can unbind data from the supplied request. The
        /// default is to use <see cref="Saml2Binding.Get(HttpRequestData)"/>
        /// </summary>
        public Func<HttpRequestData, Saml2Binding> GetBinding { get; set; }

        /// <summary>
        /// Notification called when the command has extracted data from
        /// request (by using <see cref="Saml2Binding.Unbind(HttpRequestData, IOptions)"/>)
        /// </summary>
        public Action<UnbindResult> MessageUnbound { get; set; }

        /// <summary>
        /// Notification called when the ACS command has produced a
        /// <see cref="CommandResult"/>, but before anything has been applied
        /// to the outgoing response. Set the <see cref="CommandResult.HandledResult"/>
        /// flag to suppress the library's built in apply functionality to the
        /// outgoing response.
        /// </summary>
        public Action<CommandResult, Saml2Response> AcsCommandResultCreated { get; set; }

        /// <summary>
        /// Notification called when the Logout command has produced a
        /// <see cref="CommandResult"/>, but before anything has been applied
        /// to the outgoing response. Set the <see cref="CommandResult.HandledResult"/>
        /// flag to suppress the library's built in apply functionality to the
        /// outgoing response.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout")]
        public Action<CommandResult> LogoutCommandResultCreated { get; set; }

        /// <summary>
        /// Notification called when metadata has been created, but before
        /// signing. At this point the contents of the metadata can be
        /// altered before presented.
        /// </summary>
        public Action<ExtendedEntityDescriptor, AuthServicesUrls>
            MetadataCreated { get; set; }

        /// <summary>
        /// Notification called when the Metadata command has produced a
        /// <see cref="CommandResult"/>, but before anything has been applied
        /// to the outgoing response. Set the <see cref="CommandResult.HandledResult"/>
        /// flag to suppress the library's built in apply functionality to the
        /// outgoing response.
        /// </summary>
        public Action<CommandResult> MetadataCommandResultCreated {get;set;}

        /// <summary>
        /// Notification called by the SignIn and Logout commands to validate a ReturnUrl that is not relative.
        /// Return true to indicate that you accept the ReturnUrl, false otherwise.
        /// Default validation do not accept any absolute URL.
        /// When false is returned, the SignIn and Logout commands will throw an <see cref="InvalidOperationException"/>.
        /// </summary>
        public Func<string, bool> ValidateAbsoluteReturnUrl { get; set; }
    }
}
