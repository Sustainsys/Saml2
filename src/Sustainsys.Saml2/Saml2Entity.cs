using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2;
/// <summary>
/// A Saml2 entity, i.e. an Identity Provider or a Service Provider
/// </summary>
public class Saml2Entity
{
    /// <summary>
    /// The entity id of the identity provider
    /// </summary>
    public string? EntityId { get; set; }
}
