using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Model
{
    public class BOUserSessionInfo
    {
        public string DefaultToken { get; set; }

        public bool MustChangePassword { get; set; }

        public string SerializedSession { get; set; }

        public string SessionId { get; set; }

        public string UserCUID { get; set; }

    }
}
