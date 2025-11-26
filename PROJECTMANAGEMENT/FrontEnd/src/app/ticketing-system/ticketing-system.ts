// src/app/ticketing-system/ticketing-system.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { Ticket } from '../models/ticket.model';
import { TicketService } from '../services/ticket.service';
import { ClientService, Client, Location } from '../services/client.service';
import { SimpleUser, UserService } from '../services/user.service';
import { NotificationComponent } from '../components/notification.component';
import { NotificationConfig, FilterConfig } from '../models/ticket.model';

import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';

@Component({
  selector: 'app-ticketing-system',
  standalone: true,
  imports: [CommonModule, FormsModule, NotificationComponent],
  templateUrl: './ticketing-system.html',
  styleUrls: ['./ticketing-system.css']
})
export class TicketingSystemComponent implements OnInit {
  // ---------- DATA ----------
  tickets: Ticket[] = [];
  filteredTickets: Ticket[] = [];

  clients: Client[] = [];
  locationsForSelectedClient: Location[] = [];
  users: SimpleUser[] = [];

  // ---------- UI STATE ----------
  loading = false;
  isSaving = false;

  showFilterPanel = false;
  showForm = false;
  editingId: number | null = null;

  // form model
  form: any = this.getEmptyForm();

  // filters
  filters: FilterConfig = {
    status: 'All',
    clientName: 'All',
    priority: 'All'
  };

  // pagination
  pageSizeOptions = [10, 25, 50];
  pageSize = 10;
  currentPage = 1;

  // notification
  notification: NotificationConfig = {
    show: false,
    type: 'success',
    message: ''
  };

  // dropdown constants
  categories = ['Understanding', 'Requirement', 'Issue', 'Development', 'Client Scope'];
  statuses = ['Open', 'Closed', 'On Hold'];
  priorities = ['Low', 'Medium', 'High'];

  constructor(
    private ticketService: TicketService,
    private clientService: ClientService,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    this.loadBaseData();
  }

  // ---------- INIT HELPERS ----------

  private loadBaseData() {
    this.loading = true;

    // load tickets
    this.ticketService.getAll().subscribe({
      next: (res) => {
        this.tickets = res.map((t) => this.enrichTicket(t));
        this.applyFilters();
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.showError('Failed to load tickets');
      }
    });

    // clients
    this.clientService.getAllClients().subscribe({
      next: (res) => (this.clients = res),
      error: () => this.showError('Failed to load clients')
    });

    // users
    this.userService.getSimpleUsers().subscribe({
      next: (res) => (this.users = res),
      error: () => this.showError('Failed to load users')
    });
  }

 private enrichTicket(t: Ticket): Ticket {
  // ✅ If backend already sent durations, keep them
  if (t.totalHoursElapsed != null || t.totalDaysElapsed != null) {
    return t;
  }

  // Fallback: compute on UI side only if closed and all fields exist
  if (t.status === 'Closed' && t.dateRaised && t.timeRaised && t.dateClosed && t.timeClosed) {
    try {
      const start = new Date(`${t.dateRaised}T${t.timeRaised}`);
      const end   = new Date(`${t.dateClosed}T${t.timeClosed}`);
      const diffMs = end.getTime() - start.getTime();

      const hours = Math.abs(diffMs / (1000 * 60 * 60));
      const days  = Math.abs(diffMs / (1000 * 60 * 60 * 24));

      t.totalHoursElapsed = parseFloat(hours.toFixed(2));
      t.totalDaysElapsed  = Math.floor(days);
    } catch {
      // ignore bad dates
    }
  }

  return t;
}


  private getEmptyForm() {
    const now = new Date();
    const dateStr = now.toISOString().substring(0, 10);
    const timeStr = now.toTimeString().substring(0, 5); // HH:mm 24h

    return {
      id: null,
      ticketNumber: '',
      title: '',
      description: '',
      clientId: null as number | null,
      clientName: '',
      locationId: null as number | null,
      location: '',
      category: 'Issue',
      raisedByUserId: null as number | null,
      raisedBy: '',
      assignedToUserId: null as number | null,
      assignedTo: '',
      priority: 'Medium',
      status: 'Open',
      resolution: '',
      dateRaised: dateStr,
      timeRaised: timeStr,
      dateClosed: '',
      timeClosed: '',
      totalHoursElapsed: 0,
      totalDaysElapsed: 0
    };
  }

  // ---------- KPI COMPUTED ----------

  get totalTicketsCount() {
    return this.tickets.length;
  }

  get closedTicketsCount() {
    return this.tickets.filter((t) => t.status === 'Closed').length;
  }

  get openTicketsCount() {
    return this.tickets.filter((t) => t.status === 'Open').length;
  }

