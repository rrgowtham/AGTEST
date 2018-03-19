using AHP.Web.Helpers;
using System.Web.Mvc;
using System;
using System.Net;
using System.IO;
using System.Web.UI;
using System.Linq;
using System.Linq.Expressions;
using AHP.Core.Logger;
using System.Text;
using System.Collections.Generic;
using AHP.Web.Models;

namespace AHP.Web.Controllers
{
    [OutputCache(Duration = 0,NoStore = true,Location = OutputCacheLocation.None)]
    public class ReportsController : BaseController
    {

        #region -- Members --

        private readonly IServerDataRestClient _restClient;

        private readonly IActiveAnalyticsLogger _logger;

        private readonly StringBuilder _logMessage;

        private ITableauRestConnector _tableauClient;

        #endregion

        #region -- Constructors --

        public ReportsController(IActiveAnalyticsLogger logger,IServerDataRestClient restClient,ITableauRestConnector tableauClient)
        {
            _restClient = restClient;
            _logger = logger;
            _logMessage = new StringBuilder();
            _tableauClient = tableauClient;
        }

        #endregion

        #region -- Action Methods --

        [HttpGet]
        public ActionResult Show(string id)
        {
            ViewBag.Title = "Your Reports";
            ViewData["OpenDocumentUri"] = string.Format(System.Configuration.ConfigurationManager.AppSettings["RelativeOpenDocUrl"],id);
            _logger.Info("Relative Open document url from config "+ ViewData["OpenDocumentUri"] + ".");
            //ViewData["OpenDocumentUri"] = _restClient.GetReport(Identity.SapToken, Identity.BOSerializedSessionId, id);            
            return View("Show");
        }
        
        [HttpGet]
        public ActionResult OpenDocument(string rdm)
        {
            string output = "Could not find the report requested";
            try
            {
                _logMessage.Append("Opening the report.");
                if (string.IsNullOrEmpty(rdm))
                {
                    _logMessage.Append("Passed the report id "+ rdm + ".");
                    return Content(output);
                }
                                
                string opendocUrl = _restClient.GetReport(Identity.SapToken, Identity.BOSerializedSessionId, rdm);

                _logMessage.Append("Opendocument url obtain from rest client "+ opendocUrl + ".");

                Uri openDocumentUriObject;

                if (Uri.TryCreate(opendocUrl,UriKind.RelativeOrAbsolute,out openDocumentUriObject))
                {
                    HttpWebRequest opendocRequest = (HttpWebRequest)WebRequest.Create(opendocUrl);
                    opendocRequest.Method = "GET";
                    opendocRequest.Accept = "text/html";
                    using (HttpWebResponse openDocResponse = (HttpWebResponse)opendocRequest.GetResponse())
                    {
                        using (Stream getRequestResponseStream = openDocResponse.GetResponseStream())
                        {
                            using (StreamReader sr = new StreamReader(getRequestResponseStream))
                            {
                                output = sr.ReadToEnd();
                                sr.Close();
                            }
                        }
                    }
                    _logMessage.Append("Sending the opendocument request content.");
                    return Content(output);
                }
                else
                {
                    output = "Reporting system did not respond with proper report.";
                }                
            }
            catch (Exception ex)
            {
                _logMessage.Append("An Error occurred Exception Message "+ ex.Message);
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            _logger.Info(_logMessage.ToString());
            return Content(output);
        }

        [HttpGet]
        public ActionResult ShowLaunchpad(string id)
        {
            string output = "Could not process your request";
            if (string.IsNullOrEmpty(Identity.TableauAuthTicket))
            {
                ViewBag.Error = output;
                return View();
            }

            string tableauLaunchpadUrl = System.Configuration.ConfigurationManager.AppSettings["tableauServerUrl"];

            if (string.IsNullOrEmpty(tableauLaunchpadUrl))
            {
                //show the could not process request response
                ViewBag.Error = output;
                return View();
            }

            _logMessage.AppendFormat("User {0} requested {1} tableau view. Checking if user has view access", Identity.UserName, id);

            try
            {
                GenericAjaxResponse<List<Core.DTO.TableauViewInfo>> apiResponse = _restClient.GetUsersViews(Identity.UserName, Identity.IsInternalUser ? "Internal" : "External");

                if (!apiResponse.Success || apiResponse.Data == null || apiResponse.Data.Count <= 0)
                {
                    //show the could not process request response
                    ViewBag.Error = output;
                    return View();
                }

                Core.DTO.TableauViewInfo viewToDisplay = apiResponse.Data.FirstOrDefault(vw => vw.ViewId.Equals(id, StringComparison.OrdinalIgnoreCase));

                if (viewToDisplay == null)
                {
                    ViewBag.Error = output;
                    return View();
                }

                //construct tableau embed url
                string tableauEmbedUrl = string.Format("{0}/trusted/{1}/{2}", tableauLaunchpadUrl, Identity.TableauAuthTicket, viewToDisplay.ViewUrl);

                _logMessage.AppendFormat("Tableau embed url {0}.", tableauEmbedUrl);

                ViewBag.TableauUri = tableauEmbedUrl;
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        string error = reader.ReadToEnd();
                        _logger.Error("An Error occurred during tableu report pull , Error detail " + error + ".", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _logMessage.AppendFormat("Error message {0} while loading view", ex.Message);
                ViewBag.Error = output;
            }
            _logger.Info(_logMessage.ToString());
            return View();
        }        

        #endregion

    }
}