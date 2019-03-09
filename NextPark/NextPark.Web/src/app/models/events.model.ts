import { BaseModel } from "./base.model";

export class Event extends BaseModel {
  startDate: string;
  endDate: string;
  repetitionEndDate: string;
  repetitionType: number;
  parkingId: number;
}
