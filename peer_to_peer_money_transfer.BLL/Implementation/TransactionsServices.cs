using Microsoft.AspNetCore.Http;
using peer_to_peer_money_transfer.BLL.Interfaces;
using peer_to_peer_money_transfer.BLL.Models;
using peer_to_peer_money_transfer.DAL.Dtos.Requests;
using Response = peer_to_peer_money_transfer.DAL.Dtos.Responses.Response;
using peer_to_peer_money_transfer.DAL.Entities;
using peer_to_peer_money_transfer.DAL.Enums;
using peer_to_peer_money_transfer.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace peer_to_peer_money_transfer.BLL.Implementation
{
    public class TransactionsServices : ITransactionsServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<TransactionHistory> _transactionHistoryRepo;
        private readonly IRepository<ApplicationUser> _userProfileRepo;
        //private readonly IHttpContextAccessor _contextAccessor;

        public TransactionsServices(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
           // _contextAccessor = contextAccessor;
            _transactionHistoryRepo = _unitOfWork.GetRepository<TransactionHistory>();
            _userProfileRepo = _unitOfWork.GetRepository<ApplicationUser>();
        }

        public async Task<IEnumerable<TransactionHistory>> GetTransactionHistoriesAsync(string accountNumber)
        {
            var user = await _userProfileRepo.GetSingleByAsync(a => a.AccountNumber == accountNumber);
            var userId = user.Id;
            var transactions = await _transactionHistoryRepo.GetByAsync(x => x.UserId == userId);

            //string? _userId = _contextAccessor.HttpContext?.User.GetUserId();
            //var Transactions = await _transactionHistoryRepo.GetByAsync(a => a.UserId == _userId);

            return transactions;
        }

        public async Task<TransactionHistory> GetTransactionDetailsAsync(long transactionId)
        {
            var transactions = await _transactionHistoryRepo.GetSingleByAsync(x => x.Id == transactionId);
            return transactions;
        }

        public decimal GetTranscationFee(string userType, decimal Amount)
        {
            switch (userType)
            {
                case "Corporate":
                    return CorporateTranscationFees(Amount);
                default:
                    return IndiviualTranscationFees(Amount);
            }
        }

        private static decimal IndiviualTranscationFees(decimal Amount)
        {
            if (Amount < 1000)
                return 4M;
            if (Amount < 5000)
                return 7.5M;
            if (Amount < 10000)
                return 12.5M;
            if (Amount < 20000)
                return 15;
            if (Amount < 30000)
                return 25;
            if (Amount < 50000)
                return 37.5M;
            if (Amount < 100000)
                return 60;
            if (Amount < 20000)
                return 80;
            if (Amount < 400000)
                return 100;
            if (Amount < 600000)
                return 125;
            if (Amount < 700000)
                return 135;
            if (Amount < 800000)
                return 150;
            if (Amount < 900000)
                return 170;
            return 185.5M;
        }

        private static decimal CorporateTranscationFees(decimal Amount)
        {
            if (Amount < 1000)
                return 15;
            if (Amount < 10000)
                return 25;
            if (Amount < 50000)
                return 35;
            if (Amount < 100000)
                return 45;
            if (Amount < 200000)
                return 55;
            if (Amount < 300000)
                return 75;
            if (Amount < 400000)
                return 85;
            if (Amount < 500000)
                return 95;
            if (Amount < 600000)
                return 105;
            if (Amount < 700000)
                return 115;
            if (Amount < 800000)
                return 125;
            if (Amount < 900000)
                return 135;
            if (Amount < 1000000)
                return 150;
            if (Amount < 2000000)
                return 300;
            if (Amount < 3000000)
                return 450;
            if (Amount < 4000000)
                return 600;
            if (Amount < 5000000)
                return 750;
            if (Amount < 6000000)
                return 900;
            if (Amount < 7000000)
                return 1050;
            if (Amount < 8000000)
                return 1200;
            if (Amount < 9000000)
                return 1350;
            return 1500;
        }

        public async Task<Response> SetTransferAsync(TransactionModel transactionModel)
        {
            decimal Fee = GetTranscationFee(transactionModel.UserType, transactionModel.Amount);

            transactionModel.Sender.Balance = transactionModel.Sender.Balance - transactionModel.Amount - Fee;
            transactionModel.Receiver.Balance += transactionModel.Amount;

            await _userProfileRepo.UpdateAsync(transactionModel.Sender);
            await _userProfileRepo.UpdateAsync(transactionModel.Receiver);

            var transactionHistory = new List<TransactionHistory>() {
                new TransactionHistory
                {
                    UserId = transactionModel.Sender.Id,
                    TransactionType = TransactionTypeExtension.GetStringValue(TransactionType.Transfer),
                    DateStamp = DateTime.Now,
                    Amount = transactionModel.Amount,
                    Description = $"${transactionModel.Amount} sent to {transactionModel.Receiver.FirstName} {transactionModel.Receiver.LastName} {transactionModel.Receiver.MiddleName} on {DateTime.Now.ToString("D")} by {DateTime.Now.ToString("t")}. {transactionModel.CustomerTransactionDescription}"
                },
                new TransactionHistory
                {
                    UserId = transactionModel.Sender.Id,
                    TransactionType = TransactionTypeExtension.GetStringValue(TransactionType.Debit),
                    DateStamp = DateTime.Now,
                    Amount = Fee,
                    Description = $"Transfer Charge"
                },
                new TransactionHistory
                {
                    UserId = transactionModel.Receiver.Id,
                    TransactionType = TransactionTypeExtension.GetStringValue(TransactionType.Credit),
                    DateStamp = DateTime.Now,
                    Amount = transactionModel.Amount,
                    Description = $"${transactionModel.Amount} credited from {transactionModel.Sender.FirstName} {transactionModel.Sender.LastName} {transactionModel.Sender.MiddleName} on {DateTime.Now.ToString("D")} by {DateTime.Now.ToString("t")}"
                } 
            };

            await _transactionHistoryRepo.AddRangeAsync(transactionHistory);
            var check = await _unitOfWork.SaveChangesAsync();
            return check == 0 ? new Response { Success = true, Data = $"Transfer successful" } : new Response { Success = false, Data = $"Transfer failed" };
        }

        public async Task<Response> TransferMoneyAsync(TransferRequest transferRequest)
        {
            var sender = await _userProfileRepo.GetSingleByAsync(a => a.AccountNumber == transferRequest.SenderAccountNumber);
            var receiver = await _userProfileRepo.GetSingleByAsync(a => a.AccountNumber == transferRequest.ReceiverAccountNumber);
            var validatePassword = await _userManager.CheckPasswordAsync(sender, transferRequest.SenderPassword);

            if (receiver == null)
            {
                return new Response { Success = false, Data = $"Account Number Not Found" };
            }
            if (!validatePassword)
            {
                return new Response { Success = false, Data = $"Incorrect Password" };
            }
            if (sender.Balance <= transferRequest.Amount)
            {
                return new Response { Success = false, Data = $"Insufficent Fund" };
            }
            if (transferRequest.Amount <= 0)
            {
                return new Response { Success = false, Data = $"Invalid Amount" };
            }
            if (sender.UserType == UserTypeExtension.GetStringValue(UserType.Individual) && transferRequest.Amount > ((decimal)TransactionLimit.Indiviual))
            {
                return new Response { Success = false, Data = $"Amount Over Limit" };
            }
            if (sender.UserType == UserTypeExtension.GetStringValue(UserType.Corporate) && transferRequest.Amount > ((decimal)TransactionLimit.Corporate))
            {
                return new Response { Success = false, Data = $"Amount Over Limit" };
            }

            var transactionModel = new TransactionModel
            {
                Receiver = receiver,
                Sender = sender,
                UserType = sender.UserType,
                Amount = transferRequest.Amount,
                CustomerTransactionDescription = transferRequest.Description
            };

            Response transactionCheck = await SetTransferAsync(transactionModel);
            return new Response { Success = transactionCheck.Success, Data = transactionCheck.Data };
        }
    }
}
