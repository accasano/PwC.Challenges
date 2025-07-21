import { CommonModule } from '@angular/common';
import {
  Component,
  Input,
  OnInit,
} from '@angular/core';

import { CarRentalService } from '../services/car-rental.service';

@Component({
  selector: 'app-most-used-cars-by-details',
  standalone: true,
  imports: [CommonModule],
  providers: [CarRentalService],
  templateUrl: './most-used-cars-by-details.component.html',
  styleUrls: ['./most-used-cars-by-details.component.css'],
})
export class MostUsedCarsByDetailsComponent implements OnInit {
  @Input() title: string = '';

  rows: Array<{
    ranking: number;
    model : string;
    type: string;
  }> = [];

  constructor(private carRentalService: CarRentalService) {}

  async ngOnInit() {
    this.rows = await this.carRentalService.getMostUsedCarsByBrandModelType();
  }
}
