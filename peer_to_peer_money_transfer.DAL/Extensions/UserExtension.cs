using System.Security.Claims;

namespace peer_to_peer_money_transfer.DAL.Extensions
{
    public static class UserExtension
    {
        public static string? GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst("Id")?.Value;
        }
    }
}
