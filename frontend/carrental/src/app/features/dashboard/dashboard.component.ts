import { CommonModule } from '@angular/common';
import {
  Component,
  OnInit,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService } from '../auth/services/auth.service';
import { DailyActivityGraphComponent } from './daily-activity-graph.component';
import {
  MostRentedTypeRankingsComponent,
} from './most-rented-type-rankings.component';
import { MostRentedTypeComponent } from './most-rented-type.component';
import {
  MostUsedCarsByDetailsComponent,
} from './most-used-cars-by-details.component';
import { MostUsedCarsComponent } from './most-used-cars.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, MostRentedTypeComponent, MostRentedTypeRankingsComponent, MostUsedCarsComponent, MostUsedCarsByDetailsComponent, DailyActivityGraphComponent],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent implements OnInit {
  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit() {
  }

  logout() {
    this.authService.logout();
  }
}
