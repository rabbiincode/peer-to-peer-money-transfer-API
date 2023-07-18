using System.ComponentModel.DataAnnotations;

namespace peer_to_peer_money_transfer.Shared.DataTransferObject
{
    public class RegisterIndividualDTO : RegisterAdminDTO
    {

        [DataType(DataType.EmailAddress)]
        public string? RecoveryMail { get; set; }

        [Required(ErrorMessage = "Your Profession is Required")]
        [Display(Name = "Profession")]
        public string Profession { get; set; }

        public string? BVN { get; set; }
    }
}
