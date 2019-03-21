import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { ParkingsService } from 'src/app/services/parkings.service';

@Component({
  selector: 'app-parking-form',
  templateUrl: './parking-form.component.html',
  styleUrls: ['./parking-form.component.scss']
})
export class ParkingFormComponent implements OnInit {

  constructor(
    private parkingService: ParkingsService,
    private dialogRef: MatDialogRef<ParkingFormComponent>) { }

  ngOnInit() {
  }

  onSave(parking: any) {
    this.parkingService.update(parking).subscribe(
      parkingUpdated => {
        this.onClose({ isUpdated: true, payload: parkingUpdated });
      },
      error => {
        console.log(error)
        this.dialogRef.close({ isUpdated: true });
      }
    );
  }

  onClose(dialogResult) {
    if (dialogResult) {
      this.dialogRef.close(dialogResult);
    } else {
      this.dialogRef.close({ isUpdated: false });
    }
  }

}
