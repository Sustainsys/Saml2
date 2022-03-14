using Sustainsys.Saml2.Saml2P;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Hosting;

namespace Sustainsys.Saml2.StubIdp.Models
{
    public class ManageIdpModel
    {
        [Required]
        [Display(Name = "Configuration data")]
        [DataType(DataType.MultilineText)]
        public string JsonData { get; set; }
    }
}