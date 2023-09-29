using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Samlp;

/// <summary>
/// Authentication request
/// </summary>
public class AuthnRequest : RequestAbstractType
{
    /// <summary>
    /// The assertion consumer service Url where the response should be posted back to.
    /// </summary>
    public string? AssertionConsumerServiceUrl { get; set; }
}
