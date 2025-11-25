import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RoleService, Role, RolePermission } from '../services/role.service';
import {AuthService} from '../auth/auth.service';

@Component({
  selector: 'app-role-master',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule],
  templateUrl: './role-master.html',
  styleUrls: ['./role-master.css']
})
export class RoleMasterComponent implements OnInit {
  isAdmin = false; 
  // Configuration
  modules = ['Dashboard', 'Projects', 'Ticket Tracker', 'Task Tracker', 'Masters'];
  actions = ['read', 'create', 'update', 'delete'];

  // State
  roles: Role[] = [];
  showForm = false;
  editingIndex: number | null = null; // Keeps track if we are editing (using index/ID)
  
  // Active Form Data
  form: Role = {
    name: '',
    permissions: []
  };

  constructor(private roleService: RoleService,private authService: AuthService) {}

  ngOnInit() {
  this.isAdmin = this.authService.isAdmin();
  this.loadRoles();

}

  // --- API Calls ---
  loadRoles() {
    this.roleService.getRoles().subscribe(data => {
      this.roles = data;
    });
  }

  saveRole() {
    if (!this.form.name) return alert('Role Name is required');

    // If editing
    if (this.editingIndex !== null && this.form.id) {
      this.roleService.updateRole(this.form.id, this.form).subscribe(() => {
        this.loadRoles();
        this.resetForm();
      });
    } 
    // If creating
    else {
      this.roleService.createRole(this.form).subscribe(() => {
        this.loadRoles();
        this.resetForm();
      });
    }
  }

  deleteRole(index: number) {
    const roleId = this.roles[index].id;
    if (confirm('Are you sure you want to delete this role?') && roleId) {
      this.roleService.deleteRole(roleId).subscribe(() => {
        this.loadRoles();
      });
    }
  }

  // --- UI Logic ---

  resetForm() {
    this.showForm = false;
    this.editingIndex = null;
    // ✅ Reset description as well
    this.form = { name: '', description: '', permissions: [] };
  }

  editRole(index: number) {
    this.editingIndex = index;
    // Deep copy so we don't edit the list directly until saved
    this.form = JSON.parse(JSON.stringify(this.roles[index])); 
    this.showForm = true;
  }

  // --- Permission Logic (The tricky part) ---

  /**
   * Checks if a permission exists for the checkbox
   */
  isChecked(moduleName: string, action: string): boolean {
    const perm = this.form.permissions.find(p => p.module === moduleName);
    if (!perm) return false;

    switch (action) {
      case 'create': return perm.canCreate;
      case 'read': return perm.canRead;
      case 'update': return perm.canUpdate;
      case 'delete': return perm.canDelete;
      default: return false;
    }
  }

  /**
   * Updates the permission object when a checkbox is clicked
   */
  togglePermission(moduleName: string, action: string) {
    // 1. Find existing permission object for this module
    let perm = this.form.permissions.find(p => p.module === moduleName);

    // 2. If it doesn't exist yet, create it and add to array
    if (!perm) {
      perm = {
        module: moduleName,
        canCreate: false, canRead: false, canUpdate: false, canDelete: false
      };
      this.form.permissions.push(perm);
    }

    // 3. Toggle the specific boolean based on the action string
    switch (action) {
      case 'create': perm.canCreate = !perm.canCreate; break;
      case 'read': perm.canRead = !perm.canRead; break;
      case 'update': perm.canUpdate = !perm.canUpdate; break;
      case 'delete': perm.canDelete = !perm.canDelete; break;
    }
  }

  // Helper to display actions nicely in the list view
  getActionsList(perm: RolePermission): string[] {
    const actions = [];
    if (perm.canRead) actions.push('Read');
    if (perm.canCreate) actions.push('Create');
    if (perm.canUpdate) actions.push('Update');
    if (perm.canDelete) actions.push('Delete');
    return actions;
  }
}