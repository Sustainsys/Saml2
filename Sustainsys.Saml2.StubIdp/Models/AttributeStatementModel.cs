using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Kentor.AuthServices.StubIdp.Models
{
    public class AttributeStatementModel
    {
        [Required]
        public string Type { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
