import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { firstValueFrom } from 'rxjs';

import { API_ENDPOINTS } from '../../../core/api-endpoints';

@Injectable()
export class AuthService {
  private token: string | null = null;

  constructor(private http: HttpClient) {}

  async login(email: string, password: string): Promise<boolean> {
    try {
      const response = await firstValueFrom(
        this.http.post<{ token: string }>(
          API_ENDPOINTS.login,
          {},
          { params: { email, password } }
        )
      );
      this.token = response.token;
      localStorage.setItem('auth_token', this.token);
      return !!this.token;
    } catch (error) {
      this.token = null;
      return false;
    }
  }

  getToken(): string | null {
    return this.token || localStorage.getItem('auth_token');
  }

  logout() {
    this.token = null;
    localStorage.removeItem('auth_token');
  }

  async register(email: string, password: string): Promise<boolean> {
    try {
      const response = await firstValueFrom(
        this.http.post(
          API_ENDPOINTS.register,
          {},
          { params: { email, password } }
        )
      );
      return true;
    } catch (error) {
      return false;
    }
  }
}
