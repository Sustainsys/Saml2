using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Base class for saml requests, corresponds to section 3.2.1 in SAML Core specification.
    /// </summary>
    public abstract class Saml2RequestBase
    {
        private readonly string id = "id" + Guid.NewGuid().ToString("N");

        /// <summary>
        /// The id of the request.
        /// </summary>
        public string Id
        {
            get
            {
                return id;
            }
        }
    }
}
