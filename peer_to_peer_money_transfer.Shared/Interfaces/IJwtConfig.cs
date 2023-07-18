using peer_to_peer_money_transfer.DAL.Entities;

namespace peer_to_peer_money_transfer.Shared.Interfaces
{
    public interface IJwtConfig
    {
        Task<string> GenerateJwtToken(ApplicationUser user);
    }
}
