import { environment } from '../../environments/environment';

const BASE_URL = environment.BASE_URL;

export const API_ENDPOINTS = {
  login: `${BASE_URL}/api/auth/login`,
  register: `${BASE_URL}/api/auth/register`,
  most_rented_car: `${BASE_URL}/api/statistics/car-types/most-rented`,
  most_rented_car_ranking: `${BASE_URL}/api/statistics/car-types/most-rented/ranking`,
  most_used_cars: `${BASE_URL}/api/statistics/cars/most-used`,
  most_used_cars_by_brand_model_type: `${BASE_URL}/api/statistics/cars/most-used/by-details`,
  daily_operational_stats: `${BASE_URL}/api/statistics/daily-activity`,
};
