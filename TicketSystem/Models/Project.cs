using System.ComponentModel.DataAnnotations;

namespace TicketSystem.Models
{
    public class Project
    {
        public int Id { get; set; }
        [Required]
        public required string Title { get; set; }
        [Required]
        public required string Description { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public required DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public bool Closed { get; set; } = false;
        [DataType(DataType.Date)]
        public DateTime? ClosedAt { get; set; } = null;
        public ICollection<Ticket>? Tickets { get; set; }

    }
}
