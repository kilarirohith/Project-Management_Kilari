import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

import { ProjectService } from '../services/project.service';
import {
  ProjectDTO,
  DashboardProject,
  ProjectStatus,
  ProjectType,
  Milestone
} from '../models/project.model';

import {
  ClientService,
  Client,
  Location,
  Unit
} from '../services/client.service';

import { MilestoneMasterService } from '../services/milestone-master.service';
import { ProjectMasterService } from '../services/projectMaster.service';

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './projects.html',
  styleUrls: ['./projects.css']
})
export class ProjectsComponent implements OnInit {

  // Summary
  totalProjects = 0;
  running = 0;
  completed = 0;
  delayed = 0;
  onHold = 0;

  activeTab: string = 'All';

  // Modal / form
  isModalOpen = false;
  isSubmitting = false;
  isEditing = false;
  editingId: number | null = null;
  projectForm!: FormGroup;

  // Dropdown data from backend
  clients: Client[] = [];
  projectTypes: string[] = [];    // from ProjectMaster
  milestones: string[] = [];      // from MilestoneMaster
  locations: string[] = [];       // filtered by client
  units: string[] = [];           // filtered by location

  // Projects for table
  projects: DashboardProject[] = [];

  constructor(
    private fb: FormBuilder,
    private projectService: ProjectService,
    private clientService: ClientService,
    private milestoneService: MilestoneMasterService,
    private projectMasterService: ProjectMasterService
  ) {
    this.initForm();
  }

  ngOnInit(): void {
    this.loadClients();
    this.loadMilestones();
    this.loadProjectTypes();
    this.loadProjects();
  }

  // --- Form ---

  initForm() {
    this.projectForm = this.fb.group({
      project: ['', Validators.required],
      projectType: ['', Validators.required],
      clientId: ['', Validators.required],
      location: ['', Validators.required],
      unit: ['', Validators.required],
      milestone: ['', Validators.required],
      planStart: ['', Validators.required],
      planClose: ['', Validators.required]
    });
  }

  get f() {
    return this.projectForm.controls;
  }

  // --- Load masters from backend ---

  loadClients() {
    this.clientService.getAllClients().subscribe({
      next: (data) => {
        this.clients = data;        // 👈 let TypeScript infer Client[]
      },
      error: (err) => console.error('Error loading clients', err)
    });
  }

  loadMilestones() {
    this.milestoneService.getAll().subscribe({
      next: (data) => {
        this.milestones = (data as Milestone[]).map(m => m.name);
      },
      error: (err) => console.error('Error loading milestones', err)
    });
  }

  loadProjectTypes() {
    this.projectMasterService.getAllProjects().subscribe({
      next: (data) => {
        this.projectTypes = (data as ProjectType[]).map(p => p.projectName);
      },
      error: (err) => console.error('Error loading project master', err)
    });
  }

  loadProjects() {
    this.projectService.getProjects().subscribe({
      next: (data: ProjectDTO[]) => {
        this.projects = data.map(p => this.mapToDashboardProject(p));
        this.updateStats();
      },
      error: (err) => console.error('Error loading projects', err)
    });
  }

  private mapToDashboardProject(p: ProjectDTO): DashboardProject {
    const status = this.mapStatus(p.status);

    return {
      id: p.id,
      code: p.projectCode || `PRJ-${p.id}`,
      name: p.projectName,
      type: p.projectType || '-',
      client: p.clientName || '',
      location: p.clientLocation || '-',
      unit: p.unit || '-',
      milestone: p.milestone || '-',
      planStart: p.planStartDate ? p.planStartDate.substring(0, 10) : '-',
      planEnd: p.planEndDate ? p.planEndDate.substring(0, 10) : '-',
      actualStart: p.actualStartDate ? p.actualStartDate.substring(0, 10) : '-',
      status
    };
  }

  private mapStatus(status: string): ProjectStatus {
    switch (status) {
      case 'Running':
      case 'Completed':
      case 'Delayed':
      case 'OnHold':
      case 'Pending':
        return status as ProjectStatus;
      default:
        return 'Pending';
    }
  }

  private updateStats() {
    this.totalProjects = this.projects.length;
    this.running   = this.projects.filter(p => p.status === 'Running').length;
    this.completed = this.projects.filter(p => p.status === 'Completed').length;
    this.delayed   = this.projects.filter(p => p.status === 'Delayed').length;
    this.onHold    = this.projects.filter(p => p.status === 'OnHold').length;
  }