  get onHoldTicketsCount() {
    return this.tickets.filter((t) => t.status === 'On Hold').length;
  }

  // ---------- FILTERING + PAGINATION ----------

  toggleFilters() {
    this.showFilterPanel = !this.showFilterPanel;
  }

  onFilterChange() {
    this.applyFilters();
  }

  private applyFilters() {
    this.filteredTickets = this.tickets.filter((t) => {
      const matchStatus =
        this.filters.status === 'All' ||
        (t.status || '').toLowerCase() === this.filters.status.toLowerCase();
      const matchClient =
        this.filters.clientName === 'All' ||
        (t.clientName || '').toLowerCase() === this.filters.clientName.toLowerCase();
      const matchPriority =
        this.filters.priority === 'All' ||
        (t.priority || '').toLowerCase() === this.filters.priority.toLowerCase();

      return matchStatus && matchClient && matchPriority;
    });

    this.currentPage = 1;
  }

  get totalItems() {
    return this.filteredTickets.length;
  }

  get totalPages() {
    return Math.max(1, Math.ceil(this.totalItems / this.pageSize));
  }

  get pagedTickets(): Ticket[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredTickets.slice(start, start + this.pageSize);
  }

  onPageSizeChange() {
    this.currentPage = 1;
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
  }

  get showingFrom() {
    if (this.totalItems === 0) return 0;
    return (this.currentPage - 1) * this.pageSize + 1;
  }

  get showingTo() {
    return Math.min(this.currentPage * this.pageSize, this.totalItems);
  }

  // ---------- FORM HANDLERS ----------

  startAdd() {
    this.editingId = null;
    this.showForm = true;
    this.form = this.getEmptyForm();
    this.locationsForSelectedClient = [];
  }

  startEdit(ticket: Ticket) {
    this.editingId = ticket.id!;
    this.showForm = true;

    const client = this.clients.find(
      (c) => c.clientName.toLowerCase() === (ticket.clientName || '').toLowerCase()
    );
    let clientId: number | null = client ? client.id! : null;

    if (clientId) {
      this.onClientChange(clientId, false);
    }

    this.form = {
      id: ticket.id,
      ticketNumber: ticket.ticketNumber || '',
      title: ticket.title || '',
      description: ticket.description || '',
      clientId,
      clientName: ticket.clientName || '',
      locationId: null as number | null,
      location: ticket.location || '',
      category: ticket.category || 'Issue',
      raisedByUserId: null,
      raisedBy: ticket.raisedBy || '',
      assignedToUserId: ticket.assignedToUserId ?? null,
      assignedTo: ticket.assignedTo || '',
      priority: ticket.priority || 'Medium',
      status: ticket.status || 'Open',
      resolution: ticket.resolution || '',
      dateRaised: ticket.dateRaised ? ticket.dateRaised.substring(0, 10) : '',
      timeRaised: ticket.timeRaised || '',
      dateClosed: ticket.dateClosed ? ticket.dateClosed.substring(0, 10) : '',
      timeClosed: ticket.timeClosed || '',
      totalHoursElapsed: ticket.totalHoursElapsed || 0,
      totalDaysElapsed: ticket.totalDaysElapsed || 0
    };
  }

  cancelForm() {
    this.showForm = false;
    this.editingId = null;
    this.form = this.getEmptyForm();
  }

  onClientChange(clientId: number, clearLocation = true) {
    this.form.clientId = clientId;

    const client = this.clients.find((c) => c.id === clientId);
    this.form.clientName = client?.clientName || '';

    if (clearLocation) {
      this.form.locationId = null;
      this.form.location = '';
    }

    this.clientService.getLocationsByClient(clientId).subscribe({
      next: (locs) => (this.locationsForSelectedClient = locs),
      error: () => this.showError('Failed to load locations for client')
    });
  }

  onLocationChange(locationId: number) {
    this.form.locationId = locationId;
    const loc = this.locationsForSelectedClient.find((l) => l.id === locationId);
    this.form.location = loc?.locationName || '';
  }

  onRaisedByChange(userId: number) {
    this.form.raisedByUserId = userId;
    const user = this.users.find((u) => u.id === userId);
    this.form.raisedBy = user ? user.fullName : '';
  }

  onAssignedToChange(userId: number) {
    this.form.assignedToUserId = userId;
    const user = this.users.find((u) => u.id === userId);
    this.form.assignedTo = user ? user.fullName : '';
  }

