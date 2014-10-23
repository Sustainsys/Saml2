using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;

namespace Kentor.AuthServices
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public class RequestedAttribute : Saml2Attribute
    {
        public RequestedAttribute(string name)
            : base(name)
        { }

        /// <summary>
        /// Is this attribute required by the service provider?
        /// </summary>
        public bool IsRequired { get; set; }

        public static readonly Uri AttributeNameFormatUri = new Uri("urn:oasis:names:tc:SAML:2.0:attrname-format:uri");
        public static readonly Uri AttributeNameFormatUnspecified = new Uri("urn:oasis:names:tc:SAML:2.0:attrname-format:unspecified");
        public static readonly Uri AttributeNameFormatBasic = new Uri("urn:oasis:names:tc:SAML:2.0:attrname-format:basic");
    }
}
