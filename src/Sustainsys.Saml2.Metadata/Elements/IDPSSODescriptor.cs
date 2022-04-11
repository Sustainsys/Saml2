using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Metadata.Elements;

/// <summary>
/// IDPSSODescriptor
/// </summary>
public class IDPSSODescriptor : SSODescriptor
{
    /// <summary>
    /// List of SingleSignOnService endpoints.
    /// </summary>
    public List<Endpoint> SingleSignOnServices { get; } = new();
    
    /// <summary>
    /// Does the Idp wants any AuthnRequests to be signed?
    /// </summary>
    public bool WantAuthnRequestsSigned { get; set; }
}
