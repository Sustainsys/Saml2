using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Options for the service provider's behaviour; i.e. everything except
    /// the idp and federation list.
    /// </summary>
    public interface ISPOptions
    {
        /// <summary>
        /// Return Uri to redirect the client to, if no return uri was specified
        /// when initiating the signin sequence.
        /// </summary>
        Uri ReturnUri { get; }

        /// <summary>
        /// Optional attribute that describes for how long in seconds anyone may cache the metadata 
        /// presented by the service provider. Defaults to 3600 seconds.
        /// </summary>
        TimeSpan MetadataCacheDuration { get; }
    }
}
