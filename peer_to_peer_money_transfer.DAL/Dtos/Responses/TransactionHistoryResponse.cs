using peer_to_peer_money_transfer.DAL.Enums;

namespace peer_to_peer_money_transfer.DAL.Dtos.Responses
{
    public class TransactionHistoryResponse
    {
        public TransactionType TransactionType { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }
    }
}
