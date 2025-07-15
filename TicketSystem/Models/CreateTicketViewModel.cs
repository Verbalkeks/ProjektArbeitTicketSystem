using Microsoft.EntityFrameworkCore.InMemory.ValueGeneration.Internal;
using Microsoft.Identity.Client;

namespace TicketSystem.Models
{
    public class CreateTicketViewModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int AssignedProjectId { get; set; }
        public Project? AssignedProject { get; set; }
        public string? AssignedUserId { get; set; }
        public List<int>? TicketDependencies { get; set; }
    }
}
