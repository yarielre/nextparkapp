import { distinctUntilChanged, debounceTime } from "rxjs/operators";
import { fromEvent as observableFromEvent, Observable } from "rxjs";
import {
  Component,
  OnInit,
  ElementRef,
  ViewChild,
  ChangeDetectorRef
} from "@angular/core";

import {
  MatPaginator,
  MatSort,
  MatDialog,
  MatDialogConfig
} from "@angular/material";
import { SelectionModel } from "@angular/cdk/collections";

import { UserDataSource } from "../../_helpers/data-sources";
import { UsersService, NotificationService } from "src/app/services";
import { User } from "src/app/models";
import { UserDeleteConfirmDialogComponent } from "../_shared/delete-confirm-dialog/user-delete-confirm-dialog.component";
import { UserFormComponent } from "../_forms/user-form/user-form.component";

@Component({
  selector: "app-users",
  templateUrl: "./users.component.html",
  styleUrls: ["./users.component.scss"]
})
export class UsersComponent implements OnInit {
  showNavListCode;
  displayedColumns = [
    "select",
    "id",
    "name",
    "lastname",
    "userName",
    "email",
    "phone",
    "address",
    "cap",
    "city",
    "state",
    "carPlate",
    "balance",
    "profit"
  ];

  dataSource: UserDataSource;
  selection = new SelectionModel<string>(true, []);

  constructor(
    private userService: UsersService,
    private changeDetectorRefs: ChangeDetectorRef,
    private dialog: MatDialog,
    private notifService: NotificationService
  ) {}

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild("filter") filter: ElementRef;

  ngOnInit() {
    this.refresh();

    observableFromEvent(this.filter.nativeElement, "keyup")
      .pipe(distinctUntilChanged())
      .subscribe(() => {
        if (!this.dataSource) {
          return;
        }
        this.dataSource.filter = this.filter.nativeElement.value;
      });
  }

  onDelete(selected: string[]) {
    this.openDeleteDialog(selected);
  }

  openDeleteDialog(selected: string[]) {
    const users = this.dataSource.renderedData.filter(x =>
      selected.includes(x.id.toString())
    );
    const dialogConfig: MatDialogConfig = new MatDialogConfig();
    const message =
      users.length > 1
        ? `Are you sure you want to delete all users`
        : `Are you sure you want to delete this user <em>${
            users[0].name
          }.</em>`;
    dialogConfig.autoFocus = true;
    dialogConfig.width = "50%";
    dialogConfig.data = {
      title: "Delete User",
      text: message,
      payload: users.map(x => x.id)
    };

    this.dialog
      .open(UserDeleteConfirmDialogComponent, dialogConfig)
      .afterClosed()
      .subscribe(deleteDialogdata => {
        if (deleteDialogdata !== undefined && deleteDialogdata.isOnDelete) {
          this.refresh();
          this.notifService.success("Deleted successfully.");
        }
      });
  }

  onEdit(selected: string[]) {
    const user = this.dataSource.renderedData.filter(x =>
      selected.includes(x.id.toString())
    );

    this.userService.fillForm(user[0]);
    this.openEditDialog();
  }

  openEditDialog() {
    const dialogConfig: MatDialogConfig = new MatDialogConfig();
    dialogConfig.autoFocus = true;
    dialogConfig.width = "50%";
    this.dialog
      .open(UserFormComponent, dialogConfig)
      .afterClosed()
      .subscribe(usertEditedResult => {
        if (usertEditedResult !== undefined && usertEditedResult.isUpdated) {
          if (usertEditedResult.payload !== undefined) {
            this.refresh();
            this.notifService.success("User updated.");
          } else {
            this.notifService.success("Server error");
          }
        }
      });
  }

  refresh() {
    this.userService.getAll().subscribe(res => {
      this.dataSource = new UserDataSource(this.paginator, this.sort, res.result);
      this.changeDetectorRefs.detectChanges();
    });
  }

  isAllSelected(): boolean {
    if (!this.dataSource) {
      return false;
    }
    if (this.selection.isEmpty()) {
      return false;
    }
    return (
      this.selection.selected.length === this.dataSource.renderedData.length
    );
  }

  masterToggle() {
    if (!this.dataSource) {
      return;
    }

    if (this.isAllSelected()) {
      this.selection.clear();
    } else {
      this.dataSource.renderedData.forEach(data =>
        this.selection.select(data.id.toString())
      );
    }
  }
}
