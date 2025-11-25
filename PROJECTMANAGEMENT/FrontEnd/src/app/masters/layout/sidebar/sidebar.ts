import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../auth/auth.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './sidebar.html',
  styleUrls: ['./sidebar.css']
})
export class SidebarComponent implements OnInit {
  @Input() sidebarOpen = false;
  @Output() sidebarClose = new EventEmitter<void>();

  username = 'User';
  role = 'User';

  constructor(private auth: AuthService) {}

  ngOnInit() {
    const user = this.auth.getUser();
    if (user) {
      this.username = user.fullName;
      this.role = user.role || 'User';
    }
  }

  getInitials(): string {
    return this.username.substring(0, 2).toUpperCase();
  }

  closeSidebar(): void {
    this.sidebarClose.emit();
  }

  logout(): void {
    this.auth.logout();
  }
}