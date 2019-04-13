import { Injectable } from '@angular/core';
import {BaseService} from './base.service';
import {Transaction} from '../models/transaction.model';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {HttpClient} from '@angular/common/http';
import {StorageService} from './storage.service';
import {NEXT_PARK_URL} from '../_helpers/constants/host.settings';
import {Observable} from 'rxjs';
import {ApiResponse} from '../models/api-response';

@Injectable({
  providedIn: 'root'
})
export class TransactionsService extends BaseService<Transaction> {

  public form: FormGroup;

  constructor(
    private httpService: HttpClient,
    private storageService: StorageService,
    private fb: FormBuilder) {

    super(httpService, storageService, NEXT_PARK_URL.transactions);
    this.initForm();
  }

  getAll(): Observable<ApiResponse<Transaction[]>> {
    return super.getAll();
  }

  add(obj: Transaction, url: string = this.baseUrl): Observable<Transaction> {
    return super.add(obj, url);
  }

  update(obj: Transaction): Observable<Transaction> {
    return super.update(obj);
  }

  delete(id: number): Observable<Transaction> {
    return super.delete(id);
  }


  fillForm(transaction: Transaction) {
    this.form.setValue({
      id: transaction.id,
      userId: transaction.userId,
      creationDate: transaction.creationDate,
      completationDate: transaction.completationDate,
      status: transaction.status,
      type: transaction.type,
      cashMoved: transaction.cashMoved,
      purchaseId: transaction.purchaseId,
      purchaseToken: transaction.purchaseToken,
      purchaseState: transaction.purchaseState,
    });
  }

  initForm() {
    this.form = this.fb.group({
      id: [0, Validators.required],
      userId: [0, Validators.required],
      creationDate: [new  Date()],
      completationDate: [new  Date()],
      status: [0, Validators.required],
      type: [0, Validators.required],
      cashMoved: [0, Validators.required],
      purchaseId: [''],
      purchaseToken: [''],
      purchaseState: [''],
    });
  }
}

