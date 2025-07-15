namespace TicketSystem.Models
{
    public class EditTicketViewModel
    {
        public required int Id { get; set; }
        public required string Description { get; set; }
        public required string? AssignedUserId { get; set; }
        public List<Ticket>? Blockings { get; set; }
        public Standings? Standing { get; set; }
    }
}
