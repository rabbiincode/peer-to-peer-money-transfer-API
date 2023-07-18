using System.ComponentModel.DataAnnotations;

namespace peer_to_peer_money_transfer.BLL.Models
{
    public class Login
    {
        [Required(ErrorMessage = "UserName name Must be Unique")]
        public string EmailAddressOrUserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(20, ErrorMessage = "Your Password is limited to {2} to {1} characters", MinimumLength = 5)]
        public string Password { get; set; }
    }
}
