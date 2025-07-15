namespace TicketSystem.Models
{
    public class FileModel
    {
        public int Id { get; set; }
        public required string FileName { get; set; }
        public required string FilePath { get; set; }
        public required long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
    }
}
