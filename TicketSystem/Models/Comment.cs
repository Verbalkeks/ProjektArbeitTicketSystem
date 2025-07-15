namespace TicketSystem.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string AuthorId { get; set; }
        public required AppUser Author { get; set; }
        public required int TicketId { get; set; }
        public required Ticket Ticket { get; set; }
    }
}
