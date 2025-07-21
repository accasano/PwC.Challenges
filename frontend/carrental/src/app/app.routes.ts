import { Routes } from '@angular/router';

import { AuthGuard } from './features/auth/auth.guard';
import { LoginComponent } from './features/auth/login/login.component';
import {
  RegistrationComponent,
} from './features/auth/registration/registration.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';

export const routes: Routes = [
  { path: '', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
  { path: 'register', component: RegistrationComponent },
  { path: '**', redirectTo: '' }
];
