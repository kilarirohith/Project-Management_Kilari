import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { VendorService, Vendor } from '../../services/vendor.service';

@Component({
  selector: 'app-vendor-master',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './vendor-master.html',
  styleUrls: ['./vendor-master.css']
})
export class VendorMasterComponent implements OnInit {
  vendors: Vendor[] = [];

  // Form State
  form: Vendor = {
    vendorName: '',
    vendorLocation: '',
    vendorGst: '',
    email: '',
    spoc: ''
  };

  showForm = false;
  editingId: number | null = null;

  constructor(private vendorService: VendorService) {}

  ngOnInit() {
    this.loadVendors();
  }

  loadVendors() {
    this.vendorService.getAll().subscribe({
      next: (data) => (this.vendors = data),
      error: (err) => console.error('Error loading vendors', err)
    });
  }

  showAddForm() {
    this.resetForm();
    this.editingId = null;
    this.showForm = true;
  }

  handleSave() {
    if (!this.form.vendorName.trim()) {
      alert('Vendor Name is required');
      return;
    }

    if (this.editingId) {
      // UPDATE
      this.vendorService.update(this.editingId, this.form).subscribe({
        next: () => {
          this.loadVendors();
          this.cancelForm();
        },
        error: () => alert('Update Failed')
      });
    } else {
      // CREATE
      this.vendorService.create(this.form).subscribe({
        next: () => {
          this.loadVendors();
          this.cancelForm();
        },
        error: () => alert('Create Failed')
      });
    }
  }

  handleEdit(id: number) {
    const vendor = this.vendors.find((v) => v.id === id);
    if (vendor) {
      this.form = { ...vendor };
      this.editingId = id;
      this.showForm = true;
    }
  }

  handleDelete(id: number) {
    if (!confirm('Delete this vendor?')) return;

    this.vendorService.delete(id).subscribe({
      next: () => this.loadVendors(),
      error: () => alert('Delete Failed')
    });
  }

  resetForm() {
    this.form = {
      vendorName: '',
      vendorLocation: '',
      vendorGst: '',
      email: '',
      spoc: ''
    };
  }

  cancelForm() {
    this.showForm = false;
    this.resetForm();
    this.editingId = null;
  }
}
