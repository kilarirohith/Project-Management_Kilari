namespace server.DTOs
{
    public class DashboardSummaryDTO
    {
        // Projects
        public int TotalProjects { get; set; }
        public int RunningProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int DelayedProjects { get; set; }
        public int OnHoldProjects { get; set; }

        // Tasks
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }

        // Tickets
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int OnHoldTickets { get; set; }
    }
}
