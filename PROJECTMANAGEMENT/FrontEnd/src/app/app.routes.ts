// src/app/app.routes.ts
import { Routes } from '@angular/router';

import { LoginComponent } from './masters/login/login';
import { LayoutComponent } from './masters/layout/layout';
import { DashboardComponent } from './Dashboard/dashboard/dashboard';
import { ClientMasterComponent } from './masters/client-master/client-master';
import { ProjectMasterComponent } from './masters/project-master/project-master';
import { UserMasterComponent } from './masters/users/users';
import { RoleMasterComponent } from './role-master/role-master';
import { TaskTrackerComponent } from './task-tracker/task-tracker';
import { VendorMasterComponent } from './masters/vendor-master/vendor-master';
import { MilestoneMasterComponent } from './masters/mileston-master/mileston-master';
import { ProjectsComponent } from './projects/projects';
import { MastersComponent } from './masters/master/master';
import { VendorWorkComponent } from './masters/vendor-work/vendor-work.component';
import { ApprovalDeskComponent } from './approval-desk/approval-desk.component';
import { TicketingSystemComponent } from './ticketing-system/ticketing-system';
import { AuthGuard } from './masters/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },

  {
    path: 'layout',
    component: LayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: 'dashboard', component: DashboardComponent },
      { path: 'masters', component: MastersComponent },
      { path: 'client-master', component: ClientMasterComponent },
      { path: 'project-master', component: ProjectMasterComponent },
      { path: 'user', component: UserMasterComponent },
      { path: 'role-master', component: RoleMasterComponent },
      { path: 'vendor-master', component: VendorMasterComponent },
      { path: 'milestone-master', component: MilestoneMasterComponent },
      { path: 'vendor-work', component: VendorWorkComponent },
      { path: 'projects', component: ProjectsComponent },
      { path: 'task-tracker', component: TaskTrackerComponent },
      { path: 'approval-desk', component: ApprovalDeskComponent }
    ]
  },
  { path: '**', redirectTo: 'login' }
];
