using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Model
{
    public class UserSetupInfo : IErrorProvider
    {
        public bool IsEmailPresent { get; set; }

        public bool AreSecurityquestionsPresent { get; set; }

        public bool Success
        {
            get;set;            
        }

        public List<string> Errors
        {
            get;set;
        }

        public UserSetupInfo()
        {
            Success = false;
            Errors = new List<string>();
        }
    }
}
