using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
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
        /// Return Uri to redirect the client to, if no return uri was specified
        /// when initiating the signin sequence.
        /// </summary>
        public Uri ReturnUri { get; set; }

        /// <summary>
        /// Return Uri to redirect the client to, if no return uri was specified
        /// when initiating the signin sequence.
        /// </summary>
        public TimeSpan MetadataCacheDuration { get; set; }

        volatile private Saml2PSecurityTokenHandler saml2PSecurityTokenHandler;
        volatile bool automaticSaml2PSecurityTokenHandlerInstance;

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
                    saml2PSecurityTokenHandler = value = new Saml2PSecurityTokenHandler(EntityId);
                    automaticSaml2PSecurityTokenHandlerInstance = true;
                }

                return value;
            }
            set
            {
                saml2PSecurityTokenHandler = value;
                automaticSaml2PSecurityTokenHandlerInstance = false;
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
                if(automaticSaml2PSecurityTokenHandlerInstance)
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
                    throw new ArgumentNullException("value");
                }

                value = value.TrimEnd('/');

                if (!value.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    value = "/" + value;
                }

                modulePath = value;
            }
        }
    }
}
