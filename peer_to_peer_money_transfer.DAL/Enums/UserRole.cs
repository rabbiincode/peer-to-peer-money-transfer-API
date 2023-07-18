namespace peer_to_peer_money_transfer.DAL.Enums
{
    public enum UserRole
    {
        SuperAdmin = 1,
        Admin,
        User,
    }

    public static class GetUserRole
    {
        public static string? GetStringValue(this UserRole userRole)
        {
            return userRole switch
            {
                UserRole.SuperAdmin => "SuperAdmin",
                UserRole.Admin => "Admin",
                UserRole.User => "user",
                _ => null
            };
        }
    }
}
