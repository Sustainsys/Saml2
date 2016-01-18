using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.WebSso
{
    /// <summary>
    /// Saml2 binding types.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Justification="Might do that in the future, but not right now")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum Saml2BindingType
    {
        /// <summary>
        /// The http redirect binding according to saml bindings section 3.4
        /// </summary>
        HttpRedirect = 1,

        /// <summary>
        /// The http post binding according to saml bindings section 3.5
        /// </summary>
        HttpPost = 2,

        /// <summary>
        /// The artifact resolution binding according to bindings section 3.6
        /// </summary>
        Artifact = 4,
    }
}
