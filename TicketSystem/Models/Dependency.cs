namespace TicketSystem.Models
{
    public class Dependency
    {
        public int Id { get; set; }
        public required Ticket DependentTicket { get; set; }
        public required int DependentTicketId { get; set; }
        public required Ticket TicketDependency { get; set; }
        public required int TicketDependencyId { get; set; }
    }
}
