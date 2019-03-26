import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {EmailSubjectMessage} from '../models';
import {StorageService} from '../services/storage.service';

@Injectable({
  providedIn: 'root'
})
export class EmailSenderService {

  form: FormGroup;
  httpHeaders: HttpHeaders;


  constructor(private http: HttpClient,
              private fb: FormBuilder,
              private storageService: StorageService) {
    this.initForm();
    this.httpHeaders = new HttpHeaders().set('Authorization', 'Bearer ' + this.storageService.getToken());
  }

  private initForm() {
    this.form = this.fb.group({
      subject: [''],
      message: ['', Validators.required]
    });
  }

  sendEmailToAllUsers(url: string, emailBody: EmailSubjectMessage) {
    return this.http.post(url, emailBody, {headers: this.httpHeaders});
  }

  sendEmailToUser(url: string, emailBody: EmailSubjectMessage, id: number) {
    return this.http.post(url,
      {id: id, subject: emailBody.subject, message: emailBody.message},
      {headers: this.httpHeaders});
  }
}
