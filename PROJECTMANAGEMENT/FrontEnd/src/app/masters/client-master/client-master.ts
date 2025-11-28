import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClientService, Client, Location } from '../../services/client.service';

@Component({
  selector: 'app-client-master',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './client-master.html',
  styleUrls: ['./client-master.css']
})
export class ClientMasterComponent implements OnInit {
  clients: Client[] = [];
  
  // Default Form State
  form: Client = {
    clientName: "",
    gst: "",
    email: "",
    locations: [{ locationName: "", spoc: "", units: [{ unitName: "" }] }],
  };
  
  showForm = false;
  deleteId: number | null = null; // Changed to number

  constructor(private clientService: ClientService) {}

  ngOnInit() {
    this.loadClients();
  }

  loadClients() {
    this.clientService.getAllClients().subscribe({
      next: (data) => this.clients = data,
      error: (err) => console.error('Error loading clients', err)
    });
  }

  // --- Form Logic ---

  handleChange(field: keyof Client, value: string) {
    // @ts-ignore
    this.form[field] = value;
  }

  handleLocationChange(index: number, field: 'locationName' | 'spoc', value: string) {
    this.form.locations[index][field] = value;
  }

  handleUnitChange(locIndex: number, unitIndex: number, value: string) {
    this.form.locations[locIndex].units[unitIndex].unitName = value;
  }

  addLocation() {
    this.form.locations.push({ locationName: "", spoc: "", units: [{ unitName: "" }] });
  }

  addUnit(locIndex: number) {
    this.form.locations[locIndex].units.push({ unitName: "" });
  }

  // --- API Actions ---

  handleSave() {
    if (!this.form.clientName.trim() || !this.form.email.trim()) {
      alert("Client Name and Email are required");
      return;
    }

    // Clean data structure matches backend DTO automatically based on interface
    const payload: Client = {
      ...this.form,
      locations: this.form.locations
    };

    if (this.form.id) {
      // Update
      this.clientService.updateClient(this.form.id, payload).subscribe({
        next: () => {
          alert('Client Updated');
          this.cancelForm();
          this.loadClients();
        },
        error: (err) => alert('Update Failed')
      });
    } else {
      // Create
      this.clientService.createClient(payload).subscribe({
        next: () => {
          alert('Client Created');
          this.cancelForm();
          this.loadClients();
        },
        error: (err) => alert('Create Failed')
      });
    }
  }

  handleEdit(id: number) { // Changed ID to number
    this.clientService.getClientById(id).subscribe({
      next: (data) => {
        // Deep copy to avoid editing table directly
        this.form = JSON.parse(JSON.stringify(data)); 
        this.showForm = true;
      },
      error: (err) => alert('Could not fetch details')
    });
  }

  setDeleteId(id: number) { // Changed ID to number
    this.deleteId = id;
  }

confirmDelete() {
  if (this.deleteId == null) return;   // use == null to catch null/undefined
  
  this.clientService.deleteClient(this.deleteId).subscribe({
    next: () => {
      this.deleteId = null;
      this.loadClients();
    },
    error: (err) => {
      if (err.status === 403) {
        alert('You do not have permission to delete clients.');
      } else {
        alert('Delete Failed');
      }
      this.deleteId = null;
    }
  });
}


  // --- Helpers ---

  cancelForm() {
    this.showForm = false;
    this.resetForm();
  }

  resetForm() {
    this.form = {
      clientName: "",
      gst: "",
      email: "",
      locations: [{ locationName: "", spoc: "", units: [{ unitName: "" }] }],
    };
  }

  getSortedClients(): Client[] {
    return this.clients; // Sorting can be done in backend or here
  }

  getLocationNames(locations: Location[]): string {
    return locations.map(l => l.locationName).join(", ");
  }

  getSPOCs(locations: Location[]): string {
    return locations.map(l => l.spoc).join(", ");
  }

  getUnitNames(locations: Location[]): string {
    return locations
      .map(l => l.units?.map(u => u.unitName).join(", "))
      .filter(u => u)
      .join("; ");
  }
}