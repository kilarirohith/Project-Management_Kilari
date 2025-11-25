import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardService } from '../../services/dashboard.service';
import { DashboardSummary } from '../../models/dashboard-summary.model';
import { NgxChartsModule } from '@swimlane/ngx-charts';

interface StatCard {
  title: string;
  value: number;
  icon: string;
  gradient?: boolean;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, NgxChartsModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class DashboardComponent implements OnInit {
  // Stats from API
  projectStats = {
    total: 0,
    running: 0,
    completed: 0,
    delayed: 0,
    onHold: 0
  };

  ticketStats = {
    total: 0,
    open: 0,
    closed: 0,
    hold: 0
  };

  // Pie chart data for ngx-charts
  projectPieData: { name: string; value: number }[] = [];
  ticketPieData: { name: string; value: number }[] = [];

  // Optional extra settings for ngx-charts
  showLegend = true;
  showLabels = true;
  explodeSlices = false;
  doughnut = false; // set true if you want donut style
  gradient = true;
  view: [number, number] = [400, 300]; // width x height

  statCards: StatCard[] = [];

  loading = false;
  errorMessage = '';

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.loadSummary();
  }

  private loadSummary() {
    this.loading = true;
    this.dashboardService.getSummary().subscribe({
      next: (summary: DashboardSummary) => {
        this.loading = false;

        // ---------- Map to local stats ----------
        this.projectStats = {
          total: summary.totalProjects,
          running: summary.runningProjects,
          completed: summary.completedProjects,
          delayed: summary.delayedProjects,
          onHold: summary.onHoldProjects
        };

        this.ticketStats = {
          total: summary.totalTickets,
          open: summary.openTickets,
          closed: summary.closedTickets,
          hold: summary.onHoldTickets
        };

        // ---------- Cards ----------
        this.statCards = [
          { title: 'Total Projects', value: this.projectStats.total, icon: '📊', gradient: true },
          { title: 'Running Projects', value: this.projectStats.running, icon: '📈' },
          { title: 'Completed Projects', value: this.projectStats.completed, icon: '✅' },
          { title: 'Delayed Projects', value: this.projectStats.delayed, icon: '⚠️' },
          { title: 'On Hold Projects', value: this.projectStats.onHold, icon: '⏸️' },

          { title: 'Total Tickets', value: this.ticketStats.total, icon: '🎫', gradient: true },
          { title: 'Open Tickets', value: this.ticketStats.open, icon: '⏳' },
          { title: 'Closed Tickets', value: this.ticketStats.closed, icon: '✔️' },
          { title: 'On Hold Tickets', value: this.ticketStats.hold, icon: '🛑' }
        ];

        // ---------- Pie data for ngx-charts ----------
        this.projectPieData = [
          { name: 'Running', value: this.projectStats.running },
          { name: 'Completed', value: this.projectStats.completed },
          { name: 'Delayed', value: this.projectStats.delayed },
          { name: 'On Hold', value: this.projectStats.onHold }
        ];

        this.ticketPieData = [
          { name: 'Open', value: this.ticketStats.open },
          { name: 'Closed', value: this.ticketStats.closed },
          { name: 'On Hold', value: this.ticketStats.hold }
        ];
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = 'Failed to load dashboard summary';
        console.error(err);
      }
    });
  }
}
