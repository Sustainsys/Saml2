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
        // Ctor used for testing.
        internal IdentityProvider(Uri destinationUrl, ISPOptions spOptions)
        {
            singleSignOnServiceUrl = destinationUrl;
            this.spOptions = spOptions;
        }

        readonly ISPOptions spOptions;

        internal IdentityProvider(IdentityProviderElement config, ISPOptions spOptions)
        {
            singleSignOnServiceUrl = config.DestinationUrl;
            EntityId = new EntityId(config.EntityId);
            binding = config.Binding;
            AllowUnsolicitedAuthnResponse = config.AllowUnsolicitedAuthnResponse;
            metadataLocation = config.MetadataUrl;
            this.spOptions = spOptions;

            var certificate = config.SigningCertificate.LoadCertificate();

            if (certificate != null)
            {
                signingKey = certificate.PublicKey.Key;
            }

            try
            {
                if (config.LoadMetadata)
                {
                    LoadMetadata();
                }

                Validate();
            }
            catch (WebException)
            {
                // If we had a web exception, the metadata failed. It will 
                // be automatically retried.
            }
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="metadata">Metadata to use to configure the identity provider.</param>
        /// <param name="allowUnsolicitedAuthnResponse">Are unsolicited responses allowed from this idp?</param>
        /// <param name="spOptions">Service Provider option to use when creating AuthnRequests.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "sp")]
        public IdentityProvider(ExtendedEntityDescriptor metadata, bool allowUnsolicitedAuthnResponse, ISPOptions spOptions)
        {
            AllowUnsolicitedAuthnResponse = allowUnsolicitedAuthnResponse;
            this.spOptions = spOptions;
            LoadMetadata(metadata);
            Validate();
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

        private Saml2BindingType binding;

        /// <summary>
        /// The binding used when sending AuthnRequests to the identity provider.
        /// </summary>
        public Saml2BindingType Binding
        {
            get
            {
                ReloadMetadataIfExpired();
                return binding;
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
                ReloadMetadataIfExpired();
                return singleSignOnServiceUrl;
            }
        }

        /// <summary>
        /// The Entity Id of the identity provider.
        /// </summary>
        public EntityId EntityId { get; private set; }

        /// <summary>
        /// Is this idp allowed to send unsolicited responses, i.e. idp initiated sign in?
        /// </summary>
        public bool AllowUnsolicitedAuthnResponse { get; private set; }

        private Uri metadataLocation;

        /// <summary>
        /// Location of metadata for the Identity Provider.
        /// </summary>
        public Uri MetadataLocation
        {
            get
            {
                return metadataLocation ?? new Uri(EntityId.Id);
            }
        }

        /// <summary>
        /// Create an authenticate request aimed for this idp.
        /// </summary>
        /// <param name="returnUrl">The return url where the browser should be sent after
        /// successful authentication.</param>
        /// <param name="authServicesUrls">Urls for AuthServices, used to populate fields
        /// in the created AuthnRequest</param>
        /// <returns></returns>
        public Saml2AuthenticationRequest CreateAuthenticateRequest(
            Uri returnUrl,
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

            var responseData = new StoredRequestState(EntityId, returnUrl);

            PendingAuthnRequests.Add(new Saml2Id(authnRequest.Id), responseData);

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
                ReloadMetadataIfExpired();
                return signingKey;
            }
        }

        object metadataLoadLock = new object();

        private void LoadMetadata()
        {
            lock (metadataLoadLock)
            {
                try
                {
                    var metadata = MetadataLoader.LoadIdp(MetadataLocation);

                    LoadMetadata(metadata);
                }
                catch (WebException)
                {
                    MetadataValidUntil = DateTime.MinValue;
                    throw;
                }
            }
        }

        private void LoadMetadata(ExtendedEntityDescriptor metadata)
        {
            lock (metadataLoadLock)
            {
                if (EntityId != null)
                {
                    if (metadata.EntityId.Id != EntityId.Id)
                    {
                        var msg = string.Format(CultureInfo.InvariantCulture,
                            "Unexpected entity id \"{0}\" found when loading metadata for \"{1}\".",
                            metadata.EntityId.Id, EntityId.Id);
                        throw new ConfigurationErrorsException(msg);
                    }
                }
                else
                {
                    EntityId = metadata.EntityId;
                }

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

                MetadataValidUntil = metadata.CalculateMetadataValidUntil();
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

                if (value.HasValue)
                {
                    Task.Delay(MetadataRefreshScheduler.GetDelay(value.Value))
                        .ContinueWith((_) => LoadMetadata());
                }
            }
        }

        private void ReloadMetadataIfExpired()
        {
            if (MetadataValidUntil < DateTime.UtcNow)
            {
                lock (metadataLoadLock)
                {
                    LoadMetadata();
                }
            }
        }
    }
}
