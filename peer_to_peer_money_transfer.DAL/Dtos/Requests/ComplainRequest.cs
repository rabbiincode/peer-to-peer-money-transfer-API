namespace peer_to_peer_money_transfer.DAL.Dtos.Requests
{
    public class ComplainRequest 
    {
        public long TransationId { get; set; }

        public string ComplainSubject { get; set; } = null!;

        public string ComplainDescription { get; set; } = null!;
    }
}
