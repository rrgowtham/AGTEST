using AHP.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Service
{
    /// <summary>
    /// Sends emails to recipients through a smtp server
    /// </summary>
    public interface IEmailSenderService
    {
        /// <summary>
        /// Sends a customer support request
        /// </summary>
        /// <param name="supportRequest">Instance of <see cref="Customer"/> having customer support request</param>
        /// <param name="emailTemplate">email template containing information about user</param>
        /// <returns>True when email has been sent successfully, False otherwise</returns>
        bool SendEmail(Customer supportRequest, string emailTemplate);

        /// <summary>
        /// Sends an email through SMTP server
        /// </summary>
        /// <param name="from">Email address of the sender</param>
        /// <param name="to">Email address of the recipient</param>
        /// <param name="cc">Email address of copied recipient</param>
        /// <param name="subject">Subject of the email</param>
        /// <param name="body">Content or body of the email</param>
        /// <returns>True when email has been sent successfully, False otherwise</returns>
        bool SendEmail(string from,string[] to,string[] cc,string subject,string body);
    }
}
