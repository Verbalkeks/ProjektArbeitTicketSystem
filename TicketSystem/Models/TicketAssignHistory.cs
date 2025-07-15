namespace TicketSystem.Models
{
    public class TicketAssignHistory
    {
        public int Id { get; set; }
        public required DateTime AssignedAt { get; set; }
        public required AppUser AssignedBy { get; set; }
        public required string AssignedById { get; set; }
        public required AppUser AssignedTo { get; set; }
        public required string AssignedToId { get; set; }
        public required int TicketId { get; set; }
    }
}
