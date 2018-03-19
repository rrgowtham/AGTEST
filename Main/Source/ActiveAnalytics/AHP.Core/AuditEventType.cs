using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core
{
    public static class AuditEventType
    {

        public const string CHANGE_PASSWORD = "CHANGE-PASSWORD";

        public const string ANSWER_SECURITY_QUESTION = "ANSWER-QUESTIONS";

        public const string RESET_PASSWORD = "RESET-PASSWORD";

        public const string LOGIN_SUCCESS = "LOGIN-SUCCESS";

        public const string LOGIN_ATTEMPT = "LOGIN-ATTEMPT";

        public const string INVALID_LOGIN = "INVALID-LOGON-ATTEMPT";
        
    }
}
