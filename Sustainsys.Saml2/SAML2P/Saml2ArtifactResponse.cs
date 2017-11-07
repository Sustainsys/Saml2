using Kentor.AuthServices.Exceptions;
using Kentor.AuthServices.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
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

            Status = StatusCodeHelper.FromString(
                xml["Status", Saml2Namespaces.Saml2PName]
                ["StatusCode", Saml2Namespaces.Saml2PName]
                .GetAttribute("Value"));

            if (Status == Saml2StatusCode.Success)
            {
                message = xml.ChildNodes.OfType<XmlElement>()
                    .SkipWhile(x => x.LocalName != "Status")
                    .Skip(1).Single();
            }
        }

        XmlElement message;

        /// <summary>
        /// Contained message.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public XmlElement GetMessage()
        {
            if(Status != Saml2StatusCode.Success)
            {
                throw new UnsuccessfulSamlOperationException(string.Format(
                    CultureInfo.InvariantCulture,
                    "Artifact resolution returned status {0}, can't extract message.",
                    Status));
            }

            return message;
        }

        /// <summary>
        /// Status code of the Artifact response.
        /// </summary>
        public Saml2StatusCode Status { get; }
    }
}
