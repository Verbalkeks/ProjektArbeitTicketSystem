namespace TicketSystem.Models
{
    public class AdminCreateUserViewModel
    {
        // reguläre Daten
        public required string Username { get; set; }
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        // Passwort (optional)
        public required string? Password { get; set; }
        public required string? ConfirmPassword { get; set; }
        // Rollen
        public required List<AdminRoleCheckbox> Roles { get; set; } = new();
    }
}
