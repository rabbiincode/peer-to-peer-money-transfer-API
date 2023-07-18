using Microsoft.Extensions.Logging;
using peer_to_peer_money_transfer.DAL.Entities;
using peer_to_peer_money_transfer.DAL.Interfaces;

namespace peer_to_peer_money_transfer.BLL.Infrastructure
{
    public class GenerateAccountNumber
    {
        private readonly IRepository<ApplicationUser> _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GenerateAccountNumber> _logger;

        public GenerateAccountNumber(IUnitOfWork unitOfWork, ILogger<GenerateAccountNumber> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userRepo = _unitOfWork.GetRepository<ApplicationUser>();
        }
        
        public async Task<string> GenerateAccount()
        {
            try
            {
                start:
                const string Number = "37";
                Random random = new Random();

                var randomNumber = random.Next(10000000, 99999999);

                string accountNumber = Number + randomNumber.ToString();

                var accountExists = await _userRepo.AnyAsync(a => a.AccountNumber == accountNumber);

                switch (accountExists)
                {
                    case true:
                        goto start;
                    default:
                        return accountNumber;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(GenerateAccountNumber)}");
                return "Registration failed...please try again";
            }
        }
    }
}
