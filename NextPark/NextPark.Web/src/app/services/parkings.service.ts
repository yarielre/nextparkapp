import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { NEXT_PARK_URL } from '../_helpers/constants/host.settings';
import { Parking } from '../models';
import { BaseService } from './base.service';
import { StorageService } from './storage.service';

@Injectable({
  providedIn: 'root'
})
export class ParkingsService extends BaseService<Parking> {
  private form: FormGroup;

  constructor(
    public httpService: HttpClient,
    private formBuilder: FormBuilder,
    private storageService: StorageService
  ) {
    super(httpService, storageService, `${NEXT_PARK_URL.parking}`);
    this.form = this.formBuilder.group({
      id: [0, Validators.required],
      address: ['', Validators.required],
      cap: ['', Validators.required],
      city: ['', Validators.required],
      state: ['', Validators.required],
      carPlate: ['', Validators.required],
      latitude: ['', Validators.required],
      longitude: ['', Validators.required],
      priceMin: ['', Validators.required],
      priceMax: ['', Validators.required],
      status: ['', Validators.required]
    });
  }

  getAll(): Observable<Parking[]> {
    return super.getAll();
  }

  add(obj: Parking, url: string = this.baseUrl): Observable<Parking> {
    return super.add(obj, url);
  }

  update(obj: Parking): Observable<Parking> {
    return super.update(obj);
  }

  delete(id: number): Observable<Parking> {
    return super.delete(id);
  }

  fillForm(parking: Parking) {
    this.form.setValue({
      id: parking.id,
      address: parking.address,
      cap: parking.cap,
      city: parking.city,
      state: parking.state,
      carPlate: parking.carPlate,
      latitude: parking.latitude,
      longitude: parking.longitude,
      priceMin: parking.priceMin,
      priceMax: parking.priceMin,
      status: parking.status
    });
  }
}
