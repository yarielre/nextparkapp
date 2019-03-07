import {Injectable, NgZone} from '@angular/core';
import {MatSnackBar} from '@angular/material';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  constructor(private snackBar: MatSnackBar, private zone: NgZone) {

  }

  public success(message, action = 'close', duration = 3000) {
    this.zone.run(() => {
      this.snackBar.open(
        message,
        action,
        {
          duration,
          horizontalPosition: 'right',
          verticalPosition: 'top',
          panelClass: ['snackbar-success']
        });
    });
  }


  public error(message, action = 'close', duration = 5000) {
    this.zone.run(() => {
      this.snackBar.open(
        message,
        action,
        {
          duration,
          horizontalPosition: 'right',
          verticalPosition: 'top',
          panelClass: ['snackbar-error']
        });
    });
  }
}
