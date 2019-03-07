import {BaseModel} from './base.model';

export class User extends BaseModel {
    username: string;
    password: string;
    firstName: string;
    lastName: string;
    token?: string;
}
