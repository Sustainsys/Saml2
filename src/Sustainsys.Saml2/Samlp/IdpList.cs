using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Samlp;
/// <summary>
/// An advisory list of identity providers and associated information.
/// Element IdpList, 3.4.1.3
/// </summary>
public class IdpList
{
    /// <summary>
    /// Specifies a single identity provider.
    /// </summary>
    public List<IdpEntry> IdpEntries { get; } = [];

    /// <summary>
    /// If the IdpList is not complete, use URI reference.
    /// </summary>
    public string? GetComplete { get; set; }

}