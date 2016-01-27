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
        private bool allowChange = false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "allowChange")]
        internal void AllowChange(bool allowChange)
        {
            this.allowChange = allowChange;
        }

        /// <summary>
        /// Used for testing, always returns true in production.
        /// </summary>
        /// <returns>Returns true (unless during tests)</returns>
        public override bool IsReadOnly()
        {
            return !allowChange;
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
