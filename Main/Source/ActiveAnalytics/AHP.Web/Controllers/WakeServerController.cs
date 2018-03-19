using AHP.Core.Logger;
using AHP.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AHP.Web.Controllers
{
    public class WakeServerController : BaseController
    {
        private readonly IServerDataRestClient _restClient;

        private readonly IActiveAnalyticsLogger _logger;

        public WakeServerController(IServerDataRestClient restClient,IActiveAnalyticsLogger logger)
        {
            _restClient = restClient;
            _logger = logger;
        }

        [HttpPost]
        public ActionResult IsLoggedOn()
        {
            bool response = false;
            try
            {
               response = _restClient.IsLoggedOn(Identity.BOSessionId);
            }
            catch (Exception ex)
            {
                response = false;
                _logger.Error("Error occurred waking up server",ex);
            }
            return Json(new { Success = response });
        }

        // GET: WakeServer
        public ActionResult Index()
        {
            return Content("");
        }
    }
}