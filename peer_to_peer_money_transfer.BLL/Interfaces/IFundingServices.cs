using PayStack.Net;
using peer_to_peer_money_transfer.DAL.Dtos.Requests;
using Response = peer_to_peer_money_transfer.DAL.Dtos.Responses.Response;

namespace peer_to_peer_money_transfer.BLL.Interfaces
{
    public interface IFundingService
    {
        TransactionInitializeResponse MakePayment(DepositRequest depositRequest);

        TransactionVerifyResponse VerifyPayment(string referenceCode);

        Task<Response> FundAccount(string currentUserId, string reference);

        Task<Response> CardDepositAsync(CardDepositRequest deposit);

        Task<Response> WithdrawAsync(WithdrawalRequest withdraw);

        Task<Response> PurchaseAirtimeAsync(PurchaseAirtime airtime);
    }
}
