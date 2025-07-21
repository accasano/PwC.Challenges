import { CommonModule } from '@angular/common';
import {
  Component,
  Input,
  OnInit,
} from '@angular/core';
import { FormsModule } from '@angular/forms';

import { CarRentalService } from '../services/car-rental.service';

@Component({
  selector: 'app-most-rented-type-rankings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  providers: [CarRentalService],
  templateUrl: './most-rented-type-rankings.component.html',
  styleUrls: ['./most-rented-type-rankings.component.css'],
})
export class MostRentedTypeRankingsComponent implements OnInit {
  selectedStartDate: string = '';
  selectedEndDate: string = '';
  @Input() title: string = '';
  @Input() startDate!: string;
  @Input() endDate!: string;

  rows: Array<{
    carType: string;
    rentalCount: number;
    utilizationPercentage: number;
  }> = [];

  constructor(private carRentalService: CarRentalService) {
  }
  
  getDefaultStartDate(): string {
    const date = new Date();
    date.setDate(date.getDate() - 7);
    return date.toISOString().slice(0, 10);
  }

  getDefaultEndDate(): string {
    return new Date().toISOString().slice(0, 10);
  }

  async ngOnInit() {
  }

  async onLoadClick() {
    await this.loadData();
  }

  private async loadData() {
    if (!this.selectedStartDate || !this.selectedEndDate) {
      this.rows = [];
      return;
    }
    this.rows = await this.carRentalService.getMostRentedCarRanking(
      this.selectedStartDate,
      this.selectedEndDate
    );
  }
}
