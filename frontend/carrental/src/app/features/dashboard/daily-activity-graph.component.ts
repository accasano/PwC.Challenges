import { CommonModule } from '@angular/common';
import {
  Component,
  Input,
  OnInit,
} from '@angular/core';

import { ChartModule } from 'primeng/chart';

import { CarRentalService } from '../services/car-rental.service';

@Component({
  selector: 'app-daily-activity-graph',
  standalone: true,
  imports: [CommonModule, ChartModule],
  templateUrl: './daily-activity-graph.component.html',
  providers: [CarRentalService]
})
export class DailyActivityGraphComponent implements OnInit {
  @Input() title: string = '';
  chartData: any;
  chartOptions: any;

  constructor(private carRentalService: CarRentalService) {}

  async ngOnInit() {
    const stats = await this.carRentalService.getDailyOperationalStats();
    this.chartData = {
      labels: ['Cancellations', 'Rentals', 'Unused Cars'],
      datasets: [
        {
          data: [stats.cancellations, stats.rentals, stats.unusedCars],
          backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56'],
          hoverBackgroundColor: ['#FF6384', '#36A2EB', '#FFCE56']
        }
      ]
    };
    this.chartOptions = {
      responsive: true,
      plugins: {
        legend: {
          position: 'top'
        }
      }
    };
  }
}