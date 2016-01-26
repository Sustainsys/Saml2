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
        /// Ctor
        /// </summary>
        /// <param name="xml">Parsed XML with message.</param>
        public Saml2ArtifactResponse(XmlElement xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException(nameof(xml));
            }

            Message = xml.ChildNodes.OfType<XmlElement>()
                .SkipWhile(x => x.LocalName != "Status")
                .Skip(1).Single();
        }

        /// <summary>
        /// Contained message.
        /// </summary>
        public XmlElement Message { get; }
    }
}
