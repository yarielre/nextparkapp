import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable } from "rxjs";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { StorageService } from "../services/storage.service";
import { TokenResponse, LoginModel } from "../models";
import { NEXT_PARK_URL } from "../constants";

@Injectable({ providedIn: "root" })
export class AuthenticationService {
  httpHeaders: HttpHeaders;
  loginForm: FormGroup;

  formErrors = {
    'username': '',
    'password': ''
  };

  validationMessages = {
    username: {
      required: 'Please enter your username'
    },
    password: {
      required: 'please enter your password',
      pattern: 'The password must contain numbers and letters',
      minlength: 'Please enter more than 4 characters',
      maxlength: 'Please enter less than 25 characters'
    }
  };

  constructor(
    private http: HttpClient,
    private fb: FormBuilder,
    private storageService: StorageService
  ) {
    this.buildForm();
    this.httpHeaders = new HttpHeaders().set(
      "Authorization",
      "Bearer " + this.storageService.getToken()
    );
  }

  login(loginModel: LoginModel): Observable<TokenResponse> {
    return this.http.post<TokenResponse>(
      `${NEXT_PARK_URL.auth}/login`,
      loginModel
    );
  }

  logout() {
    return this.http.get<any>(`${NEXT_PARK_URL.auth}/logout`, {
      headers: this.httpHeaders
    });
  }

  isLoged(): boolean {
    return this.storageService.isAuthenticated();
  }

  buildForm() {
    this.loginForm = this.fb.group({
      username: ["", [Validators.required]],
      password: [
        "",
        [
          Validators.pattern("^(?=.*[0-9])(?=.*[a-zA-Z])([a-zA-Z0-9]+)$"),
          Validators.minLength(6),
          Validators.maxLength(25)
        ]
      ]
    });

    this.loginForm.valueChanges.subscribe(data => this.onValueChanged(data));
  }
  onValueChanged(data) {
      if (!this.loginForm) {
      return;
    }
    const form = this.loginForm;
    for (const field in this.formErrors) {
      if (Object.prototype.hasOwnProperty.call(this.formErrors, field)) {
        this.formErrors[field] = '';
        const control = form.get(field);
        if (control && control.dirty && !control.valid) {
          const messages = this.validationMessages[field];
          for (const key in control.errors) {
            if (Object.prototype.hasOwnProperty.call(control.errors, key)) {
              this.formErrors[field] += messages[key] + ' ';
            }
          }
        }
      }
    }
  }
}
