namespace TicketSystem.Models
{
    public class DetailsTicketViewModel
    {
        public Ticket Ticket { get; set; }
        public Comment Comment { get; set; }
        public List<Comment> Comments { get; set; }
        public FileUploadViewModel File { get; set; }
        public List<FileInfoViewModel> Files { get; set; }
    }
}
