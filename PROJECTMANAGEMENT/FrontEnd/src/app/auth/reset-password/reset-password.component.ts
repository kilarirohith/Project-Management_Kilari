import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reset-password.html',
  styleUrls: ['./reset-password.css'],
})
export class ResetPasswordComponent implements OnInit {
  token = '';
  password = '';
  confirmPassword = '';
  message = '';
  error = '';
  loading = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private auth: AuthService
  ) {}

  ngOnInit(): void {
    this.token = this.route.snapshot.queryParamMap.get('token') || '';
    if (!this.token) {
      this.error = 'Invalid password reset link.';
    }
  }

  submit() {
    this.message = '';
    this.error = '';

    if (!this.password || !this.confirmPassword) {
      this.error = 'Please enter and confirm your new password.';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.error = 'Passwords do not match.';
      return;
    }

    if (!this.token) {
      this.error = 'Invalid or missing reset token.';
      return;
    }

    this.loading = true;

    this.auth.resetPassword(this.token, this.password).subscribe({
      next: (res: any) => {
        this.loading = false;
        this.message = res?.message || 'Password updated successfully.';

        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 1500);
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Failed to reset password.';
      },
    });
  }
}
