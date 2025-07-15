namespace TicketSystem.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string User {  get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
