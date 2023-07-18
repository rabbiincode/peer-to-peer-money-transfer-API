using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace peer_to_peer_money_transfer.DAL.Entities;

public class ApplicationUserRole : IdentityUserRole<string>
{
    public virtual ApplicationUser ApplicationUser { get; set; }
    public virtual ApplicationRole ApplicationRole { get; set; }
}