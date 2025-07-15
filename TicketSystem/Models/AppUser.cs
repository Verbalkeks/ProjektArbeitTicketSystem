using Microsoft.AspNetCore.Identity;

namespace TicketSystem.Models
{
    public class AppUser : IdentityUser
    {
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public string Fullname
        {
            get => Firstname + " " + Lastname;
        }
    }
}
