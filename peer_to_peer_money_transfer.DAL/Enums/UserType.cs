namespace peer_to_peer_money_transfer.DAL.Enums;

public enum UserType
{
    Individual = 1,
    Corporate,
    Admin,
    SuperAdmin
}

public static class UserTypeExtension
{
    public static string? GetStringValue(this UserType userType)
    {
        return userType switch
        {
            UserType.Individual => "Individual",
            UserType.Corporate => "Corporate",
            UserType.Admin => "Admin",
            UserType.SuperAdmin => "SuperAdmin",
            _ => null
        };
    }
}