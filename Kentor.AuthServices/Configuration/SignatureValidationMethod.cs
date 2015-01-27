using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Enumeration of configuration options that control how validation of signatures will be handled.
    /// </summary>
    public enum SignatureValidationMethod
    {
        /// <summary>
        /// The default method is to validate signatures if present.
        /// </summary>
        Default,
        /// <summary>
        /// This will skip the validation of signatures even if they are present.
        /// </summary>
        Skip,
        /// <summary>
        /// This option will make AuthServices disallow unsigned messages. 
        /// </summary>
        Demand
    }
}
