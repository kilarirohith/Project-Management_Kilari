import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { TicketService } from '../services/ticket.service';
import { Ticket, NotificationConfig } from '../models/ticket.model';
import { NotificationComponent } from '../components/notification.component';
import { UserService, SimpleUser } from '../services/user.service';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-ticketing-system',
  standalone: true,
  imports: [CommonModule, FormsModule, NotificationComponent],
  templateUrl: './ticketing-system.html',
  styleUrls: ['./ticketing-system.css']
})
export class TicketingSystemComponent implements OnInit {
  tickets: Ticket[] = [];

  // Form model
  form: Ticket = {
    title: '',
    description: '',
    priority: 'Medium',
    createdByUserId: 0,
    assignedToUserId: null
  };

  editingId: number | null = null;
  isSaving = false;
  loading = false;

  // Users for dropdown
  users: SimpleUser[] = [];
  currentUserId: number | null = null;

  // Notification state
  notification: NotificationConfig = {
    show: false,
    type: 'success',
    message: ''
  };

  constructor(
    private ticketService: TicketService,
    private userService: UserService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadCurrentUser();
    this.loadUsers();
    this.loadTickets();
  }

  // ------------------------------
  // AUTH HELPERS
  // ------------------------------
  loadCurrentUser() {
    const user = this.authService.getUser?.(); // your existing implementation
    if (user && user.id) {
      this.currentUserId = user.id;
      this.form.createdByUserId = user.id;
    }
  }

  // ------------------------------
  // LOAD USERS FOR DROPDOWN
  // ------------------------------
  loadUsers() {
    this.userService.getSimpleUsers().subscribe({
      next: (data) => {
        this.users = data;
      },
      error: () => {
        this.showError('Failed to load users');
      }
    });
  }

  // ------------------------------
  // LOAD TICKETS
  // ------------------------------
  loadTickets() {
    this.loading = true;
    this.ticketService.getAll().subscribe({
      next: (data) => {
        this.tickets = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.showError('Failed to load tickets');
      }
    });
  }

  // ------------------------------
  // FORM HELPERS
  // ------------------------------
  startCreate() {
    this.editingId = null;
    this.form = {
      title: '',
      description: '',
      priority: 'Medium',
      createdByUserId: this.currentUserId ?? 0,
      assignedToUserId: null
    };
  }

  startEdit(ticket: Ticket) {
    this.editingId = ticket.id ?? null;
    this.form = {
      ...ticket,
      createdByUserId: ticket.createdByUserId ?? (this.currentUserId ?? 0),
      assignedToUserId: ticket.assignedToUserId ?? null
    };
  }

  cancelForm() {
    this.editingId = null;
    this.form = {
      title: '',
      description: '',
      priority: 'Medium',
      createdByUserId: this.currentUserId ?? 0,
      assignedToUserId: null
    };
  }

  // Assign to current user
  assignToMe() {
    if (this.currentUserId != null) {
      this.form.assignedToUserId = this.currentUserId;
    } else {
      this.showError('Could not detect current user from login');
    }
  }

  // ------------------------------
  // SAVE (CREATE / UPDATE)
  // ------------------------------
  saveTicket() {
    if (!this.form.title.trim() || !this.form.description.trim()) {
      this.showError('Title and Description are required');
      return;
    }

    // 👇 Ensure createdByUserId is always set
    if (!this.form.createdByUserId && this.currentUserId) {
      this.form.createdByUserId = this.currentUserId;
    }

    this.isSaving = true;

    if (this.editingId) {
      // UPDATE
      this.ticketService.update(this.editingId, this.form).subscribe({
        next: () => {
          this.isSaving = false;
          this.showSuccess('Ticket updated successfully!');
          this.cancelForm();
          this.loadTickets();
        },
        error: () => {
          this.isSaving = false;
          this.showError('Failed to update ticket');
        }
      });
    } else {
      // CREATE
      this.ticketService.create(this.form).subscribe({
        next: () => {
          this.isSaving = false;
          this.showSuccess('Ticket created successfully!');
          this.cancelForm();
          this.loadTickets();
        },
        error: () => {
          this.isSaving = false;
          this.showError('Failed to create ticket');
        }
      });
    }
  }

  // ------------------------------
  // DELETE
  // ------------------------------
  deleteTicket(id: number | undefined) {
    if (!id) return;
    if (!confirm('Are you sure you want to delete this ticket?')) return;

    this.ticketService.delete(id).subscribe({
      next: () => {
        this.showSuccess('Ticket deleted successfully!');
        this.loadTickets();
      },
      error: () => {
        this.showError('Failed to delete ticket');
      }
    });
  }

  // ------------------------------
  // NOTIFICATION HELPERS
  // ------------------------------
  showSuccess(message: string) {
    this.notification = {
      show: true,
      type: 'success',
      message
    };
    this.autoHide();
  }

  showError(message: string) {
    this.notification = {
      show: true,
      type: 'error',
      message
    };
    this.autoHide();
  }

  autoHide() {
    setTimeout(() => {
      this.notification.show = false;
    }, 3000);
  }

  onNotificationClose() {
    this.notification.show = false;
  }
}
