using System.ComponentModel.DataAnnotations;

namespace AHP.Web.ViewModel
{
    public class AccountRecoveryInfoViewModel
    {        
        public bool DoesUsernameExists { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessage = "Username required")]
        public string Username { get; set; }

        public string PrimaryQuestion { get; set; }

        public string PrimaryAnswer { get; set; }

        public string SecondaryQuestion { get; set; }

        public string SecondaryAnswer { get; set; }

    }
}