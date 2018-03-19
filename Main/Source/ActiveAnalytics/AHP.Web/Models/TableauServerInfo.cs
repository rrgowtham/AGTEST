using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AHP.Web.Models
{
    public class TableauServerInfo
    {

        public string ServerUrl { get; set; }

        public string ApiVersion { get; set; }

        public bool EnableProxy { get; set; }

        public const string Proxy = "proxy.ahm.corp";

        public const int ProxyPort = 9119;

    }
}