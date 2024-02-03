using Sustainsys.Saml2.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Validation;

/// <summary>
/// Exception thrown when validation is not successful
/// </summary>
public class SamlValidationException : Exception
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="message">message</param>
    public SamlValidationException(string message) : base(message) { }
}
