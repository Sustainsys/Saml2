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
        [Description("Incoming LogoutRequest")]
        public string LogoutRequestXml { get; set; }

        [Required]
        [DisplayName("Single Logout Service Response Url")]
        public string SingleLogoutResponseUrl { get; set; }
    }
}