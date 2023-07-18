using System.ComponentModel.DataAnnotations;

namespace peer_to_peer_money_transfer.DAL.Dtos.Requests
{
    public class PurchaseAirtime
    {
        [Required]
        public string CashMingleAccountNumber { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string Password { get; set; }
    }
}
