using AHP.Core.Logger;
using AHP.Core.Model;
using AHP.Core.Service;
using System.IO;
using System.Text;

namespace AHP.Web.Api.Controllers
{
    public class NeedHelpController : ApiBaseController
    {
        #region -- Members --

        private readonly IActiveAnalyticsLogger _logger;

        private readonly StringBuilder _logMessages;

        private readonly IEmailSenderService _emailService;

        #endregion

        #region -- Constructors --

        public NeedHelpController(IActiveAnalyticsLogger logger,IEmailSenderService emailService)
        {
            _logger = logger;
            _logMessages = new StringBuilder();
            _emailService = emailService;
        }

        #endregion

        #region -- Api Methods --

        [System.Web.Mvc.HttpPost]
        public bool PostNeedHelpMessage(Customer customer)
        {
            bool response = false;

            try
            {
                string template;
                using (var emailTemplateReader = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/EmailTemplates/CustomerIssue.html")))
                {
                    template = emailTemplateReader.ReadToEnd();
                }
                _logMessages.AppendFormat("Requesting need help message for user. Email {0}, FirstName {1}, Selected Issue {2}, Issue Description {3}",customer.Email,customer.FirstName,customer.SelectedIssue,customer.IssueDescription);
                response = _emailService.SendEmail(customer, template);
                _logMessages.Append("Email request send successfully");
            }
            catch (System.Exception ex)
            {
                _logMessages.AppendFormat("Error occurred sending email. Exception info {0}",ex.Message);
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            _logger.Info(_logMessages.ToString());
            return response;
        } 

        #endregion
    }
}
