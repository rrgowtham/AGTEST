using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AHP.Core.Model
{
    public class TrustedLogonResponse
    {

        public string logonToken { get; set; }

        public string error_code { get; set; }

        public string message { get; set; }

    }
}