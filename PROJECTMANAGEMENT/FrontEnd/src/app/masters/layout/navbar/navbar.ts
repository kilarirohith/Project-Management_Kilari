import { Component, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common'; 
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../auth/auth.service'

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.css']
})
export class NavbarComponent implements OnInit {
  @Output() sidebarToggle = new EventEmitter<void>();
  
  userAvatar: string | null = null;
  username = 'Guest'; // Default

  constructor(private auth: AuthService) {}

  ngOnInit() {
    const user = this.auth.getUser();
    if (user && user.fullName) {
      this.username = user.fullName;
    }
  }

  toggleSidebar(): void {
    this.sidebarToggle.emit();
  }

  logout(): void {
    this.auth.logout();
  }
}