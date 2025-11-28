import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './forgot-password.html',
  styleUrls: ['./forgot-password.css'],
})
export class ForgotPasswordComponent {
  email = '';
  message = '';
  error = '';
  loading = false;

  constructor(private auth: AuthService) {}

  submit() {
    this.message = '';
    this.error = '';
    if (!this.email.trim()) {
      this.error = 'Please enter your email';
      return;
    }

    this.loading = true;
    this.auth.forgotPassword(this.email).subscribe({
      next: (res: any) => {
        this.loading = false;
        this.message = res?.message || 'If email exists, reset link sent';
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Failed to send reset email';
      },
    });
  }
}
