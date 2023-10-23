using Sustainsys.Saml2.Common;
using Sustainsys.Saml2.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Serialization;

/// <summary>
/// Reader for data from an Xml Document.
/// </summary>
public partial class SamlXmlReader : ISamlXmlReader
{
    /// <inheritdoc/>
    public virtual IEnumerable<string>? AllowedHashAlgorithms { get; set; } =
        new[]
        {
            SignedXml.XmlDsigRSASHA256Url,
            SignedXml.XmlDsigRSASHA384Url,
            SignedXml.XmlDsigRSASHA512Url,
            SignedXml.XmlDsigDSAUrl
        };

    /// <inheritdoc/>
    public virtual IEnumerable<SigningKey>? TrustedSigningKeys { get; set; }

    /// <inheritdoc/>
    public virtual Func<string, Saml2Entity>? EntityResolver { get; set; }

    /// <summary>
    /// Helper method that calls ThrowOnErrors. To supress errors and prevent 
    /// throwing, this is the last chance method to override.
    /// </summary>
    protected virtual void ThrowOnErrors(XmlTraverser source)
        => source.ThrowOnErrors();
}
