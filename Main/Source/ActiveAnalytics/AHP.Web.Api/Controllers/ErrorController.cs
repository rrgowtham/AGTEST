using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace AHP.Web.Api.Controllers
{
 
    public class ErrorController : ApiBaseController
    {
        [HttpGet, HttpPost, HttpPut, HttpDelete, HttpHead, HttpOptions]
        public IHttpActionResult NotFound(string path)
        {
            // log error to ELMAH
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new HttpException(404, "404 Not Found: /" + path)));

            // return 404
            return NotFound();
        }
    }
    
}