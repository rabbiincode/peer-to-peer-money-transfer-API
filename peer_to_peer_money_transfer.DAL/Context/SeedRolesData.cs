using Microsoft.EntityFrameworkCore;
using peer_to_peer_money_transfer.DAL.Entities;

namespace peer_to_peer_money_transfer.DAL.Context
{
    public static class SeedRolesData
    { 
        //An extension method on the ModelBuilder type
        public static void SeedRoles(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationRole>().HasData(
                new ApplicationRole {Name = "SuperAdmin", ConcurrencyStamp = "1", NormalizedName = "SUPERADMIN"},
                new ApplicationRole {Name = "Admin", ConcurrencyStamp = "2", NormalizedName = "ADMIN"},
                new ApplicationRole {Name = "User", ConcurrencyStamp = "3", NormalizedName = "USER"}
            );
        }
    }
}
