using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Model
{
    public class BOAuthentication
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserAuth { get; set; }
        public string LogonToken { get; set; }
        public string URI { get; set; }
        public string NameSpace { get; private set; }
        public int StatusCode { get; set; }
        public string UriWithHostName { get; set; }

        public bool MustChangePassword { get; set; }

        public string BOSesssionID { get; set; }

        public string BOSerializedSessionId { get; set; }

        public BOAuthentication()
        {
            NameSpace = "http://www.sap.com/rws/bip";
        }

    }
}
