using AHP.Core.Logger;
using AHP.Core.Model;
using AHP.Core.Service;
using AHP.Web.Api.Models;
using System.Configuration;
using System.Text;
using System.Web.Http;
using System.Windows.Forms;
using System.Xml;

namespace AHP.Web.Api.Controllers
{
    public class WebiReportsController : ApiBaseController
    {
        #region -- Members --

        private readonly IBOReportService _boReportService;
        private readonly string _baseUrl;
        private readonly string _defaultFolderId;
        private readonly string _baseUrlWithHostName;
        private readonly StringBuilder _logMessages;
        private readonly IActiveAnalyticsLogger _logger;
        private readonly BOAuthentication _boAuthmodel;

        #endregion

        #region -- Constructors --

        public WebiReportsController(IBOReportService boReportService,IActiveAnalyticsLogger logger)
        {
            _boReportService = boReportService;
            _baseUrl = ConfigurationManager.AppSettings["BOBaseUrl"];
            _defaultFolderId = ConfigurationManager.AppSettings["BIRepositoryDefaultReportsFolderId"];
            _baseUrlWithHostName = ConfigurationManager.AppSettings["BOBaseUrlWithHostName"];
            _logger = logger;
            _logMessages = new StringBuilder();
            _boAuthmodel = new BOAuthentication()
            {
                UserName = string.Empty,
                Password = string.Empty,
                URI = _baseUrl,
                LogonToken = string.Empty,
                BOSesssionID = string.Empty,
                UriWithHostName = _baseUrlWithHostName
            };
        }

        #endregion

        #region -- Api Methods --

        [HttpGet]
        public ReportCategory GetReportList(string sapToken)
        {
            _boAuthmodel.LogonToken = sapToken;            

            _logMessages.AppendFormat("Getting report list with base Uri {0}, UrlwithHostname {1} and Token {2}.",_baseUrl,_baseUrlWithHostName,sapToken);
            ReportCategory reportCategory = null;
            try
            {
                _logMessages.AppendFormat("Invoking BO Report service to get report categories with default folder id {0}.",_defaultFolderId);
                reportCategory = _boReportService.GetReportList(_boAuthmodel, _defaultFolderId);
            }
            catch (System.Exception ex)
            {
                _logMessages.AppendFormat("An error occurred. Exception information {0}.",ex.Message);
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            _logger.Info(_logMessages.ToString());
            return reportCategory;
        }

        [Route("api/webireports/{id}")]
        [HttpGet]
        public string GetReport(string sapToken, string sessionId, string id)
        {
            _boAuthmodel.LogonToken = sapToken;
            _boAuthmodel.BOSesssionID = sessionId;
            
            _logMessages.AppendFormat("Getting open document Uri with Token - {0}, SessionId - {1}, ReportId - {2}.",_boAuthmodel.LogonToken,_boAuthmodel.BOSesssionID,id);
            string reportHtml = string.Empty;
            try
            {
                reportHtml = _boReportService.GetReport(_boAuthmodel, id);
                _logMessages.AppendFormat("Received open document Url {0}.",reportHtml);
            }
            catch (System.Exception ex)
            {
                _logMessages.AppendFormat("Error occurred getting opendocument url. Exception message {0}.",ex.Message);
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            _logger.Info(_logMessages.ToString());
            return reportHtml;
        } 

        #endregion
    }
}
