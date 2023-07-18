using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using peer_to_peer_money_transfer.BLL.Interfaces;
using peer_to_peer_money_transfer.DAL.Entities;
using peer_to_peer_money_transfer.DAL.Enums;
using peer_to_peer_money_transfer.DAL.Interfaces;
using peer_to_peer_money_transfer.Shared.DataTransferObject;
using System.Text.RegularExpressions;

namespace peer_to_peer_money_transfer.BLL.Implementation
{
    public class AdminServices : IAdminServices
    {
        string pattern = "^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\\.[a-zA-Z0-9-]+)*$";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepository<ApplicationUser> _userRepoService;
        private readonly IRepository<TransactionHistory> _transactionHistoryRepo;

        public AdminServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userRepoService = _unitOfWork.GetRepository<ApplicationUser>();
            _transactionHistoryRepo = _unitOfWork.GetRepository<TransactionHistory>();
        }

        public async Task<IEnumerable<ApplicationUser>> GetAll()
        {
            return await _userRepoService.GetAllAsync();
        }

        public async Task<IEnumerable<GetCharacterDTO>> GetAllCustomers()
        {
            var allUser = await _userRepoService.GetAllAsync();
            var select = _mapper.Map<IEnumerable<GetCharacterDTO>>(allUser);
            return select;
        }

        public async Task<GetCharacterDTO> GetCustomerByUserId(string userId)
        {
            var user = await _userRepoService.GetSingleByAsync(x => x.Id == userId);
            var select = _mapper.Map<GetCharacterDTO>(user);
            return select;
        }

        public async Task<IEnumerable<GetCharacterDTO>> GetAllCustomersByCategory(string userType)
        {
            var category = await _userRepoService.GetByAsync(x => x.UserType == userType);
            var select = _mapper.Map<IEnumerable<GetCharacterDTO>>(category);
            return select;
        }

        public async Task<ApplicationUser> GetCustomerByEmailAddressOrUserNameAll(string EmailAddressOrUserName)
        {
            Regex regex = new(pattern);
            var isEmail = regex.IsMatch(EmailAddressOrUserName);
            var user = isEmail == true ? await _userRepoService.GetSingleByAsync(x => x.Email == EmailAddressOrUserName)
                       : await _userRepoService.GetSingleByAsync(x => x.UserName == EmailAddressOrUserName);
            return user;
        }

        public async Task<GetCharacterDTO> GetCustomerByEmailAddressOrUserName(string EmailAddressOrUserName)
        {
            Regex regex = new(pattern);
            var isEmail = regex.IsMatch(EmailAddressOrUserName);
            var user = isEmail == true ? await _userRepoService.GetSingleByAsync(x => x.Email == EmailAddressOrUserName)
                       : await _userRepoService.GetSingleByAsync(x => x.UserName == EmailAddressOrUserName);
            var select = _mapper.Map<GetCharacterDTO>(user);
            return select;
        }

        public async Task<GetCharacterDTO> GetCustomerByAccountNumber(string accountNumber)
        {
            var number = await _userRepoService.GetSingleByAsync(x => x.AccountNumber == accountNumber);
            var select = _mapper.Map<GetCharacterDTO>(number);
            return select;
        }

        public async Task<IEnumerable<TransactionHistory>> GetAllTransactions()
        {
            return await _transactionHistoryRepo.GetAllAsync();
        }

        public async Task<TransactionHistory> GetTransactionById(long Id)
        {
            return await _transactionHistoryRepo.GetByIdAsync(Id);
        }

        public async Task<ApplicationUser> EditCustomerDetails(string userName, JsonPatchDocument<ApplicationUser> user)
        {
            var update = await _userRepoService.GetSingleByAsync(x => x.UserName == userName);
            if (update == null) return null;
            user.ApplyTo(update);
            return await _userRepoService.UpdateAsync(update);
        }

        public async Task<string> DeactivateCustomer(string userName)
        {
            var deactivateUser = await _userRepoService.GetSingleByAsync(x => x.UserName == userName);
            if (deactivateUser == null) return null;
            deactivateUser.Activated = false;
            await _userRepoService.UpdateAsync(deactivateUser);
            return "success";
        }

        public async Task AccessFailedCount(string userName)
        {
            var count = await _userRepoService.GetSingleByAsync(x => x.UserName == userName);
            count.AccessFailedCount += 1;
            await _userRepoService.UpdateAsync(count);
        }

        public async Task ResetCount(string userName)
        {
            var resetCount = await _userRepoService.GetSingleByAsync(x => x.UserName == userName);
            resetCount.AccessFailedCount = 0;
            await _userRepoService.UpdateAsync(resetCount);
        }

        public async Task LockCustomer(string userName)
        {
            var lockedUser = await _userRepoService.GetSingleByAsync(x => x.UserName == userName);
            lockedUser.Activated = false;
            lockedUser.EmailConfirmed = false;
            await _userRepoService.UpdateAsync(lockedUser);
        }

        public async Task<string> SoftDelete(string userName)
        {
            var deleteUser = await _userRepoService.GetSingleByAsync(x => x.UserName == userName);
            if (deleteUser == null) return null;
            deleteUser.Deleted = true;
            await _userRepoService.UpdateAsync(deleteUser);
            return "success";
        }

        public async Task<string> DeleteCustomer(string userName)
        {
            var delete = await _userRepoService.GetSingleByAsync(x => x.UserName == userName);
            if (delete == null) return null;
            await _userRepoService.DeleteByIdAsync(delete.Id);
            return "success";
        }
    }
}
