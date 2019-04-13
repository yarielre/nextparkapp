import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {BaseModel} from '../models/base.model';
import {StorageService} from '../services/storage.service';
import {ApiResponse} from '../models/api-response';
import {Transaction} from '../models/transaction.model';

export class BaseService<T extends BaseModel> {
  httpOptions = {
    headers: new HttpHeaders({'Content-Type': 'application/json'})
  };

  constructor(
    public http: HttpClient,
    public storage: StorageService,
    public baseUrl: string
  ) {
    this.httpOptions.headers = new HttpHeaders().set('Authorization', 'Bearer ' + this.storage.getToken());
  }

  public getAll(url: string = this.baseUrl): Observable<ApiResponse<T[]>> {
    console.log(url, this.httpOptions);
    return this.http.get<ApiResponse<T[]>>(url, this.httpOptions);
  }

  public add(obj: T, url: string = this.baseUrl): Observable<T> {
    return this.http.post<T>(url, obj, this.httpOptions);
  }

  public update(
    obj: T,
    url: string = `${this.baseUrl}/${obj.id}`
  ): Observable<T> {
    console.log(url, this.httpOptions, obj);
    return this.http.put<T>(url, obj, this.httpOptions);
  }

  public delete(
    id: number,
    url: string = `${this.baseUrl}/${id}`
  ): Observable<T> {
    return this.http.delete<T>(url, this.httpOptions);
  }

  protected fillForm(formFill: any) {
  }

  protected initForm() {
  }
}
