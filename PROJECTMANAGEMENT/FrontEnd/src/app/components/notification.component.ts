import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationConfig } from '../models/ticket.model';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div *ngIf="notification?.show"
         [ngClass]="notification.type === 'success'
            ? 'bg-green-50 border-green-500 text-green-700'
            : 'bg-red-50 border-red-500 text-red-700'"
         class="fixed top-4 right-4 z-50 p-4 rounded-lg shadow-lg border-l-4 flex items-center gap-3 min-w-[300px]">
      <div class="p-1 rounded-full"
           [ngClass]="notification.type === 'success' ? 'bg-green-100' : 'bg-red-100'">
        {{ notification.type === 'success' ? '✓' : '⚠' }}
      </div>

      <div class="flex-1">
        <p class="font-bold">
          {{ notification.type === 'success' ? 'Success!' : 'Error!' }}
        </p>
        <p class="text-sm">{{ notification.message }}</p>
      </div>

      <button (click)="onClose.emit()" class="text-xl font-bold hover:opacity-70">
        ×
      </button>
    </div>
  `
})
export class NotificationComponent {
  @Input() notification!: NotificationConfig;
  @Output() onClose = new EventEmitter<void>();
}
