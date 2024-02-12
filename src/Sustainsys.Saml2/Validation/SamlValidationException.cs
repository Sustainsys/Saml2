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
/// <param name="message">message</param>
public class SamlValidationException(string message) : Exception(message)
{ }
