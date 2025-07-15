using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TicketSystem.Models
{
    public class MsgUser
    {
        [Key]
        public Guid ChatId { get; set; }
        public string UserId1 { get; set; }
        public string UserId2 { get; set; }

        public AppUser User1 { get; set; }
        public AppUser User2 { get; set; }
        public ICollection<Msgmsg> Messages { get; set; }
    }
}
