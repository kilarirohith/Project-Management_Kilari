// src/app/projects/projects.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators
} from '@angular/forms';

import { ProjectService } from '../services/project.service';
import {
  ProjectDTO,
  DashboardProject,
  ProjectStatus,
  ProjectType,
  Milestone,
  CreateProjectPayload,    // 👈 add this
  UpdateProjectPayload     // 👈 and this
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

  // summary
  totalProjects = 0;
  running = 0;
  completed = 0;
  delayed = 0;
  onHold = 0;

  activeTab: string = 'All';

  // modal / form
  isModalOpen = false;
  isSubmitting = false;
  isEditing = false;
  editingId: number | null = null;
  projectForm!: FormGroup;

  // dropdown data
  clients: Client[] = [];
  projectTypes: string[] = [];
  milestones: string[] = [];
  locations: string[] = [];
  units: string[] = [];
  statusOptions: ProjectStatus[] = ['Running', 'Completed', 'Delayed', 'OnHold'];

  // table data
  projects: DashboardProject[] = [];

  // pagination
  pageSizeOptions = [10, 25, 50, 100];
  pageSize = 10;
  pageIndex = 0;

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

  // ---------------- FORM ----------------

  initForm() {
    this.projectForm = this.fb.group({
      projectCode: [''],                       // only used in edit
      project: ['', Validators.required],
      projectType: ['', Validators.required],
      clientId: ['', Validators.required],
      location: ['', Validators.required],
      unit: ['', Validators.required],
      milestone: ['', Validators.required],
      planStart: ['', Validators.required],
      planClose: ['', Validators.required],
      actualStart: [''],
      actualClose: [''],
      status: ['Running', Validators.required] // 👈 manual status
    });
  }

  get f() {
    return this.projectForm.controls;
  }

  // ------------- LOAD MASTER DATA -------------

  loadClients() {
    this.clientService.getAllClients().subscribe({
      next: (data) => this.clients = data,
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
      error: (err) => console.error('Error loading project types', err)
    });
  }

  loadProjects() {
    this.projectService.getProjects().subscribe({
      next: (data: ProjectDTO[]) => {
        this.projects = data.map(p => this.mapToDashboardProject(p));
        this.updateStats();
        this.pageIndex = 0;
      },
      error: (err) => console.error('Error loading projects', err)
    });
  }

  // ------------- MAP + HELPERS -------------

  private mapToDashboardProject(p: ProjectDTO): DashboardProject {
    const status = this.mapStatus(p.status);
    const planStart = p.planStartDate ? p.planStartDate.substring(0, 10) : '-';
    const planEnd = p.planEndDate ? p.planEndDate.substring(0, 10) : '-';
    const actualStart = p.actualStartDate ? p.actualStartDate.substring(0, 10) : '-';
    const actualEnd = p.actualEndDate ? p.actualEndDate.substring(0, 10) : '-';

    const elapsedDays = this.calculateElapsedDays(planStart, actualEnd, status);

    return {
      id: p.id,
      code: p.projectCode || `PRJ-${p.id}`,
      name: p.projectName,
      type: p.projectType || '-',
      client: p.clientName || '',
      location: p.clientLocation || '-',
      unit: p.unit || '-',
      milestone: p.milestone || '-',
      planStart,
      planEnd,
      actualStart,
      actualEnd,
      elapsedDays,
      status
    };
  }

  private calculateElapsedDays(
    planStart: string,
    actualEnd: string,
    status: ProjectStatus
  ): number | '-' {
    if (!planStart || planStart === '-') return '-';

    const start = new Date(planStart);
    let end: Date;

    if (status === 'Completed' && actualEnd !== '-' && actualEnd) {
      end = new Date(actualEnd);
    } else {
      end = new Date(); // till today
    }

    const diffMs = end.getTime() - start.getTime();
    const days = Math.floor(diffMs / (1000 * 60 * 60 * 24));
    return days >= 0 ? days : 0;
  }

  private mapStatus(status: string): ProjectStatus {
    switch (status) {
      case 'Running':
      case 'Completed':
      case 'Delayed':
      case 'OnHold':
        return status as ProjectStatus;
      default:
        return 'Running';
    }
  }

  private updateStats() {
    this.totalProjects = this.projects.length;
    this.running   = this.projects.filter(p => p.status === 'Running').length;
    this.completed = this.projects.filter(p => p.status === 'Completed').length;
    this.delayed   = this.projects.filter(p => p.status === 'Delayed').length;
    this.onHold    = this.projects.filter(p => p.status === 'OnHold').length;
  }

  // --------- DEPENDENT DROPDOWNS ----------

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

  // ------------- MODAL / CRUD -------------

  openModal(project?: DashboardProject) {
    this.isModalOpen = true;
    this.isSubmitting = false;

    if (project) {
      this.isEditing = true;
      this.editingId = project.id;

      const client = this.clients.find(c => c.clientName === project.client);

      this.projectForm.patchValue({
        projectCode: project.code,
        project: project.name,
        projectType: project.type !== '-' ? project.type : '',
        clientId: client?.id ?? '',
        location: project.location !== '-' ? project.location : '',
        unit: project.unit !== '-' ? project.unit : '',
        milestone: project.milestone !== '-' ? project.milestone : '',
        planStart: project.planStart !== '-' ? project.planStart : '',
        planClose: project.planEnd !== '-' ? project.planEnd : '',
        actualStart: project.actualStart !== '-' ? project.actualStart : '',
        actualClose: project.actualEnd !== '-' ? project.actualEnd : '',
        status: project.status             // 👈 load existing status
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
      this.projectForm.reset({
        status: 'Running'                 // 👈 default on new
      });
    }
  }

  onCancel() {
    this.isModalOpen = false;
    this.isSubmitting = false;
    this.isEditing = false;
    this.editingId = null;
    this.projectForm.reset({
      status: 'Running'
    });
  }

  onSubmit() {
  if (this.projectForm.invalid) {
    this.projectForm.markAllAsTouched();
    return;
  }

  this.isSubmitting = true;
  const formData = this.projectForm.value;

  // 👇 base payload for both create & update
  const basePayload: CreateProjectPayload = {
    projectCode: formData.projectCode || '',
    projectName: formData.project,
    projectType: formData.projectType,
    clientId: Number(formData.clientId),          // ✅ required in payload type
    clientLocation: formData.location || null,
    unit: formData.unit || null,
    milestone: formData.milestone || null,
    planStartDate: formData.planStart,
    planEndDate: formData.planClose,
    status: formData.status as ProjectStatus      // ✅ from dropdown
  };

  if (this.isEditing && this.editingId) {
    const updatePayload: UpdateProjectPayload = {
      ...basePayload,
      actualStartDate: formData.actualStart || null,
      actualEndDate: formData.actualClose || null,
      elapsedDays: undefined   // or compute if you want
    };

    this.projectService.updateProject(this.editingId, updatePayload).subscribe({
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
    this.projectService.createProject(basePayload).subscribe({
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

  // ------------- TABS / FILTERS -------------

  setTab(tab: string) {
    this.activeTab = tab;
    this.pageIndex = 0;
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

  // ------------- PAGINATION -------------

  get paginatedProjects(): DashboardProject[] {
    const start = this.pageIndex * this.pageSize;
    const end = start + this.pageSize;
    return this.filteredProjects.slice(start, end);
  }

  get totalFiltered(): number {
    return this.filteredProjects.length;
  }

  get totalPages(): number {
    return this.totalFiltered === 0
      ? 1
      : Math.ceil(this.totalFiltered / this.pageSize);
  }

  get showingFrom(): number {
    if (this.totalFiltered === 0) return 0;
    return this.pageIndex * this.pageSize + 1;
  }

  get showingTo(): number {
    if (this.totalFiltered === 0) return 0;
    return Math.min(this.totalFiltered, (this.pageIndex + 1) * this.pageSize);
  }

  onPageSizeChange(event: Event) {
    const value = (event.target as HTMLSelectElement).value;
    this.pageSize = Number(value);
    this.pageIndex = 0;
  }

  nextPage() {
    if (this.pageIndex + 1 < this.totalPages) {
      this.pageIndex++;
    }
  }

  prevPage() {
    if (this.pageIndex > 0) {
      this.pageIndex--;
    }
  }
}
