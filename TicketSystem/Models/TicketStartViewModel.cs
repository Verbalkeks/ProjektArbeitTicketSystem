namespace TicketSystem.Models
{
    public class TicketStartViewModel
    {
        public TicketFilterViewModel Filter { get; set; } = new TicketFilterViewModel();
        public List<Ticket> TicketList { get; set; } = new List<Ticket>();
    }
}
