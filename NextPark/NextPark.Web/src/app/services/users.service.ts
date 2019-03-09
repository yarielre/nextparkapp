import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { NEXT_PARK_URL } from '../_helpers/constants';
// import {EmailSubjectMessage} from '../models/email-subject-message';
import { User } from '../models';
import { BaseService } from './base.service';
import { StorageService } from './storage.service';

@Injectable({
  providedIn: 'root'
})
export class UsersService extends BaseService<User> {
  private form: FormGroup;

  constructor(
    public httpService: HttpClient,
    private formBuilder: FormBuilder,
    private storageService: StorageService
  ) {
    super(httpService, storageService,  `${NEXT_PARK_URL.auth}/getallusers`);
    this.form = this.formBuilder.group({
      id: [0, Validators.required],
      name: ['', Validators.required],
      lastname: ['', Validators.required],
      username: ['', null],
      email: ['', null],
      phone: ['', Validators.required],
      address: ['', Validators.required],
      cap: ['', Validators.required],
      city: ['', Validators.required],
      state: ['', Validators.required],
      carPlate: ['', Validators.required],
      balance: [0, Validators.required],
      profit: [0, Validators.required]
    });
  }

  getAll(): Observable<User[]> {
    return super.getAll();
  }

  add(obj: User, url: string = this.baseUrl): Observable<User> {
    return super.add(obj, url);
  }

  update(obj: User): Observable<User> {
    return super.update(obj);
  }

  delete(id: number): Observable<User> {
    return super.delete(id);
  }

  fillForm(user: User) {
    this.form.setValue({
      id: user.id,
      name: user.name,
      lastname: user.lastname,
      username: user.username,
      email: user.email,
      phone: user.phone,
      address: user.address,
      cap: user.cap,
      city: user.city,
      state: user.state,
      carPlate: user.carPlate,
      balance: user.balance,
      profit: user.profit
    });
  }
}
