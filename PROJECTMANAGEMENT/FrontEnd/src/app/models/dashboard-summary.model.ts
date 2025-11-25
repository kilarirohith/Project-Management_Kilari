export interface DashboardSummary {
  totalProjects: number;
  runningProjects: number;
  completedProjects: number;
  delayedProjects: number;
  onHoldProjects: number;

  totalTasks: number;
  completedTasks: number;
  pendingTasks: number;

  totalTickets: number;
  openTickets: number;
  closedTickets: number;
  onHoldTickets: number;
}