  // --- Dependent dropdowns ---

  onClientChange() {
    const clientId = Number(this.projectForm.value.clientId);
    const client = this.clients.find(c => c.id === clientId);

    if (!client) {
      this.locations = [];
      this.units = [];
      this.projectForm.patchValue({ location: '', unit: '' });
      return;
    }

    const locNames = new Set<string>();
    client.locations.forEach((l: Location) => {
      if (l.locationName) locNames.add(l.locationName);
    });

    this.locations = Array.from(locNames);
    this.units = [];
    this.projectForm.patchValue({ location: '', unit: '' });
  }

  onLocationChange() {
    const clientId = Number(this.projectForm.value.clientId);
    const selectedLocation = this.projectForm.value.location as string;
    const client = this.clients.find(c => c.id === clientId);

    if (!client) {
      this.units = [];
      this.projectForm.patchValue({ unit: '' });
      return;
    }

    const unitsSet = new Set<string>();
    client.locations
      .filter((l: Location) => l.locationName === selectedLocation)
      .forEach((l: Location) => {
        l.units?.forEach((u: Unit) => {
          if (u.unitName) unitsSet.add(u.unitName);
        });
      });

    this.units = Array.from(unitsSet);
    this.projectForm.patchValue({ unit: '' });
  }

  // --- Modal / CRUD ---

  openModal(project?: DashboardProject) {
    this.isModalOpen = true;
    this.isSubmitting = false;

    if (project) {
      this.isEditing = true;
      this.editingId = project.id;

      const client = this.clients.find(c => c.clientName === project.client);

      this.projectForm.patchValue({
        project: project.name,
        projectType: project.type !== '-' ? project.type : '',
        clientId: client?.id ?? '',
        location: project.location !== '-' ? project.location : '',
        unit: project.unit !== '-' ? project.unit : '',
        milestone: project.milestone !== '-' ? project.milestone : '',
        planStart: project.planStart !== '-' ? project.planStart : '',
        planClose: project.planEnd !== '-' ? project.planEnd : ''
      });

      if (client) {
        this.onClientChange();
        if (project.location !== '-') {
          this.onLocationChange();
        }
      }
    } else {
      this.isEditing = false;
      this.editingId = null;
      this.projectForm.reset();
    }
  }

  onCancel() {
    this.isModalOpen = false;
    this.isSubmitting = false;
    this.isEditing = false;
    this.editingId = null;
    this.projectForm.reset();
  }

  onSubmit() {
    if (this.projectForm.invalid) {
      this.projectForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formData = this.projectForm.value;

    const dto: Partial<ProjectDTO> = {
      projectName: formData.project,
      projectType: formData.projectType,
      clientId: Number(formData.clientId),
      clientLocation: formData.location,
      unit: formData.unit,
      milestone: formData.milestone,
      planStartDate: formData.planStart,
      planEndDate: formData.planClose,
      status: 'Running'
    };

    if (this.isEditing && this.editingId) {
      this.projectService.updateProject(this.editingId, dto).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.onCancel();
          this.loadProjects();
        },
        error: (err) => {
          console.error(err);
          this.isSubmitting = false;
          alert('Failed to update project');
        }
      });
    } else {
      this.projectService.createProject(dto).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.onCancel();
          this.loadProjects();
        },
        error: (err) => {
          console.error(err);
          this.isSubmitting = false;
          alert('Failed to create project');
        }
      });
    }
  }

  deleteProject(project: DashboardProject) {
    const id = project.id;
    if (!id) return;
    if (!confirm('Delete this project?')) return;

    this.projectService.deleteProject(id).subscribe({
      next: () => this.loadProjects(),
      error: () => alert('Failed to delete project')
    });
  }

  // --- Tabs / Filters ---

  setTab(tab: string) {
    this.activeTab = tab;
  }

  get filteredProjects() {
    switch (this.activeTab) {
      case 'Project Delayed':
      case 'Milestone Delayed':
        return this.projects.filter(p => p.status === 'Delayed');
      case 'On Hold':
        return this.projects.filter(p => p.status === 'OnHold');
      case 'Running':
        return this.projects.filter(p => p.status === 'Running');
      case 'Completed':
        return this.projects.filter(p => p.status === 'Completed');
      default:
        return this.projects;
    }
  }
}
