import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import {
  ApprovalDeskService,
  ApprovalDesk
} from '../services/approval-desk.service';

import { ProjectService } from '../services/project.service';
import { ProjectDTO } from '../models/project.model';

import {
  VendorWorkService,
  VendorWork
} from '../services/vendor-work.service';

@Component({
  selector: 'app-approval-desk',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './approval-desk.html',
  styleUrls: ['./approval-desk.css']
})
export class ApprovalDeskComponent implements OnInit {
  approvals: ApprovalDesk[] = [];

  // Dropdown Data
  projects: ProjectDTO[] = [];
  vendorWorks: VendorWork[] = [];

  // Form State
  showForm = false;
  editingId: number | null = null;

  form: {
    projectId: number;
    vendorWorkId: number;
    status: string;
    remarks: string;
  } = {
    projectId: 0,
    vendorWorkId: 0,
    status: 'Pending',
    remarks: ''
  };

  constructor(
    private approvalService: ApprovalDeskService,
    private projectService: ProjectService,
    private vendorWorkService: VendorWorkService
  ) {}

  ngOnInit() {
    this.loadData();
    this.loadDropdowns();
  }

  // -------- API → UI --------

  loadData() {
    // GET /api/ApprovalDesk
    this.approvalService.getAll().subscribe({
      next: (data) => (this.approvals = data),
      error: (err) => console.error('Error loading approvals', err)
    });
  }

  loadDropdowns() {
    // Projects dropdown: GET /api/Project
    this.projectService.getProjects().subscribe({
      next: (data) => (this.projects = data),
      error: (err) => console.error('Error loading projects', err)
    });

    // VendorWork dropdown: GET /api/VendorWork
    this.vendorWorkService.getAll().subscribe({
      next: (data) => (this.vendorWorks = data),
      error: (err) => console.error('Error loading vendor works', err)
    });
  }

  // -------- Form logic --------

  openForm() {
    this.resetForm();
    this.showForm = true;
  }

  editItem(item: ApprovalDesk) {
    this.form = {
      projectId: item.projectId,
      vendorWorkId: item.vendorWorkId,
      status: item.status,
      remarks: item.remarks
    };
    this.editingId = item.id!;
    this.showForm = true;
  }

  save() {
    if (!this.form.projectId || !this.form.vendorWorkId) {
      alert('Project and Vendor Work are required');
      return;
    }

    const payload = {
      projectId: Number(this.form.projectId),
      vendorWorkId: Number(this.form.vendorWorkId),
      status: this.form.status,
      remarks: this.form.remarks
    };

    if (this.editingId) {
      // PUT /api/ApprovalDesk/{id}
      this.approvalService.update(this.editingId, payload).subscribe({
        next: () => {
          this.loadData();
          this.closeForm();
        },
        error: () => alert('Update failed')
      });
    } else {
      // POST /api/ApprovalDesk
      this.approvalService.create(payload).subscribe({
        next: () => {
          this.loadData();
          this.closeForm();
        },
        error: () => alert('Creation failed')
      });
    }
  }

  deleteItem(id: number) {
    if (!confirm('Are you sure you want to delete this approval record?')) return;

    // DELETE /api/ApprovalDesk/{id}
    this.approvalService.delete(id).subscribe({
      next: () => this.loadData(),
      error: () => alert('Delete failed')
    });
  }

  closeForm() {
    this.showForm = false;
    this.resetForm();
  }

  resetForm() {
    this.editingId = null;
    this.form = {
      projectId: 0,
      vendorWorkId: 0,
      status: 'Pending',
      remarks: ''
    };
  }
}
