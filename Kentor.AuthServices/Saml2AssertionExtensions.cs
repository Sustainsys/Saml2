using System;
using System.IdentityModel.Tokens;
using System.Xml.Linq;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Extension methods for Saml2Assertion
    /// </summary>
    public static class Saml2AssertionExtensions
    {
        /// <summary>
        /// Writes out the assertion as an XElement.
        /// </summary>
        /// <param name="assertion">The assertion to create xml for.</param>
        /// <returns>XElement</returns>
        public static XElement ToXElement(this Saml2Assertion assertion)
        {
            throw new NotImplementedException();
        }
    }
}
