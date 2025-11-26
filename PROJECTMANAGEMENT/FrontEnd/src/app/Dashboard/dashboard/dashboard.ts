// src/app/Dashboard/dashboard/dashboard.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgxChartsModule, Color, ScaleType } from '@swimlane/ngx-charts';

import { DashboardService} from '../../services/dashboard.service';
import { DashboardSummary } from  '../../models/dashboard-summary.model'

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, NgxChartsModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class DashboardComponent implements OnInit {
  loading = false;
  errorMessage = '';

  projectStats = { total: 0, running: 0, completed: 0, delayed: 0 };
  ticketStats  = { total: 0, open: 0, closed: 0, hold: 0 };

  // ngx-charts settings
  view: [number, number] = [420, 260];
  showLegend = true;
  showLabels = true;
  explodeSlices = false;
  doughnut = false;

  projectPieData: any[] = [];
  ticketPieData: any[] = [];

  // must be public so template can read
  projectPieTotal = 0;
  ticketPieTotal  = 0;

  // Project colors (Running, Completed, Delayed)
  projectColorScheme: Color = {
    name: 'projectStatus',
    selectable: true,
    group: ScaleType.Ordinal,
    domain: ['#FFB300', '#4CAF50', '#F44336']
  };

  // Ticket colors (Open, Closed, On Hold)
  ticketColorScheme: Color = {
    name: 'ticketStatus',
    selectable: true,
    group: ScaleType.Ordinal,
    domain: ['#FFC107', '#22C55E', '#EF4444']
  };

  // Slice labels – leave just the name; we show % in the legend
  formatProjectLabel = (label: string): string => label;
  formatTicketLabel  = (label: string): string => label;

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.loadDashboard();
  }

  // 🔌 Fetch data from API
  private loadDashboard(): void {
    this.loading = true;
    this.errorMessage = '';

    this.dashboardService.getSummary().subscribe({
      next: (summary: DashboardSummary) => {
        // map API → UI stats
        this.projectStats = {
          total:    summary.totalProjects,
          running:  summary.runningProjects,
          completed: summary.completedProjects,
          delayed:  summary.delayedProjects
        };

        this.ticketStats = {
          total: summary.totalTickets,
          open:  summary.openTickets,
          closed: summary.closedTickets,
          hold:  summary.onHoldTickets
        };

        this.buildCharts();
        this.loading = false;
      },
      error: (err) => {
        console.error('Dashboard load failed', err);
        this.errorMessage = 'Failed to load dashboard data.';
        this.loading = false;
      }
    });
  }

  // Build chart arrays + totals for percentages in legend
  private buildCharts(): void {
    this.projectPieData = [
      { name: 'Running',   value: this.projectStats.running },
      { name: 'Completed', value: this.projectStats.completed },
      { name: 'Delayed',   value: this.projectStats.delayed }
    ];
    this.projectPieTotal = this.projectPieData
      .reduce((sum, item) => sum + item.value, 0);

    this.ticketPieData = [
      { name: 'Open',    value: this.ticketStats.open },
      { name: 'Closed',  value: this.ticketStats.closed },
      { name: 'On Hold', value: this.ticketStats.hold }
    ];
    this.ticketPieTotal = this.ticketPieData
      .reduce((sum, item) => sum + item.value, 0);
  }
}
