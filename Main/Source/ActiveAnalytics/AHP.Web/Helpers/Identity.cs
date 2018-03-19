using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AHP.Web.Helpers
{
    public sealed class Identity
    {
        public string UserName { get; set; }

        public bool IsInternalUser { get; set; }

        public string SapToken { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DisplayName { get; set; }

        public string Company { get; set; }

        public string Role { get; set; }

        public string Email { get; set; }

        public string BOSessionId { get; set; }

        public string BOSerializedSessionId { get; set; }

        public bool MustChangePassword { get; set; }

        public bool MustSelectSecurityQuestions { get; set; }

        public string LastLogonDate { get; set; }

        public string TableauAuthTicket { get; set; }

        public bool PasswordExpired { get; set; }

        public Identity(string userName, bool isInternalUser)
            : this(userName, isInternalUser, string.Empty, string.Empty, string.Empty)
        {

        }

        public Identity(string userName, bool isInternalUser, string sapToken, string sessionId,string serializedSessionId)
            : this(userName, isInternalUser, sapToken, "", "", "", sessionId,serializedSessionId)
        {

        }

        public Identity(string userName, bool isInternalUser, string sapToken, string firstName, string lastName, string displayName, string boSessionId,string boSerializedSessionId)
        {
            this.UserName = userName;
            this.IsInternalUser = isInternalUser;
            this.SapToken = sapToken;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DisplayName = displayName;
            this.BOSessionId = boSessionId;
            this.BOSerializedSessionId = boSerializedSessionId;
        }
    }
}