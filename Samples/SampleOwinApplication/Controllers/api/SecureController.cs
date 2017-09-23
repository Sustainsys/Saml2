using System.Web.Http;

namespace SampleOwinApplication.Controllers.api
{
    [Authorize]
    public class SecureController : ApiController
    {
        //
        // GET: /api/Secure

        public IHttpActionResult Get()
        {
            var response = new { Message = "User is authenticated", User = User.Identity.Name };

            return Ok(response);
        }
    }
}
