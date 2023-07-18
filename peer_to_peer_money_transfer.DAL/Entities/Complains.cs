namespace peer_to_peer_money_transfer.DAL.Entities
{
    public class Complains
    {
        public long Id { get; set; }

        public string UserId { get; set; }

        public long TransationId { get; set; }

        public string ComplainSubject { get; set; } = null!;

        public string ComplainDescription { get; set; } = null!;

        public bool Isrevised { get; set; } = false;
    }
}
