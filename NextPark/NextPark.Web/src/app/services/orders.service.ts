import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { NEXT_PARK_URL } from '../_helpers/constants/host.settings';
import { Order } from '../models';
import { BaseService } from './base.service';
import { StorageService } from './storage.service';
import {ApiResponse} from '../models/api-response';

@Injectable({
  providedIn: 'root'
})
export class OrdersService extends BaseService<Order> {
  private form: FormGroup;

  constructor(
    public httpService: HttpClient,
    private formBuilder: FormBuilder,
    private storageService: StorageService
  ) {
    super(httpService, storageService, `${NEXT_PARK_URL.orders}`);
    this.form = this.formBuilder.group({
      id: [0, Validators.required],
      price: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      orderStatus: ['', Validators.required],
      paymentStatus: ['', Validators.required],
      paymentCode: ['', Validators.required],
      parkingId: ['', Validators.required],
      userId: ['', Validators.required]
    });
  }

  getAll(): Observable<ApiResponse<Order[]>> {
    return super.getAll();
  }

  add(obj: Order, url: string = this.baseUrl): Observable<Order> {
    return super.add(obj, url);
  }

  update(obj: Order): Observable<Order> {
    return super.update(obj);
  }

  delete(id: number): Observable<Order> {
    return super.delete(id);
  }

  fillForm(parking: Order) {
    this.form.setValue({
      id: parking.id,
      price: parking.price,
      startDate: parking.startDate,
      endDate: parking.endDate,
      orderStatus: parking.orderStatus,
      paymentStatus: parking.paymentStatus,
      paymentCode: parking.paymentCode,
      parkingId: parking.parkingId,
      userId: parking.userId
    });
  }
}
