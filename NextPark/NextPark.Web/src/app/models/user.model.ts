import { BaseModel } from './base.model';

export class User extends BaseModel {
  name: string;
  lastname: string;
  username: string;
  email: string;
  phone: string;
  address: string;
  cap: string;
  city: string;
  state: string;
  carPlate: string;
  balance: string;
  profit: string;
  token?: string;
}
