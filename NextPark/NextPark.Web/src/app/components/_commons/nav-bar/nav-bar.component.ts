import { Component, OnInit } from '@angular/core';
import { AuthenticationService, StorageService, NotificationService } from '../../../services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.scss']
})
export class NavBarComponent implements OnInit {
  constructor(
    private authService: AuthenticationService,
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
        this.notificationService.error(error.error);
      }
    );
  }
}
