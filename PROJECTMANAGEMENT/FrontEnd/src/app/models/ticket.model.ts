// src/app/models/ticket.model.ts
export interface Ticket {
  id?: number;

  ticketNumber?: string;
  title: string;
  description: string;

  clientName?: string;
  location?: string;
  category?: string;

  raisedBy?: string;
  assignedTo?: string;

  priority: string;
  status?: string;
  resolution?: string;

  dateRaised?: string;
  timeRaised?: string;

  dateClosed?: string | null;
  timeClosed?: string | null;

  totalHoursElapsed?: number;
  totalDaysElapsed?: number;

  createdByName?: string;
  assignedToName?: string;

  createdByUserId?: number;
  assignedToUserId?: number | null;
}

export interface NotificationConfig {
  show: boolean;
  type: 'success' | 'error';
  message: string;
}

export interface FilterConfig {
  status: string;
  clientName: string;
  priority: string;
}
