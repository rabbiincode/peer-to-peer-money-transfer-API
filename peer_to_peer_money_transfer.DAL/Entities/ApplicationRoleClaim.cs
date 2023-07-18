using Microsoft.AspNetCore.Identity;

namespace peer_to_peer_money_transfer.DAL.Entities;

public class ApplicationRoleClaim : IdentityRoleClaim<string>
{
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; }
    public bool Active { get; set; } = true;
    public virtual ApplicationRole ApplicatonRole { get; set; }
}