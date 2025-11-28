// src/app/role-master/role-master.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RoleService, Role, RolePermission, AppModule } from '../services/role.service';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-role-master',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule],
  templateUrl: './role-master.html',
  styleUrls: ['./role-master.css']
})
export class RoleMasterComponent implements OnInit {

  isAdmin = false;

  modules: AppModule[] = [];
  actions = ['read', 'create', 'update', 'delete'];

  newModuleName = '';

  roles: Role[] = [];
  showForm = false;
  editingIndex: number | null = null;

  form: Role = {
    name: '',
    description: '',
    permissions: []
  };

  constructor(
    private roleService: RoleService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.isAdmin = this.authService.isAdmin();
    this.loadModules();
    this.loadRoles();
  }

  // ------- Modules -------
  loadModules() {
    this.roleService.getModules().subscribe({
      next: res => this.modules = res,
      error: err => console.error('Error loading modules', err)
    });
  }

  addModule() {
    const name = this.newModuleName.trim();
    if (!name) return;

    this.roleService.createModule(name).subscribe({
      next: () => {
        this.newModuleName = '';
        this.loadModules();
      },
      error: err => alert(err.error || 'Failed to add module')
    });
  }

  deleteModule(mod: AppModule) {
    if (!mod.id) return;
    if (!confirm(`Delete module "${mod.name}"? (Only if not used in roles)`)) return;

    this.roleService.deleteModule(mod.id).subscribe({
      next: () => {
        // remove corresponding permissions from current form
        this.form.permissions = this.form.permissions.filter(p => p.module !== mod.name);
        this.loadModules();
      },
      error: err => {
        console.error(err);
        alert(err.error || 'Cannot delete module (maybe used in roles).');
      }
    });
  }

  // ------- Roles -------
  loadRoles() {
    this.roleService.getRoles().subscribe({
      next: roles => this.roles = roles,
      error: () => console.error('Failed to load roles')
    });
  }

  saveRole() {
    if (!this.form.name) {
      alert('Role name is required');
      return;
    }

    if (this.editingIndex !== null && this.form.id) {
      this.roleService.updateRole(this.form.id, this.form).subscribe({
        next: () => {
          this.loadRoles();
          this.resetForm();
        }
      });
    } else {
      this.roleService.createRole(this.form).subscribe({
        next: () => {
          this.loadRoles();
          this.resetForm();
        }
      });
    }
  }

  deleteRole(index: number) {
    const id = this.roles[index].id;
    if (!id) return;

    if (confirm('Are you sure you want to delete this role?')) {
      this.roleService.deleteRole(id).subscribe({
        next: () => this.loadRoles()
      });
    }
  }

  editRole(index: number) {
    this.editingIndex = index;
    this.form = JSON.parse(JSON.stringify(this.roles[index]));
    this.showForm = true;
  }

  resetForm() {
    this.showForm = false;
    this.editingIndex = null;
    this.form = { name: '', description: '', permissions: [] };
  }

  // ------- Permission logic -------
  isChecked(moduleName: string, action: string): boolean {
    const perm = this.form.permissions.find(p => p.module === moduleName);
    if (!perm) return false;

    return !!{
      read: perm.canRead,
      create: perm.canCreate,
      update: perm.canUpdate,
      delete: perm.canDelete
    }[action];
  }

  togglePermission(moduleName: string, action: string) {
    let perm = this.form.permissions.find(p => p.module === moduleName);

    if (!perm) {
      perm = {
        module: moduleName,
        canCreate: false,
        canRead: false,
        canUpdate: false,
        canDelete: false
      };
      this.form.permissions.push(perm);
    }

    const key = `can${action.charAt(0).toUpperCase() + action.slice(1)}`;
    (perm as any)[key] = !(perm as any)[key];
  }

  getActionsList(perm: RolePermission): string[] {
    const list = [];
    if (perm.canRead) list.push('Read');
    if (perm.canCreate) list.push('Create');
    if (perm.canUpdate) list.push('Update');
    if (perm.canDelete) list.push('Delete');
    return list;
  }
}
