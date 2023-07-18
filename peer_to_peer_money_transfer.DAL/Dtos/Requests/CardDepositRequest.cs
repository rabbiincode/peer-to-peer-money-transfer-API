using System.ComponentModel.DataAnnotations;

namespace peer_to_peer_money_transfer.DAL.Dtos.Requests
{
    public class CardDepositRequest
    {
        [Required]
        public string CashMingleAccountNumber { get; set; }

        [Required]
        public int Cvc { get; set; }

        [Required]
        public string CardName { get; set; }

        [Required]
        public long CardNumber { get; set; }

        [Required]
        public string CardExpiryDate { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
