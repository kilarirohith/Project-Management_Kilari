import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-masters',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './master.html',
  styleUrls: ['./master.css']
})
export class MastersComponent {
  constructor(private router: Router) {}

  masterCards = [
    { 
      title: 'Project Master', 
      description: 'Manage projects', 
      buttonText: 'Manage Project Master', 
      route: 'project-master',      // ✅ child path only
      icon: 'bi-folder-fill'
    },
    { 
      title: 'Milestone Master', 
      description: 'Manage milestones', 
      buttonText: 'Manage Milestone Master', 
      route: 'milestone-master',
      icon: 'bi-flag-fill'
    },
    { 
      title: 'Client Master', 
      description: 'Manage clients', 
      buttonText: 'Manage Client Master', 
      route: 'client-master',
      icon: 'bi-briefcase-fill'
    },
    { 
      title: 'Vendor Master', 
      description: 'Manage vendors', 
      buttonText: 'Manage Vendor Master', 
      route: 'vendor-master',
      icon: 'bi-shop'
    },
    { 
      title: 'User Master', 
      description: 'Manage users', 
      buttonText: 'Manage User Master', 
      route: 'user',
      icon: 'bi-people-fill'
    },
    { 
      title: 'Role Master', 
      description: 'Manage roles', 
      buttonText: 'Manage Role Master', 
      route: 'role-master',
      icon: 'bi-shield-lock-fill'
    }
  ];

  navigateTo(route: string) {
    // ✅ always builds /layout/<child>
    this.router.navigate(['/layout', route]);
  }
}
