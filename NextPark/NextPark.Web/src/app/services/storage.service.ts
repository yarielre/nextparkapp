import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { TokenResponse } from '../models';

@Injectable({  providedIn: 'root'})
export class StorageService {
  private token: string;

  constructor(private router: Router) {
    this.token = this.loadToken();
  }

  loadToken(): string {
    const token = localStorage.getItem('tokenResp');
    return token ? token : null;
  }

  setToken(tokenResp: TokenResponse): void {
    this.token = tokenResp.authToken;
    localStorage.setItem('tokenResp', tokenResp.authToken);
  }

  public getToken(): string {
    return this.token;
  }

  removeTokenResponse(): void {
    localStorage.removeItem('tokenResp');
    this.token = null;
  }

  isAuthenticated(): boolean {
    return this.token !== null ? true : false;
  }
  logout(): void {
    this.removeTokenResponse();
    this.router.navigate(['/login']);
  }
}
