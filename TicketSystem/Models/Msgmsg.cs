using Microsoft.AspNetCore.Identity;

namespace TicketSystem.Models
{
    public class Msgmsg
    {
        public Guid Id { get; set; }
        public Guid Chatid { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public string SenderId { get; set; }

        public MsgUser Chat { get; set; }
        public AppUser Sender {  get; set; }
    }
}
