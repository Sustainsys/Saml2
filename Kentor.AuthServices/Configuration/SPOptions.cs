using Kentor.AuthServices.Metadata;
using Kentor.AuthServices.Saml2P;
using System;
using System.Collections.Generic;
using System.IdentityModel.Configuration;
using System.IdentityModel.Metadata;
using System.IdentityModel.Services.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Options for the service provider's behaviour; i.e. everything except
    /// the idp and federation list.
    /// </summary>
    public class SPOptions : ISPOptions
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public SPOptions()
        {
            MetadataCacheDuration = new TimeSpan(1, 0, 0);
        }

        /// <summary>
        /// Return Uri to redirect the client to, if no return uri was specified
        /// when initiating the signin sequence.
        /// </summary>
        public Uri ReturnUrl { get; set; }

        /// <summary>
        /// Return Uri to redirect the client to, if no return uri was specified
        /// when initiating the signin sequence.
        /// </summary>
        public TimeSpan MetadataCacheDuration { get; set; }

        volatile private Saml2PSecurityTokenHandler saml2PSecurityTokenHandler;

        /// <summary>
        /// The security token handler used to process incoming assertions for this SP.
        /// The default value is to lazy create one using the current EntityId.
        /// </summary>
        public Saml2PSecurityTokenHandler Saml2PSecurityTokenHandler
        {
            get
            {
                // Capture in a local variable to prevent race conditions. Reads and writes
                // of references are atomic so there is no need for a lock.
                var value = saml2PSecurityTokenHandler;
                if(value == null)
                {
                    // Set the saved value, but don't trust it - still use a local var for the return.
                    saml2PSecurityTokenHandler = value = new Saml2PSecurityTokenHandler(this);
                }

                return value;
            }
        }

        /// <summary>
        /// Url to discovery service to use if no idp is specified in the sign in call.
        /// </summary>
        public Uri DiscoveryServiceUrl { get; set; }

        private EntityId entityId;

        /// <summary>
        /// EntityId - The identity of the ServiceProvider to use when sending requests to Idp
        /// and presenting the SP in metadata.
        /// </summary>
        public EntityId EntityId
        {
            get
            {
                return entityId;
            }
            set
            {
                if(saml2PSecurityTokenHandler != null)
                {
                    throw new InvalidOperationException("Can't change entity id when a token handler has been instantiated.");
                }
                entityId = value;
            }
        }

        private string modulePath = "/AuthServices";

        /// <summary>
        /// Application root relative path for AuthServices endpoints. The
        /// default is "/AuthServices".
        /// </summary>
        public string ModulePath
        {
            get
            {
                return modulePath;
            }
            set
            {
                if(value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                value = value.TrimEnd('/');

                if (!value.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    value = "/" + value;
                }

                modulePath = value;
            }
        }

        /// <summary>
        /// Metadata describing the organization responsible for the entity.
        /// </summary>
        public Organization Organization { get; set; }

        /// <summary>
        /// Contacts for the SAML2 entity.
        /// </summary>
        IEnumerable<ContactPerson> ISPOptions.Contacts
        {
            get
            {
                return Contacts;
            }
        }

        readonly ICollection<ContactPerson> contacts = new List<ContactPerson>();

        /// <summary>
        /// Collection of contacts for the SAML2 entity.
        /// </summary>
        public ICollection<ContactPerson> Contacts
        {
            get
            {
                return contacts;
            }
        }

        /// <summary>
        /// Collection of attribute consuming services for the service provider.
        /// </summary>
        IEnumerable<AttributeConsumingService> ISPOptions.AttributeConsumingServices
        {
            get
            {
                return AttributeConsumingServices;
            }
        }

        readonly ICollection<AttributeConsumingService> attributeConsumingServices = new List<AttributeConsumingService>();

        /// <summary>
        /// Collection of attribute consuming services for the service provider.
        /// </summary>
        public ICollection<AttributeConsumingService> AttributeConsumingServices
        {
            get
            {
                return attributeConsumingServices;
            }
        }

        private IdentityConfiguration systemIdentityModelIdentityConfiguration
            = new IdentityConfiguration(false);

        /// <summary>
        /// The System.IdentityModel configuration to use.
        /// </summary>
        public IdentityConfiguration SystemIdentityModelIdentityConfiguration
        {
            get
            {
                return systemIdentityModelIdentityConfiguration;
            }
        }

        /// <summary>
        /// Certificate for service provider to use when decrypting assertions
        /// </summary>
        public X509Certificate2 ServiceCertificate { get; set; }
    }
}
