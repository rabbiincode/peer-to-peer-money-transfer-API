using System.ComponentModel.DataAnnotations;

namespace peer_to_peer_money_transfer.DAL.Dtos.Requests
{
    public class WithdrawalRequest
    {
        [Required]
        public string CashMingleAccountNumber { get; set; }

        [Required]
        public string BankAccountNumber { get; set; }

        [Required]
        public string BankName { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
