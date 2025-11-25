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
  route: '/layout/project-master',
  icon: 'bi-folder-fill'
},

    { 
      title: 'Milestone Master', 
      description: 'Manage milestones', 
      buttonText: 'Manage Milestone Master', 
      route: '/layout/milestone-master',
      icon: 'bi-flag-fill'
    },
    { 
      title: 'Client Master', 
      description: 'Manage clients', 
      buttonText: 'Manage Client Master', 
      route: '/layout/client-master',
      icon: 'bi-briefcase-fill'
    },
    { 
      title: 'Vendor Master', 
      description: 'Manage vendors', 
      buttonText: 'Manage Vendor Master', 
      route: '/layout/vendor-master',
      icon: 'bi-shop'
    },
    { 
      title: 'User Master', 
      description: 'Manage users', 
      buttonText: 'Manage User Master', 
      route: '/layout/user',
      icon: 'bi-people-fill'
    },
    { 
      title: 'Role Master', 
      description: 'Manage roles', 
      buttonText: 'Manage Role Master', 
      route: '/layout/role-master',
      icon: 'bi-shield-lock-fill'
    }
  ];

  navigateTo(route: string) {
    this.router.navigate([route]);
  }
}