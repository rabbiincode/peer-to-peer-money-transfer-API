using peer_to_peer_money_transfer.DAL.Dtos.Requests;
using peer_to_peer_money_transfer.DAL.Entities;
using peer_to_peer_money_transfer.BLL.Models;
using peer_to_peer_money_transfer.DAL.Enums;
using Response = peer_to_peer_money_transfer.DAL.Dtos.Responses.Response;

namespace peer_to_peer_money_transfer.BLL.Interfaces
{
    public interface ITransactionsServices
    {
        Task<Response> TransferMoneyAsync(TransferRequest transferRequest);

        Task<Response> SetTransferAsync(TransactionModel transactionModel);

        Task<IEnumerable<TransactionHistory>> GetTransactionHistoriesAsync(string accountNumber);

        Task<TransactionHistory> GetTransactionDetailsAsync(long transactionId);

        decimal GetTranscationFee(string userType, decimal Amount);
    }
}
