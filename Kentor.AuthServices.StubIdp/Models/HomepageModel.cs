using Kentor.AuthServices.Saml2P;
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

namespace Kentor.AuthServices.StubIdp.Models
{
    public class HomePageModel
    {
        [Required]
        public AssertionModel AssertionModel { get; set; }

        public string CustomDescription { get; set; }

        public bool HideDetails { get; set; }
    }
}