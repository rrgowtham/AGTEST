
#region -- File Headers --

/*
 * Author: James Deepak
 * Date: May 5 2017
 * Purpose: Delivers emails through a SMTP server
 * 
 */

#endregion

#region -- Using Directives --
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHP.Core.Model;
using System.Net.Mail;
using AHP.Core.Logger;
using System.Configuration;
using System.IO; 
#endregion


namespace AHP.Core.Service
{
    public class EmailDeliveryService : IEmailSenderService
    {
        #region -- Members --
        private readonly IActiveAnalyticsLogger _logger;
        #endregion

        #region -- Constructors --
        public EmailDeliveryService(IActiveAnalyticsLogger logger)
        {
            _logger = logger;
        }
        #endregion

        #region -- IEmailSenderService Methods --

        public bool SendEmail(Customer supportRequest, string emailTemplate)
        {
            bool mailStatus = false;
            try
            {
                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = ConfigurationManager.AppSettings["smtpServer"];
                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.IsBodyHtml = true;
                        mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["EmailFROM"]);
                        string[] To = ConfigurationManager.AppSettings["EmailTO"].ToString().Split(';');
                        foreach (var emailAddress in To)
                        {
                            mailMessage.To.Add(new MailAddress(emailAddress));
                        }
                        string[] cc = ConfigurationManager.AppSettings["EmailCC"].ToString().Split(';');
                        foreach (var emailAddress in cc)
                        {
                            mailMessage.CC.Add(new MailAddress(emailAddress));
                        }
                        mailMessage.Subject = ConfigurationManager.AppSettings["EmailSUBJECT"];
                        mailMessage.SubjectEncoding = Encoding.UTF8;
                        string body = emailTemplate;

                        mailMessage.Body = string.Format(body,
                                                    supportRequest.SelectedIssue, supportRequest.FirstName + " " + supportRequest.LastName,
                                                    supportRequest.Email, supportRequest.Company, supportRequest.PhoneNumber, supportRequest.IssueDescription);
                        smtp.Send(mailMessage);
                    }
                }
                mailStatus = true;
                _logger.Info("Email sent successfully. For customer support request.");
            }
            catch (Exception ex)
            {
                _logger.Info("Error sending email. Exception message " + ex.Message + ".");
                mailStatus = false;
            }

            return mailStatus;
        }

        public bool SendEmail(string from, string[] to, string[] cc, string subject, string body)
        {
            bool mailStatus = false;
            try
            {
                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = ConfigurationManager.AppSettings["smtpServer"];
                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.IsBodyHtml = true;
                        mailMessage.From = new MailAddress(from);
                        foreach (var emailAddress in to)
                        {
                            mailMessage.To.Add(new MailAddress(emailAddress));
                        }
                        foreach (var emailAddress in cc)
                        {
                            mailMessage.CC.Add(new MailAddress(emailAddress));
                        }
                        mailMessage.Subject = subject;
                        mailMessage.SubjectEncoding = Encoding.UTF8;
                        mailMessage.Body = body;
                        mailMessage.IsBodyHtml = true;
                        smtp.Send(mailMessage);
                    }
                }
                mailStatus = true;
                _logger.Info("Email sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.Info("Error sending email. Exception message " + ex.Message + ".");
                mailStatus = false;
            }

            return mailStatus;
        } 
        #endregion
    }
}
