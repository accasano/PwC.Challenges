import { CommonModule } from '@angular/common';
import {
  Component,
  Input,
  OnInit,
} from '@angular/core';

import { CarRentalService } from '../services/car-rental.service';

@Component({
  selector: 'app-most-rented-type',
  standalone: true,
  imports: [CommonModule],
  providers: [CarRentalService],
  templateUrl: './most-rented-type.component.html',
  styleUrls: ['./most-rented-type.component.css'],
})
export class MostRentedTypeComponent implements OnInit {
  @Input() title: string = '';

  rows: Array<{
    carType: string;
    rentalCount: number;
    utilizationPercentage: number;
  }> = [];

  constructor(private carRentalService: CarRentalService) {}

  async ngOnInit() {
    const data = await this.carRentalService.getMostRentedCar();
    this.rows = [data];
  }
}
