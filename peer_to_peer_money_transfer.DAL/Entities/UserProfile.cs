using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace peer_to_peer_money_transfer.DAL.Entities
{
    public class UserProfile : BaseEntities
    {
        [Key]
        public long Id { get; set; }

        public string UserId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? MiddleName { get; set; }

        public string? BusinessName { get; set; }

        public string? UserName { get; set; }

        public string? NIN{ get; set; }

        public string? CAC { get; set; }

        public string? BusinessType { get; set; }

        public string? Profession { get; set; }

        public DateTime DateOfBirth { get; set; }


        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }
    }
}
