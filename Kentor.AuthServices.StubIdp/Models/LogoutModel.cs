using Kentor.AuthServices.Saml2P;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kentor.AuthServices.StubIdp.Models
{
    public class LogoutModel
    {
        [DisplayName("Incoming LogoutRequest")]
        public string LogoutRequestXml { get; set; }

        [Required]
        [DisplayName("Single Logout Service Response Url")]
        public Uri SingleLogoutResponseUrl { get; set; }

        public Saml2LogoutResponse ToLogoutResponse()
        {
            return new Saml2LogoutResponse(Saml2StatusCode.Success)
            {
                DestinationUrl = SingleLogoutResponseUrl
            };
        }
    }
}