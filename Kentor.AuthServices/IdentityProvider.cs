using System.Collections.Generic;
using Kentor.AuthServices.Configuration;
using System;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using Kentor.AuthServices.Internal;
using Kentor.AuthServices.Metadata;
using Kentor.AuthServices.Saml2P;
using Kentor.AuthServices.WebSso;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Concurrent;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Represents a known identity provider that this service provider can communicate with.
    /// </summary>
    public class IdentityProvider
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="entityId">Entity id of the identityprovider.</param>
        /// <param name="spOptions">Service provider options to use when 
        /// creating AuthnRequests for this Idp.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "sp")]
        public IdentityProvider(EntityId entityId, ISPOptions spOptions)
        {
            EntityId = entityId;
            this.spOptions = spOptions;
        }

        readonly ISPOptions spOptions;

        internal IdentityProvider(IdentityProviderElement config, ISPOptions spOptions)
        {
            singleSignOnServiceUrl = config.DestinationUrl;
            SingleLogoutServiceUrl = config.LogoutUrl;
            EntityId = new EntityId(config.EntityId);
            binding = config.Binding;
            AllowUnsolicitedAuthnResponse = config.AllowUnsolicitedAuthnResponse;
            metadataLocation = string.IsNullOrEmpty(config.MetadataLocation) 
                ?  null : config.MetadataLocation;
            WantAuthnRequestsSigned = config.WantAuthnRequestsSigned;

            var certificate = config.SigningCertificate.LoadCertificate();
            if (certificate != null)
            {
                signingKeys.AddConfiguredKey(
                    new X509RawDataKeyIdentifierClause(certificate));
            }

            foreach (var ars in config.ArtifactResolutionServices)
            {
                ArtifactResolutionServiceUrls[ars.Index] = ars.Location;
            }

            // If configured to load metadata, this will immediately do the load.
            LoadMetadata = config.LoadMetadata;
            this.spOptions = spOptions;

            // Validate if values are only from config. If metadata is loaded, validation
            // is done on metadata load.
            if (!LoadMetadata)
            {
                Validate();
            }
        }

        private void Validate()
        {
            if (Binding == 0)
            {
                throw new ConfigurationErrorsException("Missing binding configuration on Idp " + EntityId.Id + ".");
            }

            if (!SigningKeys.Any())
            {
                throw new ConfigurationErrorsException("Missing signing certificate configuration on Idp " + EntityId.Id + ".");
            }

            if (SingleSignOnServiceUrl == null)
            {
                throw new ConfigurationErrorsException("Missing assertion consumer service url configuration on Idp " + EntityId.Id + ".");
            }
        }

        private bool loadMetadata;

        /// <summary>
        /// Should this idp load metadata? If you intend to set the
        /// <see cref="MetadataLocation"/> that must be done before setting
        /// LoadMetadata to true.</summary>
        public bool LoadMetadata
        {
            get
            {
                return loadMetadata;
            }
            set
            {
                loadMetadata = value;
                if (loadMetadata)
                {
                    try
                    {
                        DoLoadMetadata();
                        Validate();
                    }
                    catch (WebException)
                    {
                        // Ignore if metadata load failed, an automatic
                        // retry has been scheduled.
                    }
                }
            }
        }

        private Saml2BindingType binding;

        /// <summary>
        /// The binding used when sending AuthnRequests to the identity provider.
        /// </summary>
        public Saml2BindingType Binding
        {
            get
            {
                ReloadMetadataIfRequired();
                return binding;
            }
            set
            {
                binding = value;
            }
        }

        private Uri singleSignOnServiceUrl;

        /// <summary>
        /// The Url of the single sign on service. This is where the browser is redirected or
        /// where the post data is sent to when sending an AuthnRequest to the idp.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "SignOn")]
        public Uri SingleSignOnServiceUrl
        {
            get
            {
                ReloadMetadataIfRequired();
                return singleSignOnServiceUrl;
            }
            set
            {
                singleSignOnServiceUrl = value;
            }
        }

        private IDictionary<int, Uri> artifactResolutionServiceUrls
            = new ConcurrentDictionary<int, Uri>();

        /// <summary>
        /// Artifact resolution endpoints on the idp.
        /// </summary>
        public IDictionary<int, Uri> ArtifactResolutionServiceUrls
        {
            get
            {
                ReloadMetadataIfRequired();
                return artifactResolutionServiceUrls;
            }
        }

        /// <summary>
        /// The Url of the single sign out service. This is where the browser
        /// is redirected or where the post data is sent to when sending a
        /// LogoutRequest to the idp.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout")]
        public Uri SingleLogoutServiceUrl { get; set; }
        
        /// <summary>
        /// The Entity Id of the identity provider.
        /// </summary>
        public EntityId EntityId { get; private set; }

        /// <summary>
        /// Is this idp allowed to send unsolicited responses, i.e. idp initiated sign in?
        /// </summary>
        public bool AllowUnsolicitedAuthnResponse { get; set; }

        private string metadataLocation;

        /// <summary>
        /// Location of metadata for the Identity Provider. Automatically enables
        /// <see cref="LoadMetadata"/>. The location can be a URL, an absolute
        /// path to a local file or an app relative  path 
        /// (e.g. ~/App_Data/IdpMetadata.xml). By default the entity id is
        /// interpreted as the metadata location (which is a convention).
        /// </summary>
        public string  MetadataLocation
        {
            get
            {
                return metadataLocation ?? EntityId.Id;
            }
            set
            {
                metadataLocation = value;
                LoadMetadata = true;
            }
        }

        /// <summary>
        /// Create an authenticate request aimed for this idp.
        /// </summary>
        /// <param name="returnUrl">The return url where the browser should be sent after
        /// successful authentication.</param>
        /// <param name="authServicesUrls">Urls for AuthServices, used to populate fields
        /// in the created AuthnRequest</param>
        /// <returns>AuthnRequest</returns>
        public Saml2AuthenticationRequest CreateAuthenticateRequest(
            Uri returnUrl,
            AuthServicesUrls authServicesUrls)
        {
            return CreateAuthenticateRequest(returnUrl, authServicesUrls, null);
        }

        /// <summary>
        /// Create an authenticate request aimed for this idp.
        /// </summary>
        /// <param name="returnUrl">The return url where the browser should be sent after
        /// successful authentication.</param>
        /// <param name="authServicesUrls">Urls for AuthServices, used to populate fields
        /// in the created AuthnRequest</param>
        /// <param name="relayData">Aux data that should be preserved across the authentication</param>
        /// <returns>AuthnRequest</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ServiceCertificates")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "AuthenticateRequests")]
        public Saml2AuthenticationRequest CreateAuthenticateRequest(
            Uri returnUrl,
            AuthServicesUrls authServicesUrls,
            object relayData)
        {
            if (authServicesUrls == null)
            {
                throw new ArgumentNullException(nameof(authServicesUrls));
            }

            var authnRequest = new Saml2AuthenticationRequest()
            {
                DestinationUrl = SingleSignOnServiceUrl,
                AssertionConsumerServiceUrl = authServicesUrls.AssertionConsumerServiceUrl,
                Issuer = spOptions.EntityId,
                // For now we only support one attribute consuming service.
                AttributeConsumingServiceIndex = spOptions.AttributeConsumingServices.Any() ? 0 : (int?)null,
                NameIdPolicy = spOptions.NameIdPolicy,
                RequestedAuthnContext = spOptions.RequestedAuthnContext
            };

            if(spOptions.AuthenticateRequestSigningBehavior == SigningBehavior.Always
                || (spOptions.AuthenticateRequestSigningBehavior == SigningBehavior.IfIdpWantAuthnRequestsSigned
                && WantAuthnRequestsSigned))
            {
                if(spOptions.SigningServiceCertificate == null)
                {
                    throw new ConfigurationErrorsException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Idp \"{0}\" is configured for signed AuthenticateRequests, but ServiceCertificates configuration contains no certificate with usage \"Signing\" or \"Both\".",
                            EntityId.Id));
                }

                authnRequest.SigningCertificate = spOptions.SigningServiceCertificate;
            }

            var requestState = new StoredRequestState(EntityId, returnUrl, authnRequest.Id, relayData);

            PendingAuthnRequests.Add(authnRequest.RelayState, requestState);

            return authnRequest;
        }

        /// <summary>
        /// Bind a Saml2AuthenticateRequest using the active binding of the idp,
        /// producing a CommandResult with the result of the binding.
        /// </summary>
        /// <param name="request">The AuthnRequest to bind.</param>
        /// <returns>CommandResult with the bound request.</returns>
        public CommandResult Bind(ISaml2Message request)
        {
            return Saml2Binding.Get(Binding).Bind(request);
        }

        private ConfiguredAndLoadedSigningKeysCollection signingKeys = 
            new ConfiguredAndLoadedSigningKeysCollection();

        /// <summary>
        /// The public key of the idp that is used to verify signatures of responses/assertions.
        /// </summary>
        public ConfiguredAndLoadedSigningKeysCollection SigningKeys
        {
            get
            {
                ReloadMetadataIfRequired();
                return signingKeys;
            }
        }

        object metadataLoadLock = new object();

        private void DoLoadMetadata()
        {
            lock (metadataLoadLock)
            {
                try
                {
                    var metadata = MetadataLoader.LoadIdp(MetadataLocation);

                    ReadMetadata(metadata);
                }
                catch (WebException)
                {
                    MetadataValidUntil = DateTime.MinValue;
                    throw;
                }
            }
        }

        /// <summary>
        /// Reads the supplied metadata and sets all properties of the 
        /// IdentityProvider based on the metadata.
        /// </summary>
        /// <param name="metadata">Metadata to read.</param>
        public void ReadMetadata(ExtendedEntityDescriptor metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            lock (metadataLoadLock)
            {
                if (metadata.EntityId.Id != EntityId.Id)
                {
                    var msg = string.Format(CultureInfo.InvariantCulture,
                        "Unexpected entity id \"{0}\" found when loading metadata for \"{1}\".",
                        metadata.EntityId.Id, EntityId.Id);
                    throw new ConfigurationErrorsException(msg);
                }

                ReadMetadataIdpDescriptor(metadata);

                MetadataValidUntil = metadata.CalculateMetadataValidUntil();
            }
        }

        private void ReadMetadataIdpDescriptor(ExtendedEntityDescriptor metadata)
        {
            var idpDescriptor = metadata.RoleDescriptors
                .OfType<IdentityProviderSingleSignOnDescriptor>().Single();

            WantAuthnRequestsSigned = idpDescriptor.WantAuthenticationRequestsSigned;

            // Prefer an endpoint with a redirect binding, then check for POST which 
            // is the other supported by AuthServices.
            var ssoService = idpDescriptor.SingleSignOnServices
                .FirstOrDefault(s => s.Binding == Saml2Binding.HttpRedirectUri) ??
                idpDescriptor.SingleSignOnServices
                .FirstOrDefault(s => s.Binding == Saml2Binding.HttpPostUri);

            if (ssoService != null)
            {
                binding = Saml2Binding.UriToSaml2BindingType(ssoService.Binding);
                singleSignOnServiceUrl = ssoService.Location;
            }

            foreach(var ars in idpDescriptor.ArtifactResolutionServices)
            {
                artifactResolutionServiceUrls[ars.Value.Index] = ars.Value.Location;
            }

            foreach (var ars in artifactResolutionServiceUrls.Keys
                .Where(k => !idpDescriptor.ArtifactResolutionServices.Keys.Contains(k)))
            {
                artifactResolutionServiceUrls.Remove(ars);
            }

            var keys = idpDescriptor.Keys.Where(k => k.Use == KeyType.Unspecified || k.Use == KeyType.Signing);

            signingKeys.SetLoadedItems(keys.Select(k => k.KeyInfo.First(c => c.CanCreateKey)).ToList());
        }

        private DateTime? metadataValidUntil;

        /// <summary>
        /// Validity time of the metadata this idp was configured from. Null if
        /// idp was not configured from metadata.
        /// </summary>
        public DateTime? MetadataValidUntil
        {
            get
            {
                return metadataValidUntil;
            }
            private set
            {
                metadataValidUntil = value;

                if (LoadMetadata)
                {
                    Task.Delay(MetadataRefreshScheduler.GetDelay(value.Value))
                        .ContinueWith((_) => DoLoadMetadata());
                }
            }
        }

        /// <summary>
        /// Does this Idp want the AuthnRequests signed?
        /// </summary>
        public bool WantAuthnRequestsSigned { get; set; }

        private void ReloadMetadataIfRequired()
        {
            if (LoadMetadata && MetadataValidUntil.Value < DateTime.UtcNow)
            {
                lock (metadataLoadLock)
                {
                    DoLoadMetadata();
                }
            }
        }
    }
}
