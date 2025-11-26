// src/app/task-tracker/task-tracker.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { TaskService } from '../services/task.service';
import { Task, CreateTaskPayload } from '../models/task.model';

import { ProjectDTO } from '../models/project.model';
import { ProjectService } from '../services/project.service';
import { UserService, SimpleUser } from '../services/user.service';

@Component({
  selector: 'app-task-tracker',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './task-tracker.html',
  styleUrls: ['./task-tracker.css']
})
export class TaskTrackerComponent implements OnInit {
  tasks: Task[] = [];
  projects: ProjectDTO[] = [];
  users: SimpleUser[] = [];

  form: Task & { isEditing?: boolean } = this.getEmptyForm();
  showForm = false;

  loading = false;
  saving = false;

  constructor(
    private taskService: TaskService,
    private projectService: ProjectService,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    this.loadTasks();
    this.loadProjects();
    this.loadUsers();
  }

  private getEmptyForm(): Task & { isEditing?: boolean } {
    return {
      id: undefined,
      title: '',
      description: '',
      status: 'Open',
      priority: 'Normal',
      projectId: 0,
      projectName: '',
      assignedToUserId: null,
      assignedUserName: '',
      createdAt: '',
      dueDate: '',
      type: '',
      produceStep: '',
      sampleData: '',
      acceptanceCriteria: '',
      actualClosureDate: null,
      testingStatus: '',
      testingDoneBy: '',
      isEditing: false
    };
  }

  formatDate(date?: string | null): string {
    if (!date) return '';
    const d = new Date(date);
    return isNaN(d.getTime()) ? '' : d.toISOString().split('T')[0];
  }

  // Stats
  get totalTasks(): number {
    return this.tasks.length;
  }
  get openTasks(): number {
    return this.tasks.filter(t => t.status === 'Open').length;
  }
  get inProgressTasks(): number {
    return this.tasks.filter(t => t.status === 'In Progress').length;
  }
  get closedTasks(): number {
    return this.tasks.filter(t => t.status === 'Closed').length;
  }

  // Load dropdowns
  private loadProjects() {
    this.projectService.getProjects().subscribe({
      next: data => (this.projects = data),
      error: err => console.error('Failed to load projects', err)
    });
  }

  private loadUsers() {
    this.userService.getSimpleUsers().subscribe({
      next: data => (this.users = data),
      error: err => console.error('Failed to load users', err)
    });
  }

  // CRUD
  loadTasks() {
    this.loading = true;
    this.taskService.getTasks().subscribe({
      next: data => {
        this.tasks = data;
        this.loading = false;
      },
      error: err => {
        console.error('Failed to load tasks', err);
        this.loading = false;
      }
    });
  }

  onAddNew() {
    this.showForm = true;
    this.form = this.getEmptyForm();
  }

  onEdit(task: Task) {
    this.showForm = true;
    this.form = { ...task, isEditing: true };
  }

  onDelete(id?: number) {
    if (!id) return;
    if (!confirm('Delete this task?')) return;

    this.taskService.deleteTask(id).subscribe({
      next: () => this.loadTasks(),
      error: err => console.error('Failed to delete task', err)
    });
  }

  onFieldChange(field: keyof Task, value: any) {
    this.form = { ...this.form, [field]: value };
  }

  onSave() {
    if (!this.form.title.trim()) return;

    this.saving = true;

    const payload: CreateTaskPayload = {
      title: this.form.title,
      description: this.form.description,
      status: this.form.status,
      priority: this.form.priority,
      projectId: this.form.projectId || 0,
      assignedToUserId: this.form.assignedToUserId ?? null,
      dueDate: this.form.dueDate || null,

      type: this.form.type || null,
      produceStep: this.form.produceStep || null,
      sampleData: this.form.sampleData || null,
      acceptanceCriteria: this.form.acceptanceCriteria || null,
      actualClosureDate: this.form.actualClosureDate || null,
      testingStatus: this.form.testingStatus || null,
      testingDoneBy: this.form.testingDoneBy || null
    };

    if (this.form.id) {
      this.taskService.updateTask(this.form.id, payload).subscribe({
        next: () => {
          this.saving = false;
          this.afterSave();
        },
        error: err => {
          console.error('Failed to update task', err);
          this.saving = false;
        }
      });
    } else {
      this.taskService.createTask(payload).subscribe({
        next: () => {
          this.saving = false;
          this.afterSave();
        },
        error: err => {
          console.error('Failed to create task', err);
          this.saving = false;
        }
      });
    }
  }

  private afterSave() {
    this.showForm = false;
    this.form = this.getEmptyForm();
    this.loadTasks();
  }

  onCancel() {
    this.showForm = false;
    this.form = this.getEmptyForm();
  }
}
