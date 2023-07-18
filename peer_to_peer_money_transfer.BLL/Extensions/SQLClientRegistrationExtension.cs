using peer_to_peer_money_transfer.DAL.Context;
using peer_to_peer_money_transfer.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.DependencyInjection;

public static class SqlClientRegistrationExtension
{
    public static void AddDatabaseConnection(this IServiceCollection services)
    {
        IConfiguration config;

        using (var serviceProvider = services.BuildServiceProvider())
        {
            config = serviceProvider.GetService<IConfiguration>();
        }

        string connectionString = config.GetConnectionString("DefaultConn") ?? throw new InvalidOperationException("Connection string 'DefaultConn' not found");

        services.AddDbContext<ApplicationDBContext>(options =>
        {
            options.UseSqlServer(connectionString);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.SignIn.RequireConfirmedAccount = true)
       .AddDefaultTokenProviders()
       .AddEntityFrameworkStores<ApplicationDBContext>();
       /*.AddTokenProvider<ApplicationUser>("CashMingle")*/
       /*.AddRoles<ApplicationRole>();*/

        services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromMinutes(5));
    }
}