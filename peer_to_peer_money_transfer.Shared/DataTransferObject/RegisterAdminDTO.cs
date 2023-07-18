using System.ComponentModel.DataAnnotations;

namespace peer_to_peer_money_transfer.Shared.DataTransferObject
{
    public class RegisterAdminDTO
    {
        [Required(ErrorMessage = "First Name is Required")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last Name is Required")]
        public string LastName { get; set; } = null!;

        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Email Address is Required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone No is Required")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "UserName name Must be Unique")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(20, ErrorMessage = "Your Password is limited to {2} to {1} characters", MinimumLength = 5)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Address is Required")]
        public string Address { get; set; } = null!;

        public string? NIN { get; set; }
    }
}
