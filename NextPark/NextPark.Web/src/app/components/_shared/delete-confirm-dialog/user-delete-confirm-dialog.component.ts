import { Component, Inject, OnInit } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material";
import { UsersService, NotificationService } from "../../../services";

@Component({
  templateUrl: "./delete-confirm-dialog.component.html",
  styleUrls: ["./delete-confirm-dialog.component.scss"]
})
export class UserDeleteConfirmDialogComponent implements OnInit {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private matDialogRef: MatDialogRef<UserDeleteConfirmDialogComponent>,
    private userService: UsersService,
    private notifcationService: NotificationService
  ) {}

  ngOnInit() {}

  onDelete() {
    this.userService.delete(this.data.payload).subscribe(
      user => {
        this.matDialogRef.close({
          isOnDelete: true,
          payload: this.data.payload
        });
      },
      error => {
        this.notifcationService.error("Server error");
        this.matDialogRef.close();
      }
    );
  }

  onCancel() {
    this.matDialogRef.close({ isOnDelete: false });
  }
}
