using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PayStack.Net;
using peer_to_peer_money_transfer.BLL.Interfaces;
using peer_to_peer_money_transfer.DAL.Dtos.Requests;
using peer_to_peer_money_transfer.DAL.Entities;
using peer_to_peer_money_transfer.DAL.Enums;
using peer_to_peer_money_transfer.DAL.Interfaces;
using Response = peer_to_peer_money_transfer.DAL.Dtos.Responses.Response;

namespace peer_to_peer_money_transfer.BLL.Implementation
{
    public class FundingServices : IFundingService
    {
        static IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ApplicationUser> _userRepoService;
        private readonly IRepository<TransactionHistory> _transactionHistoryRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FundingServices(IConfiguration configuration, UserManager<ApplicationUser> userManager, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userRepoService = _unitOfWork.GetRepository<ApplicationUser>();
            _transactionHistoryRepo = _unitOfWork.GetRepository<TransactionHistory>();
        }

        public async Task<Response> FundAccount(string currentUserId, string reference)
        {
           var verification = VerifyPayment(reference);

           var  user = await _userRepoService.GetByIdAsync(currentUserId);
            if (user == null)
            {
                throw new InvalidOperationException("User Not Found");
            }
            if (!user.Activated)
            {
                throw new InvalidOperationException("User has been deactivated");
            }
            if (!user.Verified)
            {
                throw new InvalidOperationException("Account Not Verifed, Verify account to continue");
            }
            if (!verification.Status)
            {
                throw new InvalidOperationException("Payment Wasn't successful");
            }
            var amount = verification.Data.Amount / 100 - (int)verification.Data.Fees;
            user.Balance += amount;
            await _unitOfWork.SaveChangesAsync();
            return new Response { Success = true, Data = $"Account funded with {amount} Account balance is {user.Balance}" };
        }

        public TransactionInitializeResponse MakePayment(DepositRequest depositRequest)
        {
            string secret = (string)_configuration.GetSection("ApiSecret").GetSection("SecretKey").Value;
            PayStackApi payStack = new(secret);
            TransactionInitializeRequest initializeRequest = _mapper.Map<TransactionInitializeRequest>(depositRequest);
            var result = payStack.Transactions.Initialize(initializeRequest);
            return result;
        }  

        public TransactionVerifyResponse VerifyPayment(string referenceCode)
        {
            string secret = (string)_configuration.GetSection("ApiSecret").GetSection("SecretKey").Value;
            PayStackApi payStack = new(secret);
            TransactionVerifyResponse result = payStack.Transactions.Verify(referenceCode);
            return result;
          
        }

        public async Task<Response> CardDepositAsync(CardDepositRequest deposit)
        {
            var user = await _userRepoService.GetSingleByAsync(x => x.AccountNumber == deposit.CashMingleAccountNumber);
            if (user == null) return new Response { Success = false, Data = $"User not found" };

            user.Balance += deposit.Amount;
            await _userRepoService.UpdateAsync(user);

            var transactionHistory = new TransactionHistory
            {
                UserId = user.Id,
                TransactionType = TransactionTypeExtension.GetStringValue(TransactionType.Deposit),
                DateStamp = DateTime.Now,
                Amount = deposit.Amount,
                Description = $"${deposit.Amount} deposited on {DateTime.Now.ToString("D")} by {DateTime.Now.ToString("t")}"
            };

            await _transactionHistoryRepo.AddAsync(transactionHistory);
            var check = await _unitOfWork.SaveChangesAsync();

            return check == 0 ? new Response { Success = true, Data = $"Deposit Successful" } : new Response { Success = false, Data = $"Deposit Failed...please try again" };
        }

        public async Task<Response> WithdrawAsync(WithdrawalRequest withdraw)
        {
            var user = await _userRepoService.GetSingleByAsync(x => x.AccountNumber == withdraw.CashMingleAccountNumber);
            var validatePassword = await _userManager.CheckPasswordAsync(user, withdraw.Password);

            if (user == null) return new Response { Success = false, Data = $"User not found" };
            if (!validatePassword) return new Response { Success = false, Data = $"Incorrect Password" };
            if (user.Balance <= withdraw.Amount) return new Response { Success = false, Data = $"Insufficient Fund" };

            user.Balance -= withdraw.Amount;
            await _userRepoService.UpdateAsync(user);

            var transactionHistory = new TransactionHistory
            {
                UserId = user.Id,
                TransactionType = TransactionTypeExtension.GetStringValue(TransactionType.Withdrawal),
                DateStamp = DateTime.Now,
                Amount = withdraw.Amount,
                Description = $"${withdraw.Amount} withdrawn on {DateTime.Now.ToString("D")} by {DateTime.Now.ToString("t")} to account number {withdraw.BankAccountNumber}, {withdraw.BankName}"
            };

            await _transactionHistoryRepo.AddAsync(transactionHistory);
            var check = await _unitOfWork.SaveChangesAsync();

            return check == 0 ? new Response { Success = true, Data = $"Withdrawal Successful" } : new Response { Success = false, Data = $"Withdrawal Failed...please try again" };
        }

        public async Task<Response> PurchaseAirtimeAsync(PurchaseAirtime airtime)
        {
            var user = await _userRepoService.GetSingleByAsync(x => x.AccountNumber == airtime.CashMingleAccountNumber);
            var validatePassword = await _userManager.CheckPasswordAsync(user, airtime.Password);

            if (user == null) return new Response { Success = false, Data = $"User not found" };
            if (!validatePassword) return new Response { Success = false, Data = $"Incorrect Password" };
            if (user.Balance <= airtime.Amount) return new Response { Success = false, Data = $"Insufficient Fund" };

            user.Balance -= airtime.Amount;
            await _userRepoService.UpdateAsync(user);

            var transactionHistory = new TransactionHistory
            {
                UserId = user.Id,
                TransactionType = TransactionTypeExtension.GetStringValue(TransactionType.Airtime),
                DateStamp = DateTime.Now,
                Amount = airtime.Amount,
                Description = $"Recharged {airtime.PhoneNumber} with ${airtime.Amount} on {DateTime.Now.ToString("D")} by {DateTime.Now.ToString("t")}"
            };

            await _transactionHistoryRepo.AddAsync(transactionHistory);
            var check = await _unitOfWork.SaveChangesAsync();

            return check == 0 ? new Response { Success = true, Data = $"Airtime Purchase Successful" } : new Response { Success = false, Data = $"Airtime Purchase Failed...please try again" };
        }
    }
}
