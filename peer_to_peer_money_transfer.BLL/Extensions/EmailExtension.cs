using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using peer_to_peer_money_transfer.Shared.EmailConfiguration;

namespace peer_to_peer_money_transfer.BLL.Extensions
{
    public static class EmailExtension
    {
        public static void ConfigureEmailServices(this IServiceCollection services)
        {
            IConfiguration config;

            using (var serviceProvider = services.BuildServiceProvider())
            {
                config = serviceProvider.GetService<IConfiguration>();
            }

            var emailConfig = config.GetSection("EmailConfiguration").Get<EmailConfiguration>();

            services.AddSingleton(emailConfig);
        }
    }
}
