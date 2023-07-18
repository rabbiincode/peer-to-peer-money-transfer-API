 using Microsoft.Extensions.DependencyInjection;

namespace peer_to_peer_money_transfer.BLL.Extensions
{
    public static class PolicyExtension
    {
        public static void AddPolicyAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("SuperAdmin",
                    policy => policy.RequireRole("SuperAdmin"));
            });
        }
    }
}
