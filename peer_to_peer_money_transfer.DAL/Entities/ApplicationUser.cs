using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace peer_to_peer_money_transfer.DAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? MiddleName { get; set; }

        public string? AccountNumber { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Balance { get; set; } = 0;

        public string? RecoveryMail { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string UserType { get; set; }

        public string? BusinessName { get; set; }

        public string? NIN { get; set; }

        public string? CAC { get; set; }

        public string? BusinessType { get; set; }

        public string? Profession { get; set; }

        public string? BVN { get; set; }

        public string Address { get; set; } = null!;

        public bool Verified { get; set; } = false;
        
        public bool Activated { get; set; } = false;

        public bool Deleted { get; set; } = false;
        
        public bool Lien { get; set; } = false;

        public virtual ICollection<Complains>? Complains { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public virtual ICollection<TransactionHistory>? TransactionHistory { get; set; }
        public virtual ICollection<ApplicationUserClaim>? Claims { get; set; }
        public virtual ICollection<IdentityUserLogin<string>>? Logins { get; set; }
        public virtual ICollection<IdentityUserToken<string>>? Tokens { get; set; }
        public virtual ICollection<ApplicationUserRole>? UserRoles { get; set; }
    }
}
