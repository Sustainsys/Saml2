using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Stored data for each PendingAuthnRequest
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "AuthnRequest")] // AuthnRequest is already in dictionary
    public class PendingAuthnRequestData
    {
        /// <summary>
        /// Creates a PendingAuthnRequestData
        /// </summary>
        /// <param name="idp">The IDP the request was sent to</param>
        /// <param name="returnUri">The Uri to redirect back to after a succesful login</param>
        public PendingAuthnRequestData(string idp, Uri returnUri)
        {
            Idp = idp;
            ReturnUri = returnUri;
        }

        /// <summary>
        /// The IDP the request was sent to
        /// </summary>
        public string Idp { get; private set; }

        /// <summary>
        /// The Uri to redirect back to after a succesful login
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public Uri ReturnUri { get; private set; }
    }
}
