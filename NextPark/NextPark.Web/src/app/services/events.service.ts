import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Observable } from "rxjs";
import { NEXT_PARK_URL } from "../_helpers/constants/host.settings";
// import {EmailSubjectMessage} from '../models/email-subject-message';
import { Event } from "../models";
import { BaseService } from "./base.service";
import { StorageService } from "./storage.service";

@Injectable({
  providedIn: "root"
})
export class EventsService extends BaseService<Event> {
  private form: FormGroup;

  constructor(
    public httpService: HttpClient,
    private formBuilder: FormBuilder,
    private storageService: StorageService
  ) {
    super(httpService, storageService, `${NEXT_PARK_URL.events}`);
    this.form = this.formBuilder.group({
      id: [0, Validators.required],
      startDate: ["", Validators.required],
      endDate: ["", Validators.required],
      repetitionEndDate: ["", Validators.required],
      repetitionType: ["", Validators.required],
      parkingId: ["", Validators.required]
    });
  }

  getAll(): Observable<Event[]> {
    return super.getAll();
  }

  add(obj: Event, url: string = this.baseUrl): Observable<Event> {
    return super.add(obj, url);
  }

  update(obj: Event): Observable<Event> {
    return super.update(obj);
  }

  delete(id: number): Observable<Event> {
    return super.delete(id);
  }

  fillForm(parking: Event) {
    this.form.setValue({
      id: parking.id,
      startDate: parking.startDate,
      endDate: parking.endDate,
      repetitionEndDate: parking.repetitionEndDate,
      repetitionType: parking.repetitionType,
      parkingId: parking.parkingId,
    });
  }
}
