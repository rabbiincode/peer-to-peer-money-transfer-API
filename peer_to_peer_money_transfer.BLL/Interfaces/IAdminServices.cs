using Microsoft.AspNetCore.JsonPatch;
using peer_to_peer_money_transfer.DAL.Entities;
using peer_to_peer_money_transfer.Shared.DataTransferObject;

namespace peer_to_peer_money_transfer.BLL.Interfaces
{
    public interface IAdminServices
    {
        Task<IEnumerable<ApplicationUser>> GetAll();

        Task<IEnumerable<GetCharacterDTO>> GetAllCustomers();

        Task<GetCharacterDTO> GetCustomerByUserId(string userId);

        Task<IEnumerable<GetCharacterDTO>> GetAllCustomersByCategory(string userType);

        Task<GetCharacterDTO> GetCustomerByEmailAddressOrUserName(string EmailAddressOrUserName);

        Task<ApplicationUser> GetCustomerByEmailAddressOrUserNameAll(string EmailAddressOrUserName);

        Task<GetCharacterDTO> GetCustomerByAccountNumber(string accountNumber);

        Task<IEnumerable<TransactionHistory>> GetAllTransactions();

        Task<TransactionHistory> GetTransactionById(long Id);

        Task<ApplicationUser> EditCustomerDetails(string userName, JsonPatchDocument<ApplicationUser> user);

        Task<string> DeactivateCustomer(string userName);

        Task AccessFailedCount(string userName);

        Task ResetCount(string userName);

        Task LockCustomer(string userName);

        Task<string> SoftDelete(string userName); //soft delete

        Task<string> DeleteCustomer(string userName); //hard delete
    }
}
