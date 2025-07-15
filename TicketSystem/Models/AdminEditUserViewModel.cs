namespace TicketSystem.Models
{
    public class AdminEditUserViewModel
    {
        public required string Id { get; set; }
        // reguläre Daten
        public required string Username { get; set; }
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public string Fullname => $"{Firstname} {Lastname}";
        public string? Email { get; set; }
        public string? Phone { get; set; }
        // Passwort (optional)
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
        // Rollen
        public required List<AdminRoleCheckbox> Roles { get; set; } = new();
    }
}
