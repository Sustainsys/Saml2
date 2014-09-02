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
    public class AssertionModel
    {
        [Required]
        [Display(Name="Assertion Consumer Service Url")]
        public string AssertionConsumerServiceUrl { get; set; }

        [Display(Name = "Subject NameId")]
        public string NameId { get; set; }

        /// <summary>
        /// Creates a new Assertion model with values from web.config
        /// </summary>
        /// <returns>An <see cref="AssertionModel"/></returns>
        public static AssertionModel CreateFromConfiguration()
        {
            return new AssertionModel
            {
                AssertionConsumerServiceUrl = ConfigurationManager.AppSettings["defaultAcsUrl"],
                NameId = ConfigurationManager.AppSettings["defaultNameId"]
            };
        }

        [Display(Name = "In Response To ID")]
        public string InResponseTo { get; set; }

        // The X509KeyStorageFlags.MachineKeySet flag is required when loading a
        // certificate from file on a shared hosting solution such as Azure.
        private static readonly X509Certificate2 signingCertificate =
            new X509Certificate2(HttpContext.Current.Server.MapPath(
                "~\\App_Data\\Kentor.AuthServices.StubIdp.pfx"), "",
                X509KeyStorageFlags.MachineKeySet);

        public Saml2Response ToSaml2Response()
        {
            var identity = new ClaimsIdentity(new Claim[] { 
                new Claim(ClaimTypes.NameIdentifier, NameId )});

            return new Saml2Response(
                new EntityId(ConfigurationManager.AppSettings["issuerName"]),
                signingCertificate, new Uri(AssertionConsumerServiceUrl), InResponseTo, identity);
        }
    }
}