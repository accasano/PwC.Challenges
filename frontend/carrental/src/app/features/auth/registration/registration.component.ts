import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-registration',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css'],
})
export class RegistrationComponent {
  email = '';
  password = '';
  registerFailed = false;

  constructor(private authService: AuthService, private router: Router) {}

  async onRegister() {
    const success = await this.authService.register(this.email, this.password);
    this.registerFailed = !success;
    if (success) {
      this.router.navigate(['/dashboard']);
    }
  }
}
