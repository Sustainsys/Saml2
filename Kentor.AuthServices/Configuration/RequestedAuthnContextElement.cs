using Kentor.AuthServices.Saml2P;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Configuration of RequestedAuthnContext in generated AuthnRequests.
    /// </summary>
    public class RequestedAuthnContextElement : ConfigurationElement
    {
        internal bool AllowChange { get; set; }

        /// <summary>
        /// Used for testing, always returns true in production.
        /// </summary>
        /// <returns>Returns true (unless during tests)</returns>
        public override bool IsReadOnly()
        {
            return !AllowChange;
        }

        const string classRef = nameof(classRef);
        /// <summary>
        /// AuthnContextClassRef. Either a full URL or the last word of a
        /// standard URL.
        /// </summary>
        [ConfigurationProperty(classRef)]
        public string AuthnContextClassRef
        {
            get
            {
                return (string)base[classRef];
            }
            internal set
            {
                base[classRef] = value;
            }
        }

        const string comparison = nameof(comparison);
        /// <summary>
        /// Comparison mode of AuthnContextClassRef
        /// </summary>
        [ConfigurationProperty(comparison)]
        public AuthnContextComparisonType Comparison
        {
            get
            {
                return (AuthnContextComparisonType)base[comparison];
            }
        }
    }
}
