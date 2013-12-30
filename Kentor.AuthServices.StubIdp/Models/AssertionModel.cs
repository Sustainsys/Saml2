using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
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
        [Display(Name="Assertion Consumer Url")]
        public string AssertionConsumerUrl { get; set; }

        [Display(Name="Subject NameId")]
        public string NameId { get; set; }

        private static readonly AssertionModel defaultInstance = new AssertionModel
        {
            AssertionConsumerUrl = ConfigurationManager.AppSettings["defaultAcsUrl"],
            NameId = ConfigurationManager.AppSettings["defaultNameId"]
        };

        public static AssertionModel Default
        {
            get
            {
                return defaultInstance;
            }
        }

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
                ConfigurationManager.AppSettings["issuerName"],
                signingCertificate, new Uri(AssertionConsumerUrl), identity);
        }
    }
}