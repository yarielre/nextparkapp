import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../../../services/authentication.service';
import {  NotificationService } from '../../../services/notification.service';
import { StorageService} from '../../../services/storage.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.scss']
})
export class NavBarComponent implements OnInit {
  constructor(
    public authService: AuthenticationService,
    private storage: StorageService,
    private router: Router,
    private notificationService: NotificationService
  ) {}

  ngOnInit() {}

  logout() {
    this.authService.logout().subscribe(
      resp => {
        this.storage.logout();
      },
      error => {
        console.log(error)
        this.notificationService.error(error.error);
      }
    );
  }
}
