using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AHP.Web.Controllers
{
    [AllowAnonymous]
    public class ErrorController : BaseController
    {
        // GET: Error
        public ActionResult Index()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}