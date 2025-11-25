import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService, User } from '../../services/user.service';
import { RoleService, Role } from '../../services/role.service';

@Component({
  selector: 'app-user-master',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './users.html',
  styleUrls: ['./users.css']
})
export class UserMasterComponent implements OnInit {
  users: User[] = [];
  roles: Role[] = [];
  
  showForm = false;
  isSaving = false;
  loading = false;
  editingUserId: number | null = null; // ✅ Track which user is being edited

  userForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private roleService: RoleService
  ) {
    this.userForm = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      roleId: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadRoles();
    this.loadUsers();
  }

  loadRoles() {
    this.roleService.getRoles().subscribe({
      next: (data) => this.roles = data,
      error: (err) => console.error('Failed to load roles', err)
    });
  }

  loadUsers() {
    this.loading = true;
    this.userService.getAllUsers().subscribe({
      next: (data) => {
        this.users = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load users', err);
        this.loading = false;
      }
    });
  }

  // ✅ 1. OPEN FORM FOR CREATE
  openForm() {
    this.showForm = true;
    this.editingUserId = null; // Reset edit ID
    this.userForm.reset();
    
    // Add Password Validator back (Required for new users)
    this.userForm.get('password')?.setValidators([Validators.required, Validators.minLength(6)]);
    this.userForm.get('password')?.updateValueAndValidity();

    if(this.roles.length > 0) this.userForm.patchValue({ roleId: this.roles[0].id });
  }

  // ✅ 2. OPEN FORM FOR EDIT
 editUser(id: number) { 
    // 1. Show loading or disable interaction if needed
    
    // 2. Call the API to get fresh data
    this.userService.getUserById(id).subscribe({
      next: (user) => {
        this.showForm = true;
        this.editingUserId = id; // Set the ID for tracking

        // 3. Populate form with data FROM THE SERVER
        this.userForm.patchValue({
          fullName: user.fullName,
          email: user.email,
          roleId: user.role?.id, // Ensure your Backend sends 'role' object
          password: '' // Keep blank
        });

        // 4. Handle Password Validator
        this.userForm.get('password')?.clearValidators();
        this.userForm.get('password')?.updateValueAndValidity();
      },
      error: (err) => {
        console.error(err);
        alert('Failed to fetch user details');
      }
    });
  }


  // ✅ 3. SAVE (Handles both Create and Update)
  saveUser() {
    if (this.userForm.invalid) {
      alert('Please fill all required fields');
      return;
    }

    this.isSaving = true;

    const payload = {
      fullName: this.userForm.value.fullName,
      email: this.userForm.value.email,
      roleId: Number(this.userForm.value.roleId),
      password: this.userForm.value.password // Send whatever is typed (or empty)
    };

    if (this.editingUserId) {
      // --- UPDATE MODE ---
      this.userService.updateUser(this.editingUserId, payload).subscribe({
        next: () => {
          alert('User updated successfully!');
          this.closeAndRefresh();
        },
        error: (err) => this.handleError(err)
      });
    } else {
      // --- CREATE MODE ---
      this.userService.createUser(payload).subscribe({
        next: () => {
          alert('User created successfully!');
          this.closeAndRefresh();
        },
        error: (err) => this.handleError(err)
      });
    }
  }

  deleteUser(id: number) {
    if (!confirm('Are you sure you want to delete this user?')) return;
    this.userService.deleteUser(id).subscribe({
      next: () => {
        alert('User deleted');
        this.loadUsers();
      },
      error: (err) => alert('Failed to delete')
    });
  }

  private closeAndRefresh() {
    this.showForm = false;
    this.editingUserId = null;
    this.isSaving = false;
    this.loadUsers();
  }

  private handleError(err: any) {
    alert('Error: ' + (err.error?.message || 'Operation failed'));
    this.isSaving = false;
  }
}