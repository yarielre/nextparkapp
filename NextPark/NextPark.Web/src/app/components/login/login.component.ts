import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup } from '@angular/forms';
import {
  StorageService
} from '../../services/storage.service';
import {
  NotificationService} from '../../services/notification.service';
import {
  AuthenticationService
} from '../../services/authentication.service';
import { LoginModel } from '../../models';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  showSpinner = false;

  constructor(
    private router: Router,
    private storage: StorageService,
    private notificationService: NotificationService,
    public authService: AuthenticationService
  ) {}

  ngOnInit() {
    console.log('Login');
  }

  toogleSpiner() {
    this.showSpinner = !this.showSpinner;
  }

  login(login: LoginModel) {
    this.toogleSpiner();
    this.authService.login(login).subscribe(
      resp => {
        this.toogleSpiner();
        this.storage.setToken(resp);
        this.router.navigate(['/myTable']);
      },
      error => {
        this.toogleSpiner();
        this.notificationService.error(error.error);
      }
    );
  }
}
