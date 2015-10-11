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
            EntityId = new EntityId(config.EntityId);
            binding = config.Binding;
            AllowUnsolicitedAuthnResponse = config.AllowUnsolicitedAuthnResponse;
            metadataUrl = config.MetadataUrl;
            LoadMetadata = config.LoadMetadata;
            this.spOptions = spOptions;

            var certificate = config.SigningCertificate.LoadCertificate();

            if (certificate != null)
            {
                signingKey = certificate.PublicKey.Key;
            }

            try
            {
                if (LoadMetadata)
                {
                    DoLoadMetadata();
                }

                Validate();
            }
            catch (WebException)
            {
                // If we had a web exception, the metadata failed. It will 
                // be automatically retried.
            }
        }

        private void Validate()
        {
            if (Binding == 0)
            {
                throw new ConfigurationErrorsException("Missing binding configuration on Idp " + EntityId.Id + ".");
            }

            if (SigningKey == null)
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
        /// <see cref="MetadataUrl"/> that must be done before setting
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

        /// <summary>
        /// The Entity Id of the identity provider.
        /// </summary>
        public EntityId EntityId { get; private set; }

        /// <summary>
        /// Is this idp allowed to send unsolicited responses, i.e. idp initiated sign in?
        /// </summary>
        public bool AllowUnsolicitedAuthnResponse { get; set; }

        private Uri metadataUrl;

        /// <summary>
        /// Location of metadata for the Identity Provider. Automatically enables
        /// <see cref="LoadMetadata"/>
        /// </summary>
        public Uri MetadataUrl
        {
            get
            {
                return metadataUrl ?? new Uri(EntityId.Id);
            }
            set
            {
                metadataUrl = value;
                LoadMetadata = true;
            }
        }

        /// <summary>
        /// Create an authenticate request aimed for this idp.
        /// </summary>
        /// <param name="authServicesUrls">Urls for AuthServices, used to populate fields
        /// in the created AuthnRequest</param>
        /// <returns>AuthnRequest</returns>
        public Saml2AuthenticationRequest CreateAuthenticateRequest(
            AuthServicesUrls authServicesUrls)
        {
            if (authServicesUrls == null)
            {
                throw new ArgumentNullException("authServicesUrls");
            }

            var authnRequest = new Saml2AuthenticationRequest()
            {
                DestinationUrl = SingleSignOnServiceUrl,
                AssertionConsumerServiceUrl = authServicesUrls.AssertionConsumerServiceUrl,
                Issuer = spOptions.EntityId,
                // For now we only support one attribute consuming service.
                AttributeConsumingServiceIndex = spOptions.AttributeConsumingServices.Any() ? 0 : (int?)null
            };

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

        private AsymmetricAlgorithm signingKey;

        /// <summary>
        /// The public key of the idp that is used to verify signatures of responses/assertions.
        /// </summary>
        public AsymmetricAlgorithm SigningKey
        {
            get
            {
                ReloadMetadataIfRequired();
                return signingKey;
            }
            set
            {
                signingKey = value;
            }

        }

        object metadataLoadLock = new object();

        private void DoLoadMetadata()
        {
            lock (metadataLoadLock)
            {
                try
                {
                    var metadata = MetadataLoader.LoadIdp(MetadataUrl);

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
                throw new ArgumentNullException("metadata");
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

            // Prefer an endpoint with a redirect binding, then check for POST which 
            // is the other supported by AuthServices.
            var ssoService = idpDescriptor.SingleSignOnServices
                .FirstOrDefault(s => s.Binding == Saml2Binding.HttpRedirectUri) ??
                idpDescriptor.SingleSignOnServices
                .First(s => s.Binding == Saml2Binding.HttpPostUri);

            binding = Saml2Binding.UriToSaml2BindingType(ssoService.Binding);
            singleSignOnServiceUrl = ssoService.Location;

            var key = idpDescriptor.Keys
                .Where(k => k.Use == KeyType.Unspecified || k.Use == KeyType.Signing)
                .SingleOrDefault();

            if (key != null)
            {
                signingKey = ((AsymmetricSecurityKey)key.KeyInfo.CreateKey())
                    .GetAsymmetricAlgorithm(SignedXml.XmlDsigRSASHA1Url, false);
            }
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
