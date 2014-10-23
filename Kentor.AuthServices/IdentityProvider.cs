using Kentor.AuthServices.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Tokens;
using System.Net;
using System.IdentityModel.Metadata;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Configuration;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Represents a known identity provider that this service provider can communicate with.
    /// </summary>
    public class IdentityProvider
    {
        // Ctor used for testing.
        internal IdentityProvider(Uri destinationUri, ISPOptions spOptions)
        {
            SingleSignOnServiceUrl = destinationUri;
            this.spOptions = spOptions;
        }

        readonly ISPOptions spOptions;

        internal IdentityProvider(IdentityProviderElement config, ISPOptions spOptions)
        {
            SingleSignOnServiceUrl = config.DestinationUri;
            EntityId = new EntityId(config.EntityId);
            Binding = config.Binding;
            AllowUnsolicitedAuthnResponse = config.AllowUnsolicitedAuthnResponse;
            this.spOptions = spOptions;

            var certificate = config.SigningCertificate.LoadCertificate();

            if (certificate != null)
            {
                SigningKey = certificate.PublicKey.Key;
            }

            if (config.LoadMetadata)
            {
                LoadMetadata();
            }

            Validate();
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="metadata">Metadata to use to configure the identity provider.</param>
        /// <param name="allowUnsolicitedAuthnResponse">Are unsolicited responses allowed from this idp?</param>
        /// <param name="spOptions">Service Provider option to use when creating AuthnRequests.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "sp")]
        public IdentityProvider(EntityDescriptor metadata, bool allowUnsolicitedAuthnResponse, ISPOptions spOptions)
        {
            AllowUnsolicitedAuthnResponse = allowUnsolicitedAuthnResponse;
            this.spOptions = spOptions;
            LoadMetadata(metadata);
            Validate();
        }

        private void Validate()
        {
            if(Binding == 0)
            {
                throw new ConfigurationErrorsException("Missing binding configuration on Idp " + EntityId.Id + ".");
            }

            if(SigningKey == null)
            {
                throw new ConfigurationErrorsException("Missing signing certificate configuration on Idp " + EntityId.Id + ".");
            }

            if (SingleSignOnServiceUrl == null)
            {
                throw new ConfigurationErrorsException("Missing assertion consumer service url configuration on Idp " + EntityId.Id + ".");
            }
        }

        /// <summary>
        /// The binding used when sending AuthnRequests to the identity provider.
        /// </summary>
        public Saml2BindingType Binding { get; private set; }

        /// <summary>
        /// The Url of the single sign on service. This is where the browser is redirected or
        /// where the post data is sent to when sending an AuthnRequest to the idp.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "SignOn")]
        public Uri SingleSignOnServiceUrl { get; private set; }

        /// <summary>
        /// The Entity Id of the identity provider.
        /// </summary>
        public EntityId EntityId { get; private set; }

        /// <summary>
        /// Is this idp allowed to send unsolicited responses, i.e. idp initiated sign in?
        /// </summary>
        public bool AllowUnsolicitedAuthnResponse { get; private set; }

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
            if(authServicesUrls == null)
            {
                throw new ArgumentNullException("authServicesUrls");
            }

            var authnRequest = new Saml2AuthenticationRequest()
            {
                DestinationUri = SingleSignOnServiceUrl,
                AssertionConsumerServiceUrl = authServicesUrls.AssertionConsumerServiceUrl,
                Issuer = spOptions.EntityId,
                // For now we only support one attribute consuming service.
                AttributeConsumingServiceIndex = spOptions.AttributeConsumingServices.Any() ? 0 :  (int?)null
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

        /// <summary>
        /// The public key of the idp that is used to verify signatures of responses/assertions.
        /// </summary>
        public AsymmetricAlgorithm SigningKey { get; private set; }

        private void LoadMetadata()
        {
            // So far only support for metadata at well known location.
            var metadata = MetadataLoader.LoadIdp(new Uri(EntityId.Id));

            LoadMetadata(metadata);
        }

        private void LoadMetadata(EntityDescriptor metadata)
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

            Binding = Saml2Binding.UriToSaml2BindingType(ssoService.Binding);
            SingleSignOnServiceUrl = ssoService.Location;

            var key = idpDescriptor.Keys
                .Where(k => k.Use == KeyType.Unspecified || k.Use == KeyType.Signing)
                .SingleOrDefault();

            if (key != null)
            {
                SigningKey = ((AsymmetricSecurityKey)key.KeyInfo.CreateKey())
                    .GetAsymmetricAlgorithm(SignedXml.XmlDsigRSASHA1Url, false);
            }
        }
    }
}
