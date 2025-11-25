import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

// Angular Material Imports
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

// Services
import { ProjectMasterService, ProjectMaster } from '../../services/projectMaster.service'
import { ClientService, Client } from '../../services/client.service';

@Component({
  selector: 'app-project-master',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCardModule,
    MatIconModule
  ],
  templateUrl: './project-master.html',
  styleUrls: ['./project-master.css']
})
export class ProjectMasterComponent implements OnInit {
  form: FormGroup;
  projects: ProjectMaster[] = [];
  clients: Client[] = []; // For the Dropdown
  
  showForm = false;
  isSaving = false;
  editingId: number | null = null;

  displayedColumns: string[] = ['projectName', 'clientName', 'description', 'actions'];

  constructor(
    private fb: FormBuilder,
    private projectService: ProjectMasterService,
    private clientService: ClientService
  ) {
    this.form = this.fb.group({
      projectName: ['', Validators.required],
      description: [''],
      clientId: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadProjects();
    this.loadClients();
  }

  // --- Data Loading ---
  loadProjects() {
    this.projectService.getAllProjects().subscribe({
      next: (data) => this.projects = data,
      error: (err) => console.error('Error loading projects', err)
    });
  }

  loadClients() {
    this.clientService.getAllClients().subscribe({
      next: (data) => this.clients = data,
      error: (err) => console.error('Error loading clients', err)
    });
  }

  // --- Actions ---
  addNew() {
    this.showForm = true;
    this.editingId = null;
    this.form.reset();
  }

  editProject(project: ProjectMaster) {
    this.showForm = true;
    this.editingId = project.id!;
    this.form.patchValue({
      projectName: project.projectName,
      description: project.description,
      clientId: project.clientId
    });
  }

  saveProject() {
    if (this.form.invalid) return;
    this.isSaving = true;

    const payload: ProjectMaster = {
      projectName: this.form.value.projectName,
      description: this.form.value.description,
      clientId: this.form.value.clientId
    };

    if (this.editingId) {
      // Update
      this.projectService.updateProject(this.editingId, payload).subscribe({
        next: () => {
          this.isSaving = false;
          this.closeForm();
          this.loadProjects();
        },
        error: (err) => {
          console.error(err);
          this.isSaving = false;
          alert('Failed to update project');
        }
      });
    } else {
      // Create
      this.projectService.createProject(payload).subscribe({
        next: () => {
          this.isSaving = false;
          this.closeForm();
          this.loadProjects();
        },
        error: (err) => {
          console.error(err);
          this.isSaving = false;
          alert('Failed to create project');
        }
      });
    }
  }

  deleteProject(id: number) {
    if (confirm('Are you sure you want to delete this project?')) {
      this.projectService.deleteProject(id).subscribe({
        next: () => this.loadProjects(),
        error: () => alert('Failed to delete project')
      });
    }
  }

  closeForm() {
    this.showForm = false;
    this.editingId = null;
    this.form.reset();
  }
}