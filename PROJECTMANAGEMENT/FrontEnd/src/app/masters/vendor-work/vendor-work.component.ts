import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { VendorWorkService, VendorWork } from '../../services/vendor-work.service';
import { VendorService, Vendor } from '../../services/vendor.service';

@Component({
  selector: 'app-vendor-work',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe],
  templateUrl: './vendor-work.html',
  styleUrls: ['./vendor-work.css']
})
export class VendorWorkComponent implements OnInit {
  works: VendorWork[] = [];
  vendors: Vendor[] = [];

  form: VendorWork = {
    projectName: '',
    workDescription: '',
    startDate: '',
    endDate: '',
    status: 'Pending',
    vendorId: 0
  };

  showForm = false;
  editingId: number | null = null;

  constructor(
    private workService: VendorWorkService,
    private vendorService: VendorService
  ) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.workService.getAll().subscribe((data) => (this.works = data));
    this.vendorService.getAll().subscribe((data) => (this.vendors = data));
  }

  showAddForm() {
    this.resetForm();
    this.showForm = true;
  }

  handleChange(field: keyof VendorWork, value: any) {
    if (field === 'vendorId') {
      this.form.vendorId = Number(value);
      return;
    }
    // @ts-ignore
    this.form[field] = value;
  }

  handleSave() {
    if (!this.form.projectName || !this.form.vendorId || !this.form.startDate) {
      alert('Project Name, Vendor, and Start Date are required.');
      return;
    }

    const payload: Partial<VendorWork> = {
      projectName: this.form.projectName,
      workDescription: this.form.workDescription,
      startDate: this.form.startDate,
      endDate: this.form.endDate,
      status: this.form.status,
      vendorId: this.form.vendorId
    };

    if (this.editingId) {
      this.workService.update(this.editingId, payload).subscribe({
        next: () => {
          this.loadData();
          this.cancelForm();
        },
        error: () => alert('Update Failed')
      });
    } else {
      this.workService.create(payload).subscribe({
        next: () => {
          this.loadData();
          this.cancelForm();
        },
        error: () => alert('Create Failed')
      });
    }
  }

  handleEdit(id: number) {
    const work = this.works.find((w) => w.id === id);
    if (work) {
      this.form = {
        ...work,
        startDate: work.startDate?.split('T')[0] ?? '',
        endDate: work.endDate?.split('T')[0] ?? ''
      };
      this.editingId = id;
      this.showForm = true;
    }
  }

  handleDelete(id: number) {
    if (confirm('Delete this work entry?')) {
      this.workService.delete(id).subscribe(() => this.loadData());
    }
  }

  cancelForm() {
    this.showForm = false;
    this.resetForm();
  }

  resetForm() {
    this.form = {
      projectName: '',
      workDescription: '',
      startDate: '',
      endDate: '',
      status: 'Pending',
      vendorId: 0
    };
    this.editingId = null;
  }
}