  saveTicket() {
    // 🔧 IMPORTANT: this was why Add/Update were not working earlier.
    // You were validating "title", but the UI has no title input.
    // Now we only enforce client, location, description.

    if (!this.form.clientName || !this.form.location) {
      this.showError('Client and Location are required');
      return;
    }

    if (!this.form.description) {
      this.showError('Description is required');
      return;
    }

    // Auto-generate a simple title if not set
    if (!this.form.title || this.form.title.trim() === '') {
      this.form.title = `${this.form.category} - ${this.form.clientName}`;
    }

    this.isSaving = true;

    // build object compatible with backend DTOs
    const payload: any = {
      ticketNumber: this.form.ticketNumber,         // ignored for create
      title: this.form.title,
      description: this.form.description,
      clientName: this.form.clientName,
      location: this.form.location,
      category: this.form.category,
      raisedBy: this.form.raisedBy,
      assignedTo: this.form.assignedTo,
      priority: this.form.priority,
      status: this.form.status,
      resolution: this.form.resolution || '',
      dateRaised: this.form.dateRaised ? new Date(this.form.dateRaised) : null,
      timeRaised: this.form.timeRaised,
      dateClosed: this.form.dateClosed ? new Date(this.form.dateClosed) : null,
      timeClosed: this.form.timeClosed || '',
     
      assignedToUserId: this.form.assignedToUserId
    };

    if (this.editingId) {
      this.ticketService.update(this.editingId, payload).subscribe({
        next: (updated) => {
          this.isSaving = false;
          this.showSuccess('Ticket updated successfully');
          const idx = this.tickets.findIndex((t) => t.id === updated.id);
          if (idx >= 0) {
            this.tickets[idx] = this.enrichTicket(updated);
          }
          this.applyFilters();
          this.cancelForm();
        },
        error: () => {
          this.isSaving = false;
          this.showError('Failed to update ticket');
        }
      });
    } else {
      // For new tickets ensure status is Open and dateRaised/timeRaised are now
      if (!payload.status) {
        payload.status = 'Open';
      }
      if (!payload.dateRaised) {
        payload.dateRaised = new Date();
      }

      this.ticketService.create(payload).subscribe({
        next: (created) => {
          this.isSaving = false;
          this.showSuccess('Ticket created successfully');
          this.tickets.unshift(this.enrichTicket(created));
          this.applyFilters();
          this.cancelForm();
        },
        error: () => {
          this.isSaving = false;
          this.showError('Failed to create ticket');
        }
      });
    }
  }

  deleteTicket(id: number | undefined) {
    if (!id) return;
    if (!confirm('Are you sure you want to delete this ticket?')) return;

    this.ticketService.delete(id).subscribe({
      next: () => {
        this.showSuccess('Ticket deleted');
        this.tickets = this.tickets.filter((t) => t.id !== id);
        this.applyFilters();
      },
      error: () => this.showError('Failed to delete ticket')
    });
  }

  // ---------- STATUS / PRIORITY STYLES ----------

  getStatusClass(status?: string) {
    switch ((status || '').toLowerCase()) {
      case 'closed':
        return 'status-pill status-closed';
      case 'open':
        return 'status-pill status-open';
      case 'on hold':
        return 'status-pill status-hold';
      default:
        return 'status-pill';
    }
  }

  getPriorityClass(priority?: string) {
    switch ((priority || '').toLowerCase()) {
      case 'low':
        return 'priority-pill priority-low';
      case 'high':
        return 'priority-pill priority-high';
      default:
        return 'priority-pill priority-medium';
    }
  }

  // ---------- EXPORT EXCEL ----------

  exportToExcel() {
    const rows = this.filteredTickets.map((t) => ({
      'Ticket Number': t.ticketNumber || '',
      'Client Name': t.clientName || '',
      Location: t.location || '',
      'Date Raised': t.dateRaised || '',
      'Time Raised': t.timeRaised || '',
      Category: t.category || '',
      'Raised By': t.raisedBy || '',
      'Assigned To': t.assignedTo || '',
      Description: t.description || '',
      'Total Hours Elapsed': t.totalHoursElapsed ?? '',
      'Total Days Elapsed': t.totalDaysElapsed ?? '',
      Status: t.status || '',
      Priority: t.priority || '',
      Resolution: t.resolution || '',
      'Date Closed': t.dateClosed || '',
      'Time Closed': t.timeClosed || ''
    }));

    const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(rows);
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Tickets');

    const wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'array' });
    const blob = new Blob([wbout], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8'
    });
    saveAs(blob, `tickets_${new Date().toISOString().substring(0, 10)}.xlsx`);
  }

  // ---------- NOTIFICATION ----------

  private showSuccess(message: string) {
    this.notification = { show: true, type: 'success', message };
  }

  private showError(message: string) {
    this.notification = { show: true, type: 'error', message };
  }

  onNotificationClose() {
    this.notification.show = false;
  }
}
