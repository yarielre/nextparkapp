import { BaseModel } from './base.model';

export class Parking extends BaseModel {
  address: string;
  cap: number;
  city: string;
  state: string;
  carPlate: string;
  latitude: string;
  longitude: string;
  priceMin: number;
  priceMax: number;
  status: number;
}
