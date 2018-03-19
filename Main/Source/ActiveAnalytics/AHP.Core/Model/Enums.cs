using System.ComponentModel;
using System.Reflection;
using System;

namespace AHP.Core.Model
{
    /*
     *  Enums are declared as bytes as they are saved as tiny int
     */

    public enum AccountIssues
    {
        [Description("Account Locked")]
        AccountLocked = 1,
        [Description("Report An Error Message")]
        ReportAnErrorMessage = 2,
        [Description("Question About Data Shown")]
        QuestionAboutDataShown = 3,
        [Description("Technical Question")]
        TechnicalQuestion = 4,
        [Description("Forgot Username")]
        ForgotUsername = 5,
        [Description("Reset Password")]
        ResetPassword = 6,
        [Description("Other")]
        Other = 7
    }

    public enum LDAPAccessStatus
    {
        UserLogonSuccessful = 1,
        UserLogonUnsuccessful =2,
        UserAccountLocked = 3
    }
  

}