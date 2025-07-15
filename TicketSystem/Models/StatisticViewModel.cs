namespace TicketSystem.Models
{
    public class StatisticViewModel
    {
        // ViewModel for Statistic \\


        public int TicketOpen { get; set; }
        public int TicketClose { get; set; }
        public int TicketTotal { get; set; }

        public int ProjectOpen { get; set; }
        public int ProjectClose { get; set; }
        public int ProjectTotal { get; set; }

        public int Admin { get; set; }
        public int Dev { get; set; }
        public int Sup { get; set; }

    }
}
