import { BaseModel } from './base.model';

export class Order extends BaseModel {
  price: number;
  startDate: string;
  endDate: string;
  orderStatus: number;
  paymentStatus: number;
  paymentCode: string;
  parkingId: number;
  userId: number;
}
