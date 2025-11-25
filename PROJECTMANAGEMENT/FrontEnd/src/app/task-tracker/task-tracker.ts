// src/app/task-tracker/task-tracker.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../services/task.service';
import { Task, CreateTaskPayload } from '../models/task.model';

@Component({
  selector: 'app-task-tracker',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './task-tracker.html',
  styleUrls: ['./task-tracker.css']
})
export class TaskTrackerComponent implements OnInit {
  tasks: Task[] = [];

  // Form model
  form: Task & { isEditing?: boolean } = this.getEmptyForm();
  showForm = false;

  loading = false;
  saving = false;

  constructor(private taskService: TaskService) { }

  ngOnInit(): void {
    this.loadTasks();
  }

  // --------- Helpers ---------
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
      progress: 0,
      isEditing: false
    };
  }

  formatDate(date?: string): string {
    if (!date) return '';
    const d = new Date(date);
    return isNaN(d.getTime()) ? '' : d.toISOString().split('T')[0];
  }

  // --------- Stats getters ---------
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

  // --------- CRUD ---------
  loadTasks() {
    this.loading = true;
    this.taskService.getTasks().subscribe({
      next: (data) => {
        this.tasks = data;
        this.loading = false;
      },
      error: (err) => {
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
    this.form = {
      ...task,
      isEditing: true
    };
  }

  onDelete(id?: number) {
    if (!id) return;
    if (!confirm('Delete this task?')) return;

    this.taskService.deleteTask(id).subscribe({
      next: () => this.loadTasks(),
      error: (err) => console.error('Failed to delete task', err)
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
      dueDate: this.form.dueDate || null
    };

    // Create or Update
    if (this.form.id) {
      this.taskService.updateTask(this.form.id, payload).subscribe({
        next: () => {
          this.saving = false;
          this.updateProgressIfNeeded(this.form.id!);
        },
        error: (err) => {
          console.error('Failed to update task', err);
          this.saving = false;
        }
      });
    } else {
      this.taskService.createTask(payload).subscribe({
        next: (res) => {
          this.saving = false;
          const newId = res.taskId;
          if (typeof this.form.progress === 'number') {
            this.updateProgressIfNeeded(newId);
          } else {
            this.afterSave();
          }
        },
        error: (err) => {
          console.error('Failed to create task', err);
          this.saving = false;
        }
      });
    }
  }

  private updateProgressIfNeeded(taskId: number) {
    if (this.form.progress == null) {
      this.afterSave();
      return;
    }

    this.taskService.updateProgress({
      taskId,
      progress: this.form.progress,
      remarks: ''
    }).subscribe({
      next: () => this.afterSave(),
      error: (err) => {
        console.error('Failed to update progress', err);
        this.afterSave();
      }
    });
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
