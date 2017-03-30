using System;
using System.Globalization;
using System.Linq;
using Kentor.AuthServices.Saml2P;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using Kentor.AuthServices.Configuration;
using System.Xml;

namespace Kentor.AuthServices.WebSso
{
    /// <summary>
    /// Saml2 Artifact binding.
    /// </summary>
    public class Saml2ArtifactBinding : Saml2Binding
    {
        internal Saml2ArtifactBinding() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected internal override bool CanUnbind(HttpRequestData request)
        {
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return (request.HttpMethod == "GET" && request.QueryString.Contains("SAMLart"))
                || (request.HttpMethod == "POST" && request.Form.ContainsKey("SAMLart"));
        }

        /// <summary>
        /// Checks if the binding can extract a message out of the current
        /// http request.
        /// </summary>
        /// <param name="request">HttpRequest to check for message.</param>
        /// <param name="options">Options used to look up details of issuing
        /// idp when needed (artifact binding).</param>
        /// <returns>True if the binding supports the current request.</returns>
        public override UnbindResult Unbind(HttpRequestData request, IOptions options)
        {
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if(options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            string relayState;
            string artifact;

            switch(request.HttpMethod)
            {
                case "GET":
                    relayState = request.QueryString["RelayState"].SingleOrDefault();
                    artifact = request.QueryString["SAMLart"].SingleOrDefault();
                    break;
                case "POST":
                    request.Form.TryGetValue("RelayState", out relayState);
                    artifact = request.Form["SAMLart"];
                    break;
                default:
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                        "Artifact binding can only use GET or POST http method, but found {0}",
                        request.HttpMethod));
            }

            options.SPOptions.Logger.WriteVerbose("Artifact binding found Artifact\n" + artifact);

            var data = ResolveArtifact(artifact, request.StoredRequestState, options);

            return new UnbindResult(data, relayState, TrustLevel.None);
        }

        private static XmlElement ResolveArtifact(
            string artifact,
            StoredRequestState storedRequestState,
            IOptions options)
        {
            var binaryArtifact = Convert.FromBase64String(artifact);
            var idp = GetIdp(binaryArtifact, storedRequestState, options);
            var arsIndex = (binaryArtifact[2] << 8) | binaryArtifact[3];
            var arsUri = idp.ArtifactResolutionServiceUrls[arsIndex];

            var payload = new Saml2ArtifactResolve()
            {
                Artifact = artifact,
                Issuer = options.SPOptions.EntityId
            }.ToXml();

            if (options.SPOptions.SigningServiceCertificate != null)
            {
                var xmlDoc = new XmlDocument()
                {
                    PreserveWhitespace = true
                };

                xmlDoc.LoadXml(payload);
                xmlDoc.Sign(options.SPOptions.SigningServiceCertificate, true);
                payload = xmlDoc.OuterXml;
            }

            options.SPOptions.Logger.WriteVerbose("Calling idp " + idp.EntityId.Id + " to resolve artifact\n" + artifact);

            var response = Saml2SoapBinding.SendSoapRequest(payload, arsUri);

            options.SPOptions.Logger.WriteVerbose("Artifact resolved returned\n" + response);

            return new Saml2ArtifactResponse(response).GetMessage();
        }

        private static IdentityProvider GetIdp(
            byte[] binaryArtifact,
            StoredRequestState storedRequestState,
            IOptions options)
        {
            if(storedRequestState != null)
            {
                return options.IdentityProviders[storedRequestState.Idp];
            }

            // It is RECOMMENDED in the spec that the first part of the artifact
            // is the SHA1 of the entity id, so let's try that as a fallback.
            var sourceId = new byte[20];
            Array.Copy(binaryArtifact, 4, sourceId, 0, 20);

            return options.IdentityProviders.KnownIdentityProviders
                .Single(idp => sha1.ComputeHash(
                    Encoding.UTF8.GetBytes(idp.EntityId.Id))
                    .SequenceEqual(sourceId));
        }

        private static SHA1 sha1 = SHA1.Create();

        /// <summary>
        /// Create a SAML artifact value.
        /// </summary>
        /// <param name="issuer">Entity id of the artifact issuer.</param>
        /// <param name="endpointIndex">Index of the artifact resolution endpoint
        /// that the requester should use to resolve the artifact.</param>
        public static byte[] CreateArtifact(EntityId issuer, int endpointIndex)
        {
            if(issuer == null)
            {
                throw new ArgumentNullException(nameof(issuer));
            }

            var artifact = new byte[44];
            artifact[1] = 4; // Header is 0004

            artifact[2] = (byte)(endpointIndex >> 8);
            artifact[3] = (byte)endpointIndex;

            Array.Copy(sha1.ComputeHash(Encoding.UTF8.GetBytes(issuer.Id)),
                0, artifact, 4, 20);

            Array.Copy(SecureKeyGenerator.CreateArtifactMessageHandle(), 0, artifact, 24, 20);

            return artifact;
        }

        /// <summary>
        /// Binds a message to a http response with HTTP Redirect.
        /// </summary>
        /// <param name="message">Message to bind.</param>
        /// <param name="logger">Logger to use.</param>
        /// <returns>CommandResult.</returns>
        public override CommandResult Bind(ISaml2Message message, ILoggerAdapter logger)
        {
            if(message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var artifact = CreateArtifact(message.Issuer, 0);

            ((IDictionary<byte[], ISaml2Message>)PendingMessages).Add(artifact, message);

            var relayParam = string.IsNullOrEmpty(message.RelayState) ? "" 
                : "&RelayState=" + Uri.EscapeDataString(message.RelayState);

            return new CommandResult
            {
                HttpStatusCode = System.Net.HttpStatusCode.SeeOther,
                Location = new Uri(message.DestinationUrl.OriginalString
                + (string.IsNullOrEmpty(message.DestinationUrl.Query) ? "?" : "&")
                + "SAMLart=" + Uri.EscapeDataString(Convert.ToBase64String(artifact))
                + relayParam)
            };
        }

        /// <summary>
        /// Pending messages where the artifact has been sent.
        /// </summary>
        public static ConcurrentDictionary<byte[], ISaml2Message> PendingMessages { get; } =
            new ConcurrentDictionary<byte[], ISaml2Message>(new ByteArrayEqualityComparer());

        private class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
        {
            public bool Equals(byte[] x, byte[] y)
            {
                return x.SequenceEqual(y);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
            public int GetHashCode(byte[] obj)
            {
                return Enumerable.Range(0, obj.Length / 4)
                    .Select(i => BitConverter.ToInt32(obj, i * 4))
                    .Aggregate((a, b) => a ^ b);
            }
        }
    }
}