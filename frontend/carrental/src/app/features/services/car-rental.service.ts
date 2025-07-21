import {
  HttpClient,
  HttpHeaders,
} from '@angular/common/http';
import { Injectable } from '@angular/core';

import { firstValueFrom } from 'rxjs';

import { API_ENDPOINTS } from '../../core/api-endpoints';
import { AuthService } from '../auth/services/auth.service';

interface CarTypeStats {
  carType: string;
  rentalCount: number;
  utilizationPercentage: number;
}

interface UsedCarStats {
  carId: string;
  model: string;
  ranking: number;
  rentalCount: number;
  totalRentalDays: number;
  utilizationPercentage: number;
}

interface BrandModelTypeStats {
  ranking: number;
  model: string;
  type: string;
}

interface DailyOperationalStats {
  cancellations: number;
  rentals: number;
  unusedCars: number;
}

@Injectable()
export class CarRentalService {
  constructor(private http: HttpClient, private authService: AuthService) {}

  private getAuthHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
  }

  private async get<T>(url: string, params?: any): Promise<T> {
    return await firstValueFrom(
      this.http.get<T>(url, { headers: this.getAuthHeaders(), params })
    );
  }

  async getMostRentedCar(): Promise<CarTypeStats> {
    return this.get<CarTypeStats>(API_ENDPOINTS.most_rented_car);
  }

  async getMostRentedCarRanking(
    startDate?: string,
    endDate?: string
  ): Promise<CarTypeStats[]> {
    const params: any = {};
    if (startDate) params.startDate = startDate;
    if (endDate) params.endDate = endDate;

    return this.get<CarTypeStats[]>(API_ENDPOINTS.most_rented_car_ranking, params);
  }

  async getMostUsedCars(): Promise<UsedCarStats[]> {
    return this.get<UsedCarStats[]>(API_ENDPOINTS.most_used_cars);
  }

  async getMostUsedCarsByBrandModelType(): Promise<BrandModelTypeStats[]> {
    return this.get<BrandModelTypeStats[]>(
      API_ENDPOINTS.most_used_cars_by_brand_model_type
    );
  }

  async getDailyOperationalStats(): Promise<DailyOperationalStats> {
    return this.get<DailyOperationalStats>(
      API_ENDPOINTS.daily_operational_stats
    );
  }
}
