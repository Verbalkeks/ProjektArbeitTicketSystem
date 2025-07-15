namespace TicketSystem.Models
{
    public class AdminUserListViewModel
    {
        public required string ID { get; set; }
        public required string Name { get; set; }
        public required string Nutzername { get; set; }
        public string? Email { get; set; }
        public string? Telefonnummer { get; set; }
        public List<string> Roles { get; set; }
    }
}
