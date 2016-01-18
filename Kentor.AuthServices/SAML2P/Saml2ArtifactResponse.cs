using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// A Saml2 ArtifactResponse message as specified in SAML2 Core 3.5.2.
    /// </summary>
    public class Saml2ArtifactResponse
    {
        /// <summary>
        /// Read an artifact response from a string.
        /// </summary>
        /// <param name="xml">Parsed XML with message.</param>
        /// <returns>Saml2ArtifactResponse object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
        public static Saml2ArtifactResponse Load(XmlElement xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException(nameof(xml));
            }

            return new Saml2ArtifactResponse(
                xml.ChildNodes.OfType<XmlElement>()
                .SkipWhile(x => x.LocalName != "Status")
                .Skip(1).Single());
        }

        private Saml2ArtifactResponse(XmlElement message)
        {
            Message = message;
        }

        /// <summary>
        /// Contained message.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
        public XmlElement Message { get; }
    }
}
