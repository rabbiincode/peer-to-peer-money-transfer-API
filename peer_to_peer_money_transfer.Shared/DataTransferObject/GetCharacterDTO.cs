using peer_to_peer_money_transfer.DAL.Enums;

namespace peer_to_peer_money_transfer.Shared.DataTransferObject
{
    public class GetCharacterDTO
    {
        public string FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string? AccountNumber { get; set; }

        public string Address { get; set; }

        public DateTime? Birthday { get; set; }

        public string? BusinessName { get; set; }

        public string UserType { get; set; }

        public decimal Balance { get; set; }

        public bool Deleted { get; set; }

        public bool Active { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
