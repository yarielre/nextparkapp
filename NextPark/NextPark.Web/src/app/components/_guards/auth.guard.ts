import { Injectable } from '@angular/core';
import {
  Router,
  CanActivate
} from '@angular/router';

import { StorageService } from '../../services/storage.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(private router: Router, private storageService: StorageService) {}

  canActivate() {
    console.log('Hello storage ', this.storageService.isAuthenticated());
    if (this.storageService.isAuthenticated()) {
      return true;
    }

    // not logged in so redirect to login page
    this.router.navigate(['/login']);
    return false;
  }
}
