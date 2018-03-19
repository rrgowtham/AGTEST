using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core
{
    public class ClaimTypes
    {
        public const string DisplayName = "DisplayName";

        public const string IsInternalUser = "IsInternalUser";

        public const string LogonToken = "LogonToken";

        public const string BOSessionId = "BOSessionId";

        public const string BOSerializedSession = "BOSerializedSessionId";

        public const string MustChangePassword = "ChangePassword";

        public const string MustChangeSecurityQuestion = "ChangeSecurityQuestion";

        public const string Company = "Company";

        public const string LastLogonDate = "LastLogonDate";

        public const string TableauAuthTicket = "TableauTicket";

        public const string PasswordExpired = "PasswordExpired";
    }
}
