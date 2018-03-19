using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.DTO
{
    public class PasswordResetResponse
    {
        public string NewPassword { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

    }
}
