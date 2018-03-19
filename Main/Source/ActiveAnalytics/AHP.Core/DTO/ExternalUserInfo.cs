using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.DTO
{
    public class ExternalUserInfo
    {
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Role { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public string SupplierId { get; set; }

        public bool IsLocked { get; set; }

        public bool ChangePasswordOnLogon { get; set; }

        public bool IsEmailActivated { get; set; }

        public string Company { get; set; }

        public bool IsActive { get; set; }

        public int InvalidLogonAttemptCount { get; set; }

        public short BirthMonth { get; set; }

        public short BirthYear { get; set; }

        public string ZipCode { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedDate { get; set; }

        public string LastLogonDate { get; set; }

        public DateTime PasswordExpiresOn { get; set; }
    }
}
