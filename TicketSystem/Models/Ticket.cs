using System.Diagnostics.CodeAnalysis;

namespace TicketSystem.Models
{

    public class Ticket
    {
        public int Id { get; set; }
        public required string Title { get; set; }

        public required string Description { get; set; }
        public required Project? AssignedProject { get; set; }
        public int? AssignedProjectId { get; set; }
        public AppUser? AssignedUser { get; set; }
        public string? AssignedUserId { get; set; }
        public DateTime AssignedDate { get; set; }
        public required AppUser Creator { get; set; }
        public required string CreatorId { get; set; }
        public required DateTime CreatedDate { get; set; }
        public required Standings Standing { get; set; }
        public DateTime ClosedAt { get; set; }
        public AppUser? ClosedBy { get; set; }
        [SetsRequiredMembers]
        public Ticket()
        {
            Standing = Standings.offen;
        }
        public int ReturnStanding()
        {
            int result = 0;
            switch (Standing)
            {
                case Standings.offen:
                    result = 1;
                    break;
                case Standings.zuteilung:
                    result = 2;
                    break;
                case Standings.bearbeitung:
                    result = 3;
                    break;
                case Standings.geschlossen:
                    result = 4;
                    break;
            }
            return result;
        }
    }
}
