using peer_to_peer_money_transfer.DAL.Context;
using peer_to_peer_money_transfer.DAL.Interfaces;
using peer_to_peer_money_transfer.DAL.Implementation;
using Microsoft.Extensions.DependencyInjection;
using peer_to_peer_money_transfer.BLL.Implementation;
using peer_to_peer_money_transfer.BLL.Interfaces;
using Microsoft.AspNetCore.Identity;
using peer_to_peer_money_transfer.DAL.Entities;
using peer_to_peer_money_transfer.Shared.Interfaces;
using peer_to_peer_money_transfer.Shared.JwtConfigurations;
using peer_to_peer_money_transfer.Shared.EmailConfiguration;
using peer_to_peer_money_transfer.Shared.SmsConfiguration;
using peer_to_peer_money_transfer.BLL.Infrastructure;

namespace peer_to_peer_money_transfer.BLL.Extensions
{
    public static class MiddlewareExtension
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork<ApplicationDBContext>>();
            services.AddTransient<ITransactionsServices, TransactionsServices>();
            services.AddScoped<UserManager<ApplicationUser>>();
            services.AddScoped<IAdminServices, AdminServices>();
            services.AddScoped<GenerateAccountNumber>();
            services.AddScoped<IJwtConfig, JwtConfig>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddTransient<ISendSms, SendSms>();
            services.AddScoped<IFundingService, FundingServices>();
        }
    }
}
