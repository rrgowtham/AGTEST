using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Model
{
    [System.ComponentModel.DataAnnotations.Schema.Table("USERINFO")]
    public class UserInfo
    {

        [Column("USERNAME")]
        [System.ComponentModel.DataAnnotations.MaxLength(128)]
        [System.ComponentModel.DataAnnotations.Key]
        public string Username { get; set; }

        [Column("EMAIL")]
        [System.ComponentModel.DataAnnotations.MaxLength(128)]
        public string Email { get; set; }

        [Column("ISEMAILACTIVATED")]
        [System.ComponentModel.DataAnnotations.MaxLength(1)]
        [System.ComponentModel.DefaultValue("N")]
        public string IsEmailActivated { get; set; }

        [Column("FIRSTNAME")]
        [System.ComponentModel.DataAnnotations.MaxLength(128)]
        public string FirstName { get; set; }

        [Column("LASTNAME")]
        [System.ComponentModel.DataAnnotations.MaxLength(128)]
        public string LastName { get; set; }

        [Column("PASSWORD")]
        [System.ComponentModel.DataAnnotations.MaxLength(256)]
        public string Password { get; set; }

        [Column("SALT")]
        [System.ComponentModel.DataAnnotations.MaxLength(256)]
        public string Salt { get; set; }

        [Column("ROLE")]
        [System.ComponentModel.DataAnnotations.MaxLength(50)]
        public string Role { get; set; }

        [Column("SUPPLIERID")]
        [System.ComponentModel.DataAnnotations.MaxLength(1000)]
        public string SupplierIds { get; set; }

        [Column("ISACTIVE")]
        [System.ComponentModel.DataAnnotations.MaxLength(1)]
        [System.ComponentModel.DefaultValue("N")]
        public string IsUserActive { get; set; }

        [Column("CHANGEPWDONLOGON")]
        [System.ComponentModel.DataAnnotations.MaxLength(1)]
        [System.ComponentModel.DefaultValue("N")]
        public string ChangePasswordOnLogon { get; set; }

        [Column("ISLOCKED")]
        [System.ComponentModel.DataAnnotations.MaxLength(1)]
        [System.ComponentModel.DefaultValue("N")]
        public string IsLocked { get; set; }

        [Column("INVALIDLOGONATTEMPT")]
        [System.ComponentModel.DefaultValue(0)]
        public int InvalidLogonAttemptCount { get; set; }

        [Column("COMPANY")]
        [System.ComponentModel.DataAnnotations.MaxLength(100)]
        public string CompanyName { get; set; }

        [Column("ZIPCODE")]
        [System.ComponentModel.DataAnnotations.MaxLength(25)]
        public string ZipCode { get; set; }

        [Column("MONTHYEAR")]
        [System.ComponentModel.DataAnnotations.MaxLength(7)]
        public string MonthYear { get; set; }

        [Column("CREATEDDATE")]
        public DateTime? CreatedDate { get; set; }

        [Column("CREATEDBY")]
        [System.ComponentModel.DataAnnotations.MaxLength(128)]
        public string CreatedBy { get; set; }

        [Column("LASTLOGONDATE")]
        public DateTime? LastLoginDate { get; set; }

        [Column("PWDEXPIRESON")]
        public DateTime PasswordExpiresOn { get; set; }

    }
}
