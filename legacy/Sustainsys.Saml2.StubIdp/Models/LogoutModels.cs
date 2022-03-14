using Sustainsys.Saml2.Saml2P;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Metadata;
using Sustainsys.Saml2.Metadata;
using Microsoft.IdentityModel.Tokens.Saml2;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Web;
using System.Xml;

namespace Sustainsys.Saml2.StubIdp.Models
{
    public class InitiateLogoutModel
    {
        public InitiateLogoutModel()
        {
            SessionIndex = AssertionModel.DefaultSessionIndex;
        }

        [Required]
        [DisplayName("SP Single Logout Url")]
        public Uri DestinationUrl { get; set; }

        [Required]
        [DisplayName("Session index")]
        public string SessionIndex { get; set; }

        [Required]
        [DisplayName("Subject NameID")]
        public string NameId { get; set; }

        public Saml2LogoutRequest ToLogoutRequest()
        {
            return new Saml2LogoutRequest()
            {
                DestinationUrl = DestinationUrl,
                SigningCertificate = CertificateHelper.SigningCertificate,
                SigningAlgorithm = SignedXml.XmlDsigRSASHA256Url,
                Issuer = new Metadata.EntityId(UrlResolver.MetadataUrl.ToString()),
                NameId = new Saml2NameIdentifier(NameId),
                SessionIndex = SessionIndex,
            };
        }
    }

    public class RespondToLogoutRequestModel
    {
        [DisplayName("Received LogoutRequest")]
        public string LogoutRequestXml { get; set; }

        [DisplayName("InResponseTo")]
        public string InResponseTo { get; set; }

        [Required]
        [DisplayName("SP Single Logout Url")]
        public Uri DestinationUrl { get; set; }

        [DisplayName("Relay State")]
        public string RelayState { get; set; }

        public Saml2LogoutResponse ToLogoutResponse()
        {
            return new Saml2LogoutResponse(Saml2StatusCode.Success)
            {
                DestinationUrl = DestinationUrl,
                SigningCertificate = CertificateHelper.SigningCertificate,
                SigningAlgorithm = SignedXml.XmlDsigRSASHA256Url,
                InResponseTo = new Saml2Id(InResponseTo),
                Issuer = new Metadata.EntityId(UrlResolver.MetadataUrl.ToString()),
                RelayState = RelayState
            };
        }
    }

    public class ResponseModel
    {
        [DisplayName("Response Status")]
        public string Status { get; set; }

        [DisplayName("Response XML")]
        public string ResponseXml { get; set; }
    }
}