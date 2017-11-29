using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleIdentityServer4
{
    [Route("myapi")]
    public class MyApi : ControllerBase
    {
        [HttpGet]
        [Authorize]
        // TODO: Set Authentication Scheme to JWT handler.
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}
