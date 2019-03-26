import { Component, OnInit } from "@angular/core";
import { MatDialogRef } from "@angular/material";
import { UsersService } from "src/app/services/users.service";

@Component({
  selector: "app-user-form",
  templateUrl: "./user-form.component.html",
  styleUrls: ["./user-form.component.scss"]
})
export class UserFormComponent implements OnInit {
  constructor(
    private userService: UsersService,
    private dialogRef: MatDialogRef<UserFormComponent>
  ) {}

  ngOnInit() {}

  onSave(user: any) {
    this.userService.update(user).subscribe(
      userUpdated => {
        this.onClose({ isUpdated: true, payload: userUpdated });
      },
      error => {
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
