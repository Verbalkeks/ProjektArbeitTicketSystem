namespace TicketSystem.Models
{
    public class TicketFilterViewModel
    {

        public int? ProjectId { get; set; }
        public string? CreatorId { get; set; }
        public string? AssignedUserId { get; set; }
        public Standings? Standing { get; set; }
    }
}
