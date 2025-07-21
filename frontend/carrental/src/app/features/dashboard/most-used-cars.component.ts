import { CommonModule } from '@angular/common';
import {
  Component,
  Input,
  OnInit,
} from '@angular/core';

import { CarRentalService } from '../services/car-rental.service';

@Component({
  selector: 'app-most-used-cars',
  standalone: true,
  imports: [CommonModule],
  providers: [CarRentalService],
  templateUrl: './most-used-cars.component.html',
  styleUrls: ['./most-used-cars.component.css'],
})
export class MostUsedCarsComponent implements OnInit {
  @Input() title: string = '';

  rows: Array<{
    carId: string;
    model : string;
    ranking: number;
    rentalCount: number;
    totalRentalDays: number;
    utilizationPercentage: number;
  }> = [];

  constructor(private carRentalService: CarRentalService) {}

  async ngOnInit() {
    this.rows = await this.carRentalService.getMostUsedCars();
  }
}
