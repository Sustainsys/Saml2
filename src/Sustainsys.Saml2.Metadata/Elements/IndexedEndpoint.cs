using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Metadata.Elements;

/// <summary>
/// Metadata IndexedEndpoint
/// </summary>
public class IndexedEndpoint : Endpoint
{
    /// <summary>
    /// Inded of endpoint.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Is this the default enpdoint to use?
    /// </summary>
    public bool IsDefault { get; set; }
}
