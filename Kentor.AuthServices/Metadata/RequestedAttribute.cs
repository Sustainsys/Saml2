using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;

namespace Kentor.AuthServices.Metadata
{
    /// <summary>
    /// Specifies an attribute requested by the service provider.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public class RequestedAttribute : Saml2Attribute
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">Name of the attribute.</param>
        public RequestedAttribute(string name)
            : base(name)
        { }

        /// <summary>
        /// Is this attribute required by the service provider?
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Uri used for NameFormat to specify that the Name is a Uri.
        /// </summary>
        public static readonly Uri AttributeNameFormatUri = new Uri("urn:oasis:names:tc:SAML:2.0:attrname-format:uri");

        /// <summary>
        /// Uri used for NameFormat to specify that the format of the Name 
        /// is unspecified.
        /// </summary>
        public static readonly Uri AttributeNameFormatUnspecified = new Uri("urn:oasis:names:tc:SAML:2.0:attrname-format:unspecified");

        /// <summary>
        /// Uri used for NameFormat to specify that the format of the Name 
        /// fulfills the standard's basic requirements.
        /// </summary>
        public static readonly Uri AttributeNameFormatBasic = new Uri("urn:oasis:names:tc:SAML:2.0:attrname-format:basic");
    }
}
