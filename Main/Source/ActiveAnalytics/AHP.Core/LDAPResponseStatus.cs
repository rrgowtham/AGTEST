using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core
{
    public enum LDAPErrors
    {
        ERROR_INVALID_PASSWORD = 0x56,
        ERROR_PASSWORD_RESTRICTION = 0x52D,
        ERROR_LOGON_FAILURE = 0x52e,
        ERROR_ACCOUNT_RESTRICTION = 0x52f,
        ERROR_INVALID_LOGON_HOURS = 0x530,
        ERROR_INVALID_WORKSTATION = 0x531,
        ERROR_PASSWORD_EXPIRED = 0x532,
        ERROR_ACCOUNT_DISABLED = 0x533,
        ERROR_ACCOUNT_EXPIRED = 0x701,
        ERROR_PASSWORD_MUST_CHANGE = 0x773,
        ERROR_ACCOUNT_LOCKED_OUT = 0x775,
        ERROR_ENTRY_EXISTS = 0x2071,
    }
}
