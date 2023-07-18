using System.ComponentModel.DataAnnotations;

namespace peer_to_peer_money_transfer.Shared.DataTransferObject
{
    public class RegisterBusinessDTO : RegisterAdminDTO
    {
        [DataType(DataType.EmailAddress)]
        public string? RecoveryMail { get; set; }

        [Required(ErrorMessage = "Business Name is Required")]
        public string? BusinessName { get; set; }

        [Required(ErrorMessage = "Business Type is Required")]
        public string? BusinessType { get; set; }

        public string? BVN { get; set; } = null!;

        public string CAC { get; set; }
    }
}
