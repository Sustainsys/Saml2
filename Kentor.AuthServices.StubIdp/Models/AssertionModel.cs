using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Kentor.AuthServices.StubIdp.Models
{
    public class AssertionModel
    {
        [Required]
        [Display(Name="Assertion Consumer Url")]
        public string AssertionConsumerUrl { get; set; }

        [Display(Name="Assertion NameId")]
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
    }
}